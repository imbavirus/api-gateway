namespace ApiGateway.Models.TelephonyServer;

public class CallRequest
{
    public required string EventName { get; set; }
    public DateTime CallStart { get; set; }
    public Guid CallId { get; set; }
    public required string CallersName { get; set; }
    public required string CallersTelephoneNumber { get; set; }
}
