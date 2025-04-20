namespace ApiGateway.Models.Lacrm.Contact;

public class ContactName
{
    public string? Salutation { get; set; }
    public string? FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string? LastName { get; set; }
    public string? Suffix { get; set; }

    public Dictionary<string, object?> ToDictionary()
    {
        var dictionary = new Dictionary<string, object?>();
        if (Salutation != null) dictionary.Add("Salutation", Salutation);
        if (FirstName != null) dictionary.Add("FirstName", FirstName);
        if (MiddleName != null) dictionary.Add("MiddleName", MiddleName);
        if (LastName != null) dictionary.Add("LastName", LastName);
        if (Suffix != null) dictionary.Add("Suffix", Suffix);

        return dictionary;
    }
}
