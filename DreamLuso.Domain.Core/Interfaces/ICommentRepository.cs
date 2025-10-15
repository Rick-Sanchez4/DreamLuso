using DreamLuso.Domain.Model;

namespace DreamLuso.Domain.Core.Interfaces;

public interface ICommentRepository : IRepository<Comment>
{
    Task<Comment> SaveAsync(Comment comment);
    Task<Comment> UpdateAsync(Comment comment);
    Task<IEnumerable<Comment>> GetByPropertyAsync(Guid propertyId);
    Task<IEnumerable<Comment>> GetByUserAsync(Guid userId);
    Task<Comment?> GetByIdWithRepliesAsync(Guid id);
    Task<double> GetAverageRatingAsync(Guid propertyId);
    Task<int> GetCommentCountAsync(Guid propertyId);
    Task<IEnumerable<Comment>> GetFlaggedCommentsAsync();
    Task DeleteAsync(Guid id);
}

