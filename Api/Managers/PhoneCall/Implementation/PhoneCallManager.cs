
using ApiGateway.Models.Lacrm.Contact;
using ApiGateway.Api.Managers.Lacrm;
using ApiGateway.Models.TelephonyServer;
using ApiGateway.Models.Exceptions;
using ApiGateway.Models.Lacrm.User;
using ApiGateway.Models.Lacrm.Note;

namespace ApiGateway.Api.Managers.PhoneCall.Implementation;

public class PhoneCallManager : IPhoneCallManager
{
    private readonly ILacrmManager _lacrmManager;

    public PhoneCallManager(ILacrmManager lacrmManager)
    {
        _lacrmManager = lacrmManager ?? throw new ArgumentNullException(nameof(lacrmManager));
    } 

    public async Task<string> ProcessIncomingCallAsync(CallRequest callRequest)
    {
        string result;
        string contactId;
        User user = await _lacrmManager.GetUser() ?? throw new LacrmException("User not found in LACRM.", System.Net.HttpStatusCode.Unauthorized);

        Contact contact = callRequest.ToContact(user.UserId) ?? throw new LacrmException("Could not generate contact from provided data.", System.Net.HttpStatusCode.BadRequest);

        IEnumerable<ContactResponse> existingContacts = await _lacrmManager.GetContactsAsync(callRequest.CallersTelephoneNumber);

        if (!existingContacts.Any(x => x.Phone?.Any(y => y.Text == callRequest.CallersTelephoneNumber) ?? false))
        {
            CreateContactResponse response = await _lacrmManager.CreateContact(contact);
            contactId = response.ContactId;
            result = "Contact created successfully!";
        }
        else
        {
            // If the contact already exists, we need to update it with the new information. We assume phone number is unique.
            contactId = existingContacts.Where(x => x.Phone?.Any(y => y.Text == callRequest.CallersTelephoneNumber) ?? false).FirstOrDefault()?.ContactId ?? string.Empty;
            if (string.IsNullOrEmpty(contactId))
            {
                throw new LacrmException("Contact ID not found on existing contact.", System.Net.HttpStatusCode.InternalServerError);
            }
            result = "Contact already exists!";
        }

        // Add note to contact to show a call happened
        NoteData note = new NoteData
        {
            ContactId = contactId,
            Note = $"Incoming call from {callRequest.CallersName} ({callRequest.CallersTelephoneNumber})",
            DateDisplayedInHistory = callRequest.CallStart,
        };
        await _lacrmManager.CreateNoteAsync(note);

        return result;
    }
}