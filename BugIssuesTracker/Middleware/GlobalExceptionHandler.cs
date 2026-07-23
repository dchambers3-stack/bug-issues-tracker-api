using System.Net;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace BugTracker.Api.Middleware;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken ct
    )
    {
        var (statusCode, title) = exception switch
        {
            HttpRequestException httpEx when httpEx.StatusCode.HasValue =>
                ((int)httpEx.StatusCode.Value, httpEx.Message),
            ValidationException => ((int)HttpStatusCode.BadRequest, exception.Message),
            UnauthorizedAccessException => ((int)HttpStatusCode.Forbidden, exception.Message),
            KeyNotFoundException => ((int)HttpStatusCode.NotFound, exception.Message),
            _ => ((int)HttpStatusCode.InternalServerError, "An unexpected error occurred."),
        };

        if (statusCode == (int)HttpStatusCode.InternalServerError)
            _logger.LogError(exception, "Unhandled exception processing request");

        httpContext.Response.StatusCode = statusCode;

        await httpContext.Response.WriteAsJsonAsync(
            new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Instance = httpContext.Request.Path,
            },
            ct
        );

        return true;
    }
}
