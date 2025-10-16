using DreamLuso.Data.Context;
using DreamLuso.Domain.Core.Interfaces;
using DreamLuso.Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace DreamLuso.Data.Repositories;

public class PropertyProposalRepository : Repository<PropertyProposal>, IPropertyProposalRepository
{
    public PropertyProposalRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override async Task<PropertyProposal?> GetByIdAsync(Guid id)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Id cannot be empty", nameof(id));

        return await _dbSet
            .Include(p => p.Property)
            .Include(p => p.Client)
                .ThenInclude(c => c.User)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public override async Task<PropertyProposal> SaveAsync(PropertyProposal proposal)
    {
        if (proposal == null)
            throw new ArgumentNullException(nameof(proposal));

        await _dbSet.AddAsync(proposal);
        return proposal;
    }

    public override async Task<PropertyProposal> UpdateAsync(PropertyProposal proposal)
    {
        if (proposal == null)
            throw new ArgumentNullException(nameof(proposal));

        _dbSet.Update(proposal);
        return proposal;
    }

    public async Task<PropertyProposal?> GetByIdWithNegotiationsAsync(Guid id)
    {
        return await _dbSet
            .Include(p => p.Property)
            .Include(p => p.Client)
                .ThenInclude(c => c.User)
            .Include(p => p.Negotiations)
                .ThenInclude(n => n.Sender)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<PropertyProposal>> GetByPropertyAsync(Guid propertyId)
    {
        return await _dbSet
            .Include(p => p.Client)
                .ThenInclude(c => c.User)
            .Include(p => p.Negotiations)
            .Where(p => p.PropertyId == propertyId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<PropertyProposal>> GetByClientAsync(Guid clientId)
    {
        return await _dbSet
            .Include(p => p.Property)
                .ThenInclude(pr => pr.Address)
            .Include(p => p.Negotiations)
            .Where(p => p.ClientId == clientId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<PropertyProposal>> GetByAgentAsync(Guid agentId)
    {
        return await _dbSet
            .Include(p => p.Property)
                .ThenInclude(pr => pr!.Address)
            .Include(p => p.Client)
                .ThenInclude(c => c!.User)
            .Include(p => p.Negotiations)
            .Where(p => p.Property!.RealEstateAgentId == agentId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<PropertyProposal>> GetByStatusAsync(ProposalStatus status)
    {
        return await _dbSet
            .Include(p => p.Property)
            .Include(p => p.Client)
                .ThenInclude(c => c.User)
            .Where(p => p.Status == status)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<PropertyProposal?> GetByProposalNumberAsync(string proposalNumber)
    {
        return await _dbSet
            .Include(p => p.Property)
            .Include(p => p.Client)
            .Include(p => p.Negotiations)
            .FirstOrDefaultAsync(p => p.ProposalNumber == proposalNumber);
    }

    public async Task<bool> HasPendingProposalAsync(Guid clientId, Guid propertyId)
    {
        return await _dbSet.AnyAsync(p => 
            p.ClientId == clientId && 
            p.PropertyId == propertyId && 
            (p.Status == ProposalStatus.Pending || p.Status == ProposalStatus.InNegotiation));
    }
}

