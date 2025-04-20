using ApiGateway.Models.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using ApiGateway.Helpers;
using System.Text.Json;

namespace ApiGateway.Middleware;

/// <summary>
/// Middleware to handle exceptions globally, log them, and return a standardized error response.
/// </summary>
public class ExceptionMiddleware : IExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<IExceptionMiddleware> _logger;
    private readonly IHostEnvironment _env;

    /// <summary>
    /// Initializes a new instance of the <see cref="IExceptionMiddleware"/> class.
    /// </summary>
    /// <param name="next">The next middleware in the pipeline.</param>
    /// <param name="logger">The logger instance.</param>
    /// <param name="env">The hosting environment instance.</param>
    public ExceptionMiddleware(
        RequestDelegate next,
        ILogger<IExceptionMiddleware> logger,
        IHostEnvironment env
        )
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _env = env ?? throw new ArgumentNullException(nameof(env));
    }

    /// <summary>
    /// Invokes the middleware to handle the request and catch exceptions.
    /// </summary>
    /// <param name="context">The HTTP context for the request.</param>
    public async Task InvokeAsync(HttpContext context)
    {
        // Get this exception details from context.
        var exceptionDetails = context.Features.Get<IExceptionHandlerFeature>();

        // Helper.PrintObject(context.Features);

        // Do nothing if there isn't exception details.
        if (exceptionDetails == null)
        {
            return;
        }

        // Get the exception from details
        var exception = exceptionDetails?.Error;

        // Handle the exception based on its type
        switch (exception)
        {
            case ConfigurationException configEx:
                _logger.LogError(configEx, "A configuration error occurred. Request Path: {Path}", context.Request.Path);
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsync(Helper.StringifyObject(new ErrorResponse(configEx.Message, "Configuration error")));
                break;
            case ArgumentNullException argNullEx:
                _logger.LogError(argNullEx, "An argument null error occurred. Request Path: {Path}", context.Request.Path);
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync(Helper.StringifyObject(new ErrorResponse(argNullEx.Message, "Argument null error")));
                break;
            case HttpRequestException httpRequestEx:
                _logger.LogError(httpRequestEx, "An HTTP request error occurred. Request Path: {Path}", context.Request.Path);
                context.Response.StatusCode = httpRequestEx.StatusCode == null ? StatusCodes.Status500InternalServerError : (int)httpRequestEx.StatusCode;
                await context.Response.WriteAsync(Helper.StringifyObject(new ErrorResponse(httpRequestEx.Message, "HTTP request error")));
                break;
            case LacrmException lacrmEx:
                _logger.LogError(lacrmEx, "A LACRM API error occurred. Request Path: {Path}", context.Request.Path);
                context.Response.StatusCode = (int)lacrmEx.StatusCode;
                await context.Response.WriteAsync(Helper.StringifyObject(new ErrorResponse(lacrmEx.Message, "LACRM API error")));
                break;
            case JsonException jsonEx:
                _logger.LogError(jsonEx, "A JSON serialization error occurred. Request Path: {Path}", context.Request.Path);
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync(Helper.StringifyObject(new ErrorResponse(jsonEx.Message, "JSON serialization error")));
                break;
            default:
                _logger.LogError(exception, "An unhandled exception occurred while processing the request. Request Path: {Path}", context.Request.Path);
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsync(Helper.StringifyObject(new ErrorResponse(exception?.Message ?? "An unhandled exception occurred.", "Inactive object")));
                break;
        }
        return;
    }
}
