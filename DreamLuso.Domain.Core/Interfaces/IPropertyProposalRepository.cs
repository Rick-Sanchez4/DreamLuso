using DreamLuso.Domain.Model;

namespace DreamLuso.Domain.Core.Interfaces;

public interface IPropertyProposalRepository : IRepository<PropertyProposal>
{
    Task<PropertyProposal> SaveAsync(PropertyProposal proposal);
    Task<PropertyProposal> UpdateAsync(PropertyProposal proposal);
    Task<PropertyProposal?> GetByIdWithNegotiationsAsync(Guid id);
    Task<IEnumerable<PropertyProposal>> GetByPropertyAsync(Guid propertyId);
    Task<IEnumerable<PropertyProposal>> GetByClientAsync(Guid clientId);
    Task<IEnumerable<PropertyProposal>> GetByAgentAsync(Guid agentId);
    Task<IEnumerable<PropertyProposal>> GetByStatusAsync(ProposalStatus status);
    Task<PropertyProposal?> GetByProposalNumberAsync(string proposalNumber);
    Task<bool> HasPendingProposalAsync(Guid clientId, Guid propertyId);
}

