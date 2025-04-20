using System.Text.Json.Serialization;
using ApiGateway.Api.Helpers;
using ApiGateway.Models.Lacrm.Contact;

namespace ApiGateway.Models.TelephonyServer;

public class CallRequest
{
    public required string EventName { get; set; }
    [JsonConverter(typeof(DateTimeHelper))]
    public DateTime CallStart { get; set; }
    public Guid CallId { get; set; }
    public required string CallersName { get; set; }
    public required string CallersTelephoneNumber { get; set; }

    
    /// <summary>
    /// Creates a basic LACRM Contact object from the call request details.
    /// </summary>
    /// <returns>A new Contact object populated with caller's name and phone number.</returns>
    public Contact ToContact(string userId)
    {
        var contact = new Contact
        {
            Name = CallersName,
            // Assuming the incoming number should be added as a 'Work' phone
            Phone =
            [
                new() { Text = CallersTelephoneNumber, Type = "Work" }
            ],
            IsCompany = false,
            AssignedTo = long.Parse(userId),
        };

        return contact;
    }

}
