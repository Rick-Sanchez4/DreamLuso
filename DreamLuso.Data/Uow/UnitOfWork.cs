using DreamLuso.Data.Context;
using DreamLuso.Domain.Core.Interfaces;
using DreamLuso.Domain.Core.Uow;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data.Common;
using System.Text;

namespace DreamLuso.Data.Uow;

internal class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;
    private bool _disposed;

    // Repositories
    public IUserRepository UserRepository { get; }
    public IClientRepository ClientRepository { get; }
    public IRealEstateAgentRepository RealEstateAgentRepository { get; }
    public IPropertyRepository PropertyRepository { get; }
    public IPropertyImageRepository PropertyImageRepository { get; }
    public IPropertyVisitRepository PropertyVisitRepository { get; }
    public IContractRepository ContractRepository { get; }

    public UnitOfWork(
        ApplicationDbContext context,
        IUserRepository userRepository,
        IClientRepository clientRepository,
        IRealEstateAgentRepository realEstateAgentRepository,
        IPropertyRepository propertyRepository,
        IPropertyImageRepository propertyImageRepository,
        IPropertyVisitRepository propertyVisitRepository,
        IContractRepository contractRepository)
    {
        _context = context;
        UserRepository = userRepository;
        ClientRepository = clientRepository;
        RealEstateAgentRepository = realEstateAgentRepository;
        PropertyRepository = propertyRepository;
        PropertyImageRepository = propertyImageRepository;
        PropertyVisitRepository = propertyVisitRepository;
        ContractRepository = contractRepository;
    }

    public bool Commit()
    {
        return _context.SaveChanges() > 0;
    }

    public async Task<bool> CommitAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<DbTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        return _transaction.GetDbTransaction();
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            
            if (_transaction != null)
            {
                await _transaction.CommitAsync(cancellationToken);
            }
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    public bool HasChanges()
    {
        return _context.ChangeTracker.HasChanges();
    }

    public IEnumerable<string> DebugChanges()
    {
        var changes = new StringBuilder();
        foreach (var entry in _context.ChangeTracker.Entries())
        {
            if (entry.State == EntityState.Added || entry.State == EntityState.Modified || entry.State == EntityState.Deleted)
            {
                changes.AppendLine($"Entity: {entry.Entity.GetType().Name}");
                changes.AppendLine($"State: {entry.State}");

                foreach (var property in entry.OriginalValues.Properties)
                {
                    var originalValue = entry.OriginalValues[property]?.ToString();
                    var currentValue = entry.CurrentValues[property]?.ToString();

                    if (entry.State == EntityState.Added)
                    {
                        changes.AppendLine($"Property: {property.Name} | New Value: {currentValue}");
                    }
                    else if (entry.State == EntityState.Deleted)
                    {
                        changes.AppendLine($"Property: {property.Name} | Original Value: {originalValue}");
                    }
                    else if (entry.State == EntityState.Modified && originalValue != currentValue)
                    {
                        changes.AppendLine($"Property: {property.Name} | Original Value: {originalValue} | Current Value: {currentValue}");
                    }
                }
                changes.AppendLine();
            }
        }
        return changes.ToString().Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _transaction?.Dispose();
                _context.Dispose();
            }
            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}

