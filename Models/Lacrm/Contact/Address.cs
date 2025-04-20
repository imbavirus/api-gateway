namespace ApiGateway.Models.Lacrm.Contact;

public class Address
{
    public string? Street { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Zip { get; set; }
    public string? Country { get; set; }
    public string? Type { get ; set; }

    public Dictionary<string, string?> ToDictionary()
    {
        var dictionary = new Dictionary<string, string?>();
        if (Street != null) dictionary.Add("Street", Street);
        if (City != null) dictionary.Add("City", City);
        if (State != null) dictionary.Add("State", State);
        if (Zip != null) dictionary.Add("Zip", Zip);
        if (Country != null) dictionary.Add("Country", Country);
        if (Type != null) dictionary.Add("Type", Type);
        return dictionary;
    }
}