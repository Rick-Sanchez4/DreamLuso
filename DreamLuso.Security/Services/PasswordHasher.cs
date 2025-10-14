using DreamLuso.Security.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace DreamLuso.Security.Services;

public class PasswordHasher : IPasswordHasher
{
    public void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("A password não pode estar vazia", nameof(password));

        using (var hmac = new HMACSHA512())
        {
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }
    }

    public bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
    {
        if (password == null) throw new ArgumentNullException(nameof(password));
        if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("A password não pode estar vazia", nameof(password));
        if (storedHash.Length != 64) throw new ArgumentException("Comprimento inválido de password hash (64 bytes esperados).", nameof(storedHash));
        if (storedSalt.Length != 128) throw new ArgumentException("Comprimento inválido de password salt (128 bytes esperados).", nameof(storedSalt));

        using (var hmac = new HMACSHA512(storedSalt))
        {
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(storedHash);
        }
    }
}

