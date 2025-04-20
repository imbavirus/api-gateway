namespace ApiGateway.Models.Lacrm.Contact;

public class GetContactsResponse
{
    public bool HasMoreResults { get; set; }
    public required List<ContactResponse> Results { get; set; }
}
