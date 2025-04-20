using System;
using System.Net;

namespace ApiGateway.Models.Exceptions;

/// <summary>
/// Exception thrown when there is an issue with the configuration.
/// </summary>
public class LacrmException : System.Exception
{
    public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.InternalServerError;
    public LacrmException(string message, HttpStatusCode statusCode) : base(message)
    { 
        StatusCode = statusCode;
    }
}
