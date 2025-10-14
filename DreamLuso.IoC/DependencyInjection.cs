using DreamLuso.Application;
using DreamLuso.Data;
using DreamLuso.Security;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DreamLuso.IoC;

public static class DependencyInjection
{
    public static IServiceCollection AddIoCServices(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        // Add layer services
        services.AddDataServices(configuration);
        services.AddApplicationServices(configuration, environment);
        services.AddSecurityServices(configuration);

        return services;
    }
}

