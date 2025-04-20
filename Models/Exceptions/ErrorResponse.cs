namespace ApiGateway.Models.Exceptions;

public class ErrorResponse(string message, string type, object? payload = null)
{
    public string Message { get; set; } = message;
    public bool HasError = true;
    public string Type { get; set; } = type;
    public object? Payload { get; set; } = payload;
}