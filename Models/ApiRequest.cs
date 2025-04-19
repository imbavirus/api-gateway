namespace ApiGateway.Models
{
    public class ApiRequest
    {
        public int StatusCode { get; set; }
        public string Request { get; set; }
        public DateTime Time { get; set; }
    }
}
