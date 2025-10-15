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

    // Existing Repositories
    public IUserRepository UserRepository { get; }
    public IClientRepository ClientRepository { get; }
    public IRealEstateAgentRepository RealEstateAgentRepository { get; }
    public IPropertyRepository PropertyRepository { get; }
    public IPropertyImageRepository PropertyImageRepository { get; }
    public IPropertyVisitRepository PropertyVisitRepository { get; }
    public IContractRepository ContractRepository { get; }
    
    // New Repositories
    public INotificationRepository NotificationRepository { get; }
    public IPropertyProposalRepository PropertyProposalRepository { get; }
    public ICommentRepository CommentRepository { get; }

    public UnitOfWork(
        ApplicationDbContext context,
        IUserRepository userRepository,
        IClientRepository clientRepository,
        IRealEstateAgentRepository realEstateAgentRepository,
        IPropertyRepository propertyRepository,
        IPropertyImageRepository propertyImageRepository,
        IPropertyVisitRepository propertyVisitRepository,
        IContractRepository contractRepository,
        INotificationRepository notificationRepository,
        IPropertyProposalRepository propertyProposalRepository,
        ICommentRepository commentRepository)
    {
        _context = context;
        UserRepository = userRepository;
        ClientRepository = clientRepository;
        RealEstateAgentRepository = realEstateAgentRepository;
        PropertyRepository = propertyRepository;
        PropertyImageRepository = propertyImageRepository;
        PropertyVisitRepository = propertyVisitRepository;
        ContractRepository = contractRepository;
        NotificationRepository = notificationRepository;
        PropertyProposalRepository = propertyProposalRepository;
        CommentRepository = commentRepository;
    }

    public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RollbackAsync()
    {
        await Task.Run(() =>
        {
            foreach (var entry in _context.ChangeTracker.Entries())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.State = EntityState.Detached;
                        break;
                    case EntityState.Modified:
                    case EntityState.Deleted:
                        entry.Reload();
                        break;
                }
            }
        });
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
