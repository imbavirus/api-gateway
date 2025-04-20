using ApiGateway.Models.Lacrm.User;
using ApiGateway.Models.Exceptions;
using ApiGateway.Models.Lacrm.Contact;
using ApiGateway.Models.Lacrm.Note;

namespace ApiGateway.Api.Managers.Lacrm.Implementation;

public class LacrmManager : ILacrmManager
{
    private readonly ILacrmHttpManager _lacrmHttpManager;

    public LacrmManager(ILacrmHttpManager lacrmHttpManager)
    {
        _lacrmHttpManager = lacrmHttpManager;
    }

    public async Task<User> GetUser()
    {
        return await _lacrmHttpManager.CallLacrmApiAsync<User?>("GetUser") ?? throw new LacrmException("Failed to retrieve user data from LACRM.", System.Net.HttpStatusCode.Unauthorized);
    }

    public async Task<CreateContactResponse> CreateContact(Contact contact)
    {
        var data = contact.ToDictionary();
        return await _lacrmHttpManager.CallLacrmApiAsync<CreateContactResponse?>("CreateContact", data) ?? throw new LacrmException("Failed to create contact on LACRM.", System.Net.HttpStatusCode.InternalServerError);
    }

    public async Task<bool> EditContactAsync(Contact contact)
    {
        var data = contact.ToDictionary();
        _ = await _lacrmHttpManager.CallLacrmApiAsync<object?>("EditContact", data) ?? throw new LacrmException("Failed to edit contact on LACRM.", System.Net.HttpStatusCode.InternalServerError);
        return true;
    }
    public async Task<bool> DeleteContactAsync(string contactId)
    {
        var data = new Dictionary<string, object?> { { "ContactID", contactId } };
        _ = await _lacrmHttpManager.CallLacrmApiAsync<object?>("DeleteContact", data) ?? throw new LacrmException("Failed to delete contact on LACRM.", System.Net.HttpStatusCode.InternalServerError);
        return true;
    }

    public async Task<IEnumerable<ContactResponse>> GetContactsAsync(string searchTerm)
    {
        var data = new Dictionary<string, object?> { { "SearchTerm", searchTerm } };
        GetContactsResponse response = await _lacrmHttpManager.CallLacrmApiAsync<GetContactsResponse>("GetContacts", data) ?? throw new LacrmException("Failed to retrieve contacts from LACRM.", System.Net.HttpStatusCode.InternalServerError);
        return response?.Results ?? [];
    }
    public async Task<NoteResponse> CreateNoteAsync(NoteData note)
    {
        var data = note.ToDictionary();
        return await _lacrmHttpManager.CallLacrmApiAsync<NoteResponse?>("CreateNote", data) ?? throw new LacrmException("Failed to create note on LACRM.", System.Net.HttpStatusCode.InternalServerError);
    }

}
