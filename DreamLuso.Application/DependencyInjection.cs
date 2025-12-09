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
        
        // Add File Upload Service
        // Use wwwroot/images as base path for static file serving
        var uploadPath = Path.Combine(environment.WebRootPath ?? environment.ContentRootPath, "images");
        var baseUrl = configuration["AppSettings:BaseUrl"] ?? "http://localhost:5149";
        
        // Ensure required directories exist
        var profilesPath = Path.Combine(uploadPath, "profiles");
        var propertiesPath = Path.Combine(uploadPath, "properties");
        if (!Directory.Exists(profilesPath))
        {
            Directory.CreateDirectory(profilesPath);
        }
        if (!Directory.Exists(propertiesPath))
        {
            Directory.CreateDirectory(propertiesPath);
        }
        
        services.AddSingleton<IFileUploadService>(sp => 
            new FileUploadService(
                sp.GetRequiredService<ILogger<FileUploadService>>(), 
                uploadPath, 
                baseUrl
            ));
        
        // Add Email Service
        services.AddScoped<IEmailService, EmailService>();
        
        // Add PDF Generation Service
        services.AddScoped<IPdfGenerationService, PdfGenerationService>();
        
        return services;
    }
}

