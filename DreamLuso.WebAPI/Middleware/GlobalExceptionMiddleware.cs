using DreamLuso.Application.Common.Exceptions;
using DreamLuso.Application.Common.Responses;
using System.Net;
using System.Text.Json;

namespace DreamLuso.WebAPI.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ocorreu um erro não tratado");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
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
            response = new
            {
                statusCode = (int)HttpStatusCode.InternalServerError,
                message = "Ocorreu um erro interno no servidor"
            };
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        }

        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}

