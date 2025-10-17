using DreamLuso.Application.Common.Exceptions;
using DreamLuso.Application.Common.Responses;
using System.Net;
using System.Text.Json;

namespace DreamLuso.WebAPI.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger, IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ocorreu um erro não tratado: {Message}. StackTrace: {StackTrace}", 
                ex.Message, ex.StackTrace);
            await HandleExceptionAsync(context, ex, _environment);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception, IHostEnvironment environment)
    {
        context.Response.ContentType = "application/json";

        object response;
        
        if (exception is ApplicationValidationException validationException)
        {
            response = new
            {
                statusCode = (int)HttpStatusCode.BadRequest,
                message = "Erros de validação",
                errors = validationException.Errors
            };
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        }
        else if (exception is NotFoundException notFoundException)
        {
            response = new
            {
                statusCode = (int)HttpStatusCode.NotFound,
                message = notFoundException.Message
            };
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
        }
        else if (exception is DomainException domainException)
        {
            response = new
            {
                statusCode = (int)HttpStatusCode.BadRequest,
                message = domainException.Message
            };
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        }
        else
        {
            var errorDetails = new
            {
                statusCode = (int)HttpStatusCode.InternalServerError,
                message = "Ocorreu um erro interno no servidor",
                // Include detailed error in Production for debugging (remove later)
                detail = exception.Message,
                stackTrace = environment.IsProduction() ? exception.StackTrace : null
            };
            
            response = errorDetails;
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        }

        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}

