using DreamLuso.Domain.Model;

namespace DreamLuso.Domain.Core.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User> SaveAsync(User user);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByRefreshTokenAsync(string refreshToken);
    Task<bool> EmailExistsAsync(string email);
    Task<IEnumerable<User>> GetByRoleAsync(UserRole role);
    Task<User> UpdateAsync(User user);
}
