using DreamLuso.Data.Context;
using DreamLuso.Domain.Core.Interfaces;
using DreamLuso.Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace DreamLuso.Data.Repositories;

public class CommentRepository : Repository<Comment>, ICommentRepository
{
    public CommentRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override async Task<Comment> SaveAsync(Comment comment)
    {
        if (comment == null)
            throw new ArgumentNullException(nameof(comment));

        await _dbSet.AddAsync(comment);
        return comment;
    }

    public override async Task<Comment> UpdateAsync(Comment comment)
    {
        if (comment == null)
            throw new ArgumentNullException(nameof(comment));

        _dbSet.Update(comment);
        return comment;
    }

    public async Task<IEnumerable<Comment>> GetByPropertyAsync(Guid propertyId)
    {
        return await _dbSet
            .Include(c => c.User)
            .Include(c => c.Replies)
                .ThenInclude(r => r.User)
            .Where(c => c.PropertyId == propertyId && !c.ParentCommentId.HasValue && !c.IsFlagged)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Comment>> GetByUserAsync(Guid userId)
    {
        return await _dbSet
            .Include(c => c.Property)
            .Where(c => c.UserId == userId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<Comment?> GetByIdWithRepliesAsync(Guid id)
    {
        return await _dbSet
            .Include(c => c.User)
            .Include(c => c.Replies)
                .ThenInclude(r => r.User)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<double> GetAverageRatingAsync(Guid propertyId)
    {
        var ratings = await _dbSet
            .Where(c => c.PropertyId == propertyId && c.Rating.HasValue && !c.IsFlagged)
            .Select(c => c.Rating!.Value)
            .ToListAsync();

        return ratings.Any() ? ratings.Average() : 0;
    }

    public async Task<int> GetCommentCountAsync(Guid propertyId)
    {
        return await _dbSet
            .CountAsync(c => c.PropertyId == propertyId && !c.IsFlagged);
    }

    public async Task<IEnumerable<Comment>> GetFlaggedCommentsAsync()
    {
        return await _dbSet
            .Include(c => c.User)
            .Include(c => c.Property)
            .Where(c => c.IsFlagged)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public override async Task DeleteAsync(Guid id)
    {
        var comment = await _dbSet
            .Include(c => c.Replies)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (comment != null)
        {
            // Delete all replies first
            _dbSet.RemoveRange(comment.Replies);
            _dbSet.Remove(comment);
        }
    }
}

