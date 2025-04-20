namespace ApiGateway.Models.Lacrm.Contact;

public class Website
{
    public string? Text { get; set; }
    
    public Dictionary<string, string?> ToDictionary()
    {        
        var dictionary = new Dictionary<string, string?>();
        if (Text != null) dictionary.Add("Text", Text);
        return dictionary;
    }
}