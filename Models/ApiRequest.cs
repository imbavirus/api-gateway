namespace ApiGateway.Models
{
    public class ApiRequest
    {
        public int StatusCode { get; set; }
        public string Endpoint { get; set; }
        public DateTime Time { get; set; }
    }
}
