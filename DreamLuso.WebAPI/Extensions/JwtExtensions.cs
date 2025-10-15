using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace DreamLuso.WebAPI.Extensions;

public static class JwtExtensions
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication()
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = configuration["JwtSettings:Issuer"]!,
                        ValidateAudience = true,
                        ValidAudience = configuration["JwtSettings:Audience"]!,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:Key"]!)),
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };
                });
        return services;
    }
}

