using DreamLuso.Domain.Core.Interfaces;
using System.Data.Common;

namespace DreamLuso.Domain.Core.Uow;

public interface IUnitOfWork : IDisposable
{
    // Repository Properties
    IUserRepository UserRepository { get; }
    IClientRepository ClientRepository { get; }
    IRealEstateAgentRepository RealEstateAgentRepository { get; }
    IPropertyRepository PropertyRepository { get; }
    IPropertyImageRepository PropertyImageRepository { get; }
    IPropertyVisitRepository PropertyVisitRepository { get; }
    IContractRepository ContractRepository { get; }

    // Commit Operations
    bool Commit();
    Task<bool> CommitAsync(CancellationToken cancellationToken = default);

    // Transaction Operations
    Task<DbTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    // Utility Methods
    bool HasChanges();
    IEnumerable<string> DebugChanges();
}
