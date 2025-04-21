namespace ApiGateway.Models.Lacrm.User;

public class User
{
    public required string UserId { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Timezone { get; set; }
    public required string Email { get; set; }
}
