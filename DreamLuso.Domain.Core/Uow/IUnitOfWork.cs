using DreamLuso.Domain.Core.Interfaces;

namespace DreamLuso.Domain.Core.Uow;

public interface IUnitOfWork : IDisposable
{
    // Existing Repositories
    IUserRepository UserRepository { get; }
    IClientRepository ClientRepository { get; }
    IRealEstateAgentRepository RealEstateAgentRepository { get; }
    IPropertyRepository PropertyRepository { get; }
    IPropertyImageRepository PropertyImageRepository { get; }
    IPropertyVisitRepository PropertyVisitRepository { get; }
    IContractRepository ContractRepository { get; }
    
    // New Repositories
    INotificationRepository NotificationRepository { get; }
    IPropertyProposalRepository PropertyProposalRepository { get; }
    ICommentRepository CommentRepository { get; }
    
    // Services
    IFileStorageService FileStorageService { get; }

    Task<int> CommitAsync(CancellationToken cancellationToken = default);
    Task RollbackAsync();
}
