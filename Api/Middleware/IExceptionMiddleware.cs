namespace ApiGateway.Middleware;

/// <summary>
/// Defines the contract for handling a request within the middleware pipeline.
/// Note: This interface is not directly used by the standard ASP.NET Core
/// UseMiddleware<T> extension method, which relies on convention (InvokeAsync method).
/// </summary>
public interface IExceptionMiddleware
{
    /// <summary>
    /// Invokes the middleware logic for the given HTTP context.
    /// </summary>
    /// <param name="context">The HTTP context for the request.</param>
    /// <returns>A task that represents the completion of request processing.</returns>
    Task InvokeAsync(HttpContext context);
}
