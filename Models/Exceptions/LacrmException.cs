using System.Net;

namespace ApiGateway.Models.Exceptions;

/// <summary>
/// Exception thrown when there is an issue with the configuration.
/// </summary>
public class LacrmException(string message, HttpStatusCode statusCode) : Exception(message)
{
    public HttpStatusCode StatusCode { get; set; } = statusCode;
}
