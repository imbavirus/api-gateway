namespace ApiGateway.Models.Table;

public class ApiRequest
{
    public int StatusCode { get; set; }
    public required string Endpoint { get; set; }
    public DateTime Time { get; set; }
}
