using DreamLuso.IoC;
using DreamLuso.WebAPI.Endpoints;
using DreamLuso.WebAPI.Middleware;
using DreamLuso.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// ============================================
// CONFIGURATION
// ============================================
// Configure port for Render (only override if PORT env var is set, otherwise use launchSettings.json)
var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrEmpty(port))
{
    builder.WebHost.UseUrls($"http://+:{port}");
}

// ============================================
// SERVICES - API Documentation
// ============================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // Schema configuration
    c.CustomSchemaIds(type => type.FullName?.Replace("+", ".") ?? type.Name);
    c.IgnoreObsoleteProperties();
    c.IgnoreObsoleteActions();
    c.UseAllOfToExtendReferenceSchemas();
    c.SupportNonNullableReferenceTypes();
    c.DescribeAllParametersInCamelCase();
    
    // Handle potential errors gracefully
    try
    {
        // Include XML comments if available
        var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath))
        {
            c.IncludeXmlComments(xmlPath);
        }
    }
    catch
    {
        // Ignore XML comments errors
    }
});

// ============================================
// SERVICES - Controllers & JSON
// ============================================
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Configure enum serialization for controllers - accept enum names as strings (PascalCase)
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter(null, allowIntegerValues: false));
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });

builder.Services.ConfigureHttpJsonOptions(options =>
{
    // Configure enum serialization for minimal APIs - accept enum names as strings (PascalCase by default)
    // This allows enums to be sent as "RealEstateAgent", "Client", "Admin" (PascalCase)
    // null naming policy means use the exact enum name (PascalCase)
    options.SerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter(null, allowIntegerValues: false));
    options.SerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
});

// ============================================
// SERVICES - CORS
// ============================================
var corsOriginsEnv = Environment.GetEnvironmentVariable("CORS_ALLOWED_ORIGINS");
var configOrigins = builder.Configuration.GetSection("CorsSettings:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();

var defaultOrigins = new[]
{
    "http://localhost:4200",
    "http://localhost:3000",
    "https://dream-luso.vercel.app",
    "https://dreamluso-api.onrender.com",
    "https://dreamluso-frontend.onrender.com",
    "https://lifelong-jamal-scarless.ngrok-free.dev"
};

var allowedOrigins = !string.IsNullOrEmpty(corsOriginsEnv)
    ? corsOriginsEnv.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
    : configOrigins.Length > 0 ? configOrigins : defaultOrigins;

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
              .SetPreflightMaxAge(TimeSpan.FromSeconds(86400));
    });

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

// ============================================
// SERVICES - Application Services
// ============================================
builder.Services.AddIoCServices(builder.Configuration, builder.Environment);
builder.Services.AddSecurityServices(builder.Configuration);

// ============================================
// SERVICES - Authentication & Authorization
// ============================================
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
});
builder.Services.AddAuthorization();

// ============================================
// SERVICES - Additional Services
// ============================================
builder.Services.AddHealthChecks();
builder.Services.AddHttpContextAccessor();

// ============================================
// BUILD APPLICATION
// ============================================
var app = builder.Build();

// ============================================
// MIDDLEWARE PIPELINE
// ============================================

// Logging
var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("CORS Allowed Origins: {Origins}", string.Join(", ", allowedOrigins));
logger.LogInformation("Application starting in {Environment} environment", app.Environment.EnvironmentName);

// Swagger (only in Development)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(c =>
    {
        c.RouteTemplate = "swagger/{documentName}/swagger.json";
    });
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "DreamLuso API v1");
        c.RoutePrefix = "swagger";
        c.DisplayRequestDuration();
        c.EnableTryItOutByDefault();
        c.EnableDeepLinking();
        c.EnableFilter();
    });
}

// CORS
if (app.Environment.IsDevelopment())
{
    app.UseCors();
}
else
{
    app.UseCors("AllowAngularApp");
}

// Exception Handling
app.UseMiddleware<GlobalExceptionMiddleware>();

// Static Files
app.UseStaticFiles();

// HTTPS Redirection - Only in production
if (app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// ============================================
// ENDPOINTS
// ============================================
app.RegisterEndpoints();
app.MapHealthChecks("/health");
app.MapControllers();

// ============================================
// RUN APPLICATION
// ============================================
logger.LogInformation("Application started successfully");
app.Run();
