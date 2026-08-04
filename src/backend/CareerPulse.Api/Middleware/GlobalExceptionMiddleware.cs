using CareerPulse.Domain.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace CareerPulse.Api.Middleware;

/// <summary>
/// Global exception handler middleware.
/// ADR 009: Maps domain exceptions to RFC 7807 ProblemDetails responses.
/// DomainException  → HTTP 409 Conflict
/// ValidationException → HTTP 400 Bad Request
/// Unhandled        → HTTP 500 Internal Server Error
/// </summary>
public sealed class GlobalExceptionMiddleware
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
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Domain rule violation: {Message}", ex.Message);
            await WriteProblemAsync(context,
                StatusCodes.Status409Conflict,
                "https://careerpulse.local/errors/domain-rule-violation",
                "Domain Rule Violation",
                ex.Message);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning("Validation failed for request to {Path}", context.Request.Path);

            var errors = ex.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray());

            await WriteProblemWithErrorsAsync(context,
                StatusCodes.Status400BadRequest,
                "https://careerpulse.local/errors/validation-failed",
                "Validation Failed",
                "One or more validation errors occurred.",
                errors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception on request {Path}", context.Request.Path);
            await WriteProblemAsync(context,
                StatusCodes.Status500InternalServerError,
                "https://tools.ietf.org/html/rfc7807",
                "Internal Server Error",
                "An unexpected error occurred. Please try again later.");
        }
    }

    private static async Task WriteProblemAsync(
        HttpContext context,
        int statusCode,
        string type,
        string title,
        string detail)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";

        var problem = new ProblemDetails
        {
            Type = type,
            Title = title,
            Status = statusCode,
            Detail = detail,
            Instance = context.Request.Path
        };

        await context.Response.WriteAsJsonAsync(problem);
    }

    private static async Task WriteProblemWithErrorsAsync(
        HttpContext context,
        int statusCode,
        string type,
        string title,
        string detail,
        Dictionary<string, string[]> errors)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";

        var problem = new ProblemDetails
        {
            Type = type,
            Title = title,
            Status = statusCode,
            Detail = detail,
            Instance = context.Request.Path
        };
        problem.Extensions["errors"] = errors;

        await context.Response.WriteAsJsonAsync(problem);
    }
}
