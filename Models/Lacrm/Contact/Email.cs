namespace ApiGateway.Models.Lacrm.Contact;

public class Email
{
    public string? Text { get; set; }
    public string? Type { get; set; }
    
    public Dictionary<string, string?> ToDictionary()
    {        
        var dictionary = new Dictionary<string, string?>();
        if (Text != null) dictionary.Add("Text", Text);
        if (Type != null) dictionary.Add("Type", Type);
        return dictionary;
    }

}