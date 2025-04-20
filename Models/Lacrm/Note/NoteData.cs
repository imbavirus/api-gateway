namespace ApiGateway.Models.Lacrm.Note;

public class NoteData
{
    public required string ContactId { get; set; }
    public required string Note { get; set; }
    public DateTime DateDisplayedInHistory { get; set; }

    public Dictionary<string, object?>? ToDictionary()
    {
        Dictionary<string, object?>? dictionary = new Dictionary<string, object?>();
        if (ContactId != null) dictionary.Add("ContactId", ContactId);
        if (Note != null) dictionary.Add("Note", Note);
        if (DateDisplayedInHistory != DateTime.MinValue) dictionary.Add("DateDisplayedInHistory", DateDisplayedInHistory.ToString("yyyy-MM-dd HH:mm:ss"));
        return dictionary;
    }
}
