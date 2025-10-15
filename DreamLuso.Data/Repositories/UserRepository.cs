using DreamLuso.Data.Context;
using DreamLuso.Domain.Core.Interfaces;
using DreamLuso.Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace DreamLuso.Data.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override async Task<User> SaveAsync(User user)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));

        await _dbSet.AddAsync(user);
        return user;
    }

    public override async Task<User?> GetByIdAsync(Guid id)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Id cannot be empty", nameof(id));

        return await _dbSet
            .Include(u => u.Client)
            .Include(u => u.RealEstateAgent)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be null or empty", nameof(email));

        return await _dbSet
            .Include(u => u.Client)
            .Include(u => u.RealEstateAgent)
            .FirstOrDefaultAsync(u => u.Email == email.ToLower());
    }

    public async Task<User?> GetByRefreshTokenAsync(string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            throw new ArgumentException("Refresh token cannot be null or empty", nameof(refreshToken));

        return await _dbSet
            .Include(u => u.Client)
            .Include(u => u.RealEstateAgent)
            .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        return await _dbSet.AnyAsync(u => u.Email == email.ToLower());
    }

    public async Task<IEnumerable<User>> GetByRoleAsync(UserRole role)
    {
        return await _dbSet
            .Include(u => u.Client)
            .Include(u => u.RealEstateAgent)
            .Where(u => u.Role == role)
            .ToListAsync();
    }

    public async Task<IEnumerable<User>> GetActiveUsersAsync()
    {
        return await _dbSet
            .Include(u => u.Client)
            .Include(u => u.RealEstateAgent)
            .Where(u => u.IsActive)
            .ToListAsync();
    }

    public override async Task<User> UpdateAsync(User user)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));

        _dbSet.Update(user);
        return user;
    }
}
