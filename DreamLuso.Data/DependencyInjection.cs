using DreamLuso.Data.Context;
using DreamLuso.Data.Infrastructure;
using DreamLuso.Data.Interceptors;
using DreamLuso.Data.Repositories;
using DreamLuso.Data.Uow;
using DreamLuso.Domain.Core.Interfaces;
using DreamLuso.Domain.Core.Uow;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql.EntityFrameworkCore.PostgreSQL;

namespace DreamLuso.Data;

public static class DependencyInjection
{
    public static IServiceCollection AddDataServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Add DbContext with interceptors
        services.AddSingleton<AuditableEntityInterceptor>();
        
        services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
        {
            var connectionString = configuration.GetConnectionString("DreamLusoDB");
            
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    "Connection string 'DreamLusoDB' not found or is empty. " +
                    "Please configure ConnectionStrings__DreamLusoDB environment variable.");
            }
            
            // Converter connection string do formato postgresql:// para formato tradicional se necessário
            if (connectionString.StartsWith("postgresql://"))
            {
                connectionString = ConvertPostgresUrlToConnectionString(connectionString);
            }
            
            // Detectar o provider baseado na connection string ou variável de ambiente
            var databaseProvider = configuration["DatabaseProvider"] ?? DetectProvider(connectionString);
            
            switch (databaseProvider.ToLower())
            {
                case "postgresql":
                case "postgres":
                    options.UseNpgsql(connectionString, npgsqlOptions =>
                    {
                        npgsqlOptions.CommandTimeout(30);
                        npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "public");
                    });
                    break;
                    
                case "sqlite":
                    options.UseSqlite(connectionString, sqliteOptions =>
                    {
                        sqliteOptions.CommandTimeout(30);
                    });
                    break;
                    
                case "sqlserver":
                default:
                    options.UseSqlServer(connectionString, sqlOptions =>
                    {
                        sqlOptions.CommandTimeout(30);
                    });
                    break;
            }
            
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
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IPropertyProposalRepository, PropertyProposalRepository>();
        services.AddScoped<ICommentRepository, CommentRepository>();
        
        // Register services
        services.AddScoped<IFileStorageService, FileStorageService>();

        // Register Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
    
    private static string ConvertPostgresUrlToConnectionString(string postgresUrl)
    {
        // Converter postgresql://user:password@host:port/database para formato tradicional
        // Exemplo: postgresql://user:pass@host:5432/db -> Host=host;Port=5432;Database=db;Username=user;Password=pass
        
        var uri = new Uri(postgresUrl);
        var connectionString = $"Host={uri.Host};";
        
        if (uri.Port != -1)
            connectionString += $"Port={uri.Port};";
        
        if (!string.IsNullOrEmpty(uri.AbsolutePath) && uri.AbsolutePath.Length > 1)
            connectionString += $"Database={uri.AbsolutePath.Substring(1)};";
        
        if (!string.IsNullOrEmpty(uri.UserInfo))
        {
            var userInfo = uri.UserInfo.Split(':');
            if (userInfo.Length >= 1)
                connectionString += $"Username={userInfo[0]};";
            if (userInfo.Length >= 2)
                connectionString += $"Password={userInfo[1]};";
        }
        
        return connectionString;
    }
    
    private static string DetectProvider(string connectionString)
    {
        // Detectar provider baseado na connection string
        if (connectionString.StartsWith("postgresql://") || connectionString.Contains("Host=") || 
            (connectionString.Contains("Server=") && connectionString.Contains("Database=") && !connectionString.Contains("TrustServerCertificate")))
            return "postgresql";
        
        if (connectionString.Contains("Data Source=") && connectionString.EndsWith(".db"))
            return "sqlite";
        
        // Default: SQL Server
        return "sqlserver";
    }
}

