
public class ErrorResponse
{
    public string Message { get; set; }
    public bool HasError = true;
    public string Type { get; set; }
    public object? Payload { get; set; } = null;
    
    public ErrorResponse(string message, string type, object? payload = null)
    {
        Message = message;
        Type = type;
        Payload = payload;
    }
}