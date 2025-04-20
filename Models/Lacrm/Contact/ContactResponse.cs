namespace ApiGateway.Models.Lacrm.Contact;

public class ContactResponse
{
    public string? ContactId { get; set; }
    public bool? IsCompany { get; set; }
    public long? AssignedTo { get; set; }
    public ContactName? Name { get; set; }
    public List<Email>? Email { get; set; }
    public List<Phone>? Phone { get; set; }
    public string? CompanyName { get; set; }
    public string? JobTitle { get; set; }
    public List<Address>? Address { get; set; }
    public string? BackgroundInfo { get; set; }
    public List<Website>? Website { get; set; }
    public string? Birthday { get; set; }

    public Dictionary<string, object?> ToDictionary()
    {
        var dictionary = new Dictionary<string, object?>();
        if (ContactId != null) dictionary.Add("ContactId", ContactId);
        if (IsCompany.HasValue) dictionary.Add("IsCompany", IsCompany);
        if (AssignedTo != null) dictionary.Add("AssignedTo", AssignedTo);
        if (Name != null) dictionary.Add("Name", Name.ToDictionary());
        if (CompanyName != null) dictionary.Add("CompanyName", CompanyName);
        if (JobTitle != null) dictionary.Add("JobTitle", JobTitle);
        if (BackgroundInfo != null) dictionary.Add("BackgroundInfo", BackgroundInfo);
        if (Birthday != null) dictionary.Add("Birthday", Birthday);
        if (Email != null) dictionary.Add("Email", Email.Select(e => e.ToDictionary()).ToList());
        if (Phone != null) dictionary.Add("Phone", Phone.Select(e => e.ToDictionary()).ToList());
        if (Address != null) dictionary.Add("Address", Address.Select(e => e.ToDictionary()).ToList());
        if (Website != null) dictionary.Add("Website", Website.Select(e => e.ToDictionary()).ToList());

        return dictionary;
    }
}
