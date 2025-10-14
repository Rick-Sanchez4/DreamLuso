using System.Security.Claims;

namespace DreamLuso.Security.Interfaces;

public interface ITokenService
{
    string GenerateToken(object user);
    string GenerateRefreshToken();
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}

