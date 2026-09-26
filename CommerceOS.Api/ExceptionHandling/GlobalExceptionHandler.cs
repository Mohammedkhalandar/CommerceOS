using CommerceOS.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CommerceOS.Api.ExceptionHandling;

public class GlobalExceptionHandler
    : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(
            exception,
            "Unhandled exception occurred.");

        var statusCode = exception switch
        {
            InventoryConcurrencyException
                => StatusCodes.Status409Conflict,

            KeyNotFoundException
                => StatusCodes.Status404NotFound,

            ArgumentException
                => StatusCodes.Status400BadRequest,

            InvalidOperationException
                => StatusCodes.Status409Conflict,

            _ => StatusCodes.Status500InternalServerError
        };

        var title = statusCode switch
        {
            StatusCodes.Status400BadRequest
                => "Bad Request",

            StatusCodes.Status404NotFound
                => "Resource Not Found",

            StatusCodes.Status409Conflict
                => "Conflict",

            _ => "Internal Server Error"
        };

        var detail = statusCode == StatusCodes.Status500InternalServerError
            ? "An unexpected error occurred."
            : exception.Message;

        httpContext.Response.StatusCode = statusCode;

        await httpContext.Response.WriteAsJsonAsync(
            new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = detail,
                Instance = httpContext.Request.Path
            },
            cancellationToken);

        return true;
    }
}