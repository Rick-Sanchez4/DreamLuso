using DreamLuso.IoC;
using DreamLuso.WebAPI.Endpoints;
using DreamLuso.WebAPI.Middleware;
using DreamLuso.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configure port for Render (Render sets PORT environment variable)
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://+:{port}");

// Add services to the container.
builder.Services.AddControllers(); // Para usar Controllers (PropertyController)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // Evita conflitos de nomes de schemas
    c.CustomSchemaIds(type => type.FullName);
    
    // Configure JWT authentication in Swagger
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header usando o esquema Bearer. Exemplo: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    options.SerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
});

// CORS Configuration - prioritize environment variable, then config, then defaults
var corsOriginsEnv = Environment.GetEnvironmentVariable("CORS_ALLOWED_ORIGINS");
var configOrigins = builder.Configuration.GetSection("CorsSettings:AllowedOrigins").Get<string[]>() 
    ?? Array.Empty<string>();

var defaultOrigins = new[] { 
    "http://localhost:4200",
    "https://dream-luso.vercel.app",
    "https://dreamluso-api.onrender.com",
    "https://dreamluso-frontend.onrender.com",
    "https://lifelong-jamal-scarless.ngrok-free.dev"
};

var allowedOrigins = !string.IsNullOrEmpty(corsOriginsEnv)
    ? corsOriginsEnv.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
    : configOrigins.Length > 0 
        ? configOrigins 
        : defaultOrigins;

// Ensure Vercel domain is always included
if (!allowedOrigins.Contains("https://dream-luso.vercel.app"))
{
    allowedOrigins = allowedOrigins.Concat(new[] { "https://dream-luso.vercel.app" }).ToArray();
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials()
              .SetPreflightMaxAge(TimeSpan.FromSeconds(86400)); // Cache preflight for 24 hours
    });
    
    // Add a more permissive policy for development (optional - can be removed in production)
    if (builder.Environment.IsDevelopment())
    {
        options.AddDefaultPolicy(policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
    }
});

// Add application services
builder.Services.AddIoCServices(builder.Configuration, builder.Environment);
builder.Services.AddSecurityServices(builder.Configuration);

// JWT Authentication
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
});
builder.Services.AddAuthorization();
builder.Services.AddHealthChecks();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Log CORS configuration on startup
var corsLogger = app.Services.GetRequiredService<ILogger<Program>>();
corsLogger.LogInformation("CORS Allowed Origins: {Origins}", string.Join(", ", allowedOrigins));

// Apply migrations automatically on startup
try
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<DreamLuso.Data.Context.ApplicationDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        
        logger.LogInformation("Applying database migrations...");
        
        // Ensure database exists
        dbContext.Database.EnsureCreated();
        
        // Apply migrations
        dbContext.Database.Migrate();
        
        logger.LogInformation("Database migrations applied successfully.");
    }
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred while applying database migrations: {Error}", ex.Message);
    // Don't throw - let the app start anyway
}

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

// CORS must be before other middleware - handles OPTIONS preflight automatically
// Use default policy in development, specific policy in production
if (app.Environment.IsDevelopment())
{
    app.UseCors(); // Uses default policy
}
else
{
    app.UseCors("AllowAngularApp"); // Uses specific policy with credentials
}

// Global exception handling
app.UseMiddleware<GlobalExceptionMiddleware>();

// Static Files (for serving uploaded images)
app.UseStaticFiles();

// HTTPS redirection (after CORS)
app.UseHttpsRedirection();

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// Register endpoints
app.RegisterEndpoints();

// Health Check Endpoint
app.MapHealthChecks("/health");

// Temporary migration endpoint
app.MapPost("/migrate", async (IServiceProvider serviceProvider) =>
{
    try
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DreamLuso.Data.Context.ApplicationDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        
        logger.LogInformation("Applying database migrations via endpoint...");
        
        // Ensure database exists
        await dbContext.Database.EnsureCreatedAsync();
        
        // Apply migrations
        await dbContext.Database.MigrateAsync();
        
        logger.LogInformation("Database migrations applied successfully via endpoint.");
        
        return Results.Ok(new { message = "Migrations applied successfully" });
    }
    catch (Exception ex)
    {
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Error applying migrations via endpoint: {Error}", ex.Message);
        return Results.Problem($"Error applying migrations: {ex.Message}");
    }
});

// Map Controllers (PropertyController)
app.MapControllers();

app.Run();

