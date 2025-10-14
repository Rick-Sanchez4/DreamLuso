using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MediatR;
using FluentValidation;
using DreamLuso.Application.Common.Behaviours;
using DreamLuso.Application.Common.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace DreamLuso.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        // Add MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));
        
        // Add FluentValidation
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        
        // Add behaviors
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnitOfWorkBehaviour<,>));
        
        // Add File Upload Service
        var uploadPath = Path.Combine(environment.WebRootPath ?? environment.ContentRootPath, "uploads");
        var baseUrl = configuration["AppSettings:BaseUrl"] ?? "http://localhost:5000";
        services.AddSingleton<IFileUploadService>(sp => 
            new FileUploadService(
                sp.GetRequiredService<ILogger<FileUploadService>>(), 
                uploadPath, 
                baseUrl
            ));
        
        return services;
    }
}
