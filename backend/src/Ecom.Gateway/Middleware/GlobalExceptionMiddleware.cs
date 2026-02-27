using System;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;

namespace Ecom.Gateway.Middleware
{
    /// <summary>
    /// Middleware that catches all unhandled exceptions, logs them and
    /// returns a structured JSON response to the client.
    /// </summary>
    public sealed class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;
        private readonly IWebHostEnvironment _environment;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger, IWebHostEnvironment environment)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex).ConfigureAwait(false);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var traceId = context.TraceIdentifier;
            _logger.LogError(exception, "Unhandled exception occurred. TraceId: {TraceId}", traceId);

            var statusCode = exception switch
            {
                UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
                KeyNotFoundException => StatusCodes.Status404NotFound,
                ValidationException => StatusCodes.Status400BadRequest,
                ArgumentException => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status500InternalServerError
            };

            var problem = new ProblemDetails
            {
                Type = $"https://httpstatuses.com/{statusCode}",
                Title = GetUserFacingMessage(exception, statusCode),
                Status = statusCode,
                Detail = _environment.IsDevelopment() ? exception.Message : null
            };

            // Add trace id and (in Development) full exception to extensions
            problem.Extensions["traceId"] = traceId;
            if (_environment.IsDevelopment())
            {
                problem.Extensions["exception"] = exception.ToString();
            }

            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = statusCode;

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            await context.Response.WriteAsync(JsonSerializer.Serialize(problem, options)).ConfigureAwait(false);
        }

        private static string GetUserFacingMessage(Exception exception, int statusCode)
        {
            if (statusCode == StatusCodes.Status500InternalServerError)
            {
                return "An unexpected error occurred. Please try again or contact support.";
            }

            // For client errors, return the exception message (assumed safe for user)
            return exception?.Message ?? "An error occurred.";
        }
    }

    /// <summary>
    /// Extension to register the global exception middleware in the pipeline.
    /// </summary>
    public static class ExceptionHandlingExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
        {
            return app.UseMiddleware<GlobalExceptionMiddleware>();
        }
    }

    [System.Obsolete("Kept for compatibility with hot-reload during development. Use ProblemDetails response instead.")]
    internal class ErrorResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? TraceId { get; set; }
        public string? Details { get; set; }
    }
}
