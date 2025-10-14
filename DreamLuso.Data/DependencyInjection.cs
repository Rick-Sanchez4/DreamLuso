using DreamLuso.Data.Context;
using DreamLuso.Data.Interceptors;
using DreamLuso.Data.Repositories;
using DreamLuso.Data.Uow;
using DreamLuso.Domain.Core.Interfaces;
using DreamLuso.Domain.Core.Uow;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DreamLuso.Data;

public static class DependencyInjection
{
    public static IServiceCollection AddDataServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Add DbContext with interceptors
        services.AddSingleton<AuditableEntityInterceptor>();
        
        services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
        {
            var connectionString = configuration.GetConnectionString("DreamLusoDB") 
                ?? throw new InvalidOperationException("Connection string 'DreamLusoDB' not found.");
            
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null);
                sqlOptions.CommandTimeout(180);
            });
            
            // Add interceptor
            var auditInterceptor = serviceProvider.GetRequiredService<AuditableEntityInterceptor>();
            options.AddInterceptors(auditInterceptor);
        });

        // Register repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IClientRepository, ClientRepository>();
        services.AddScoped<IRealEstateAgentRepository, RealEstateAgentRepository>();
        services.AddScoped<IPropertyRepository, PropertyRepository>();
        services.AddScoped<IPropertyImageRepository, PropertyImageRepository>();
        services.AddScoped<IPropertyVisitRepository, PropertyVisitRepository>();
        services.AddScoped<IContractRepository, ContractRepository>();

        // Register Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}

