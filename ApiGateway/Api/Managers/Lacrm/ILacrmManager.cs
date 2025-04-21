using ApiGateway.Models.Lacrm.Contact;
using ApiGateway.Models.Lacrm.Note;
using ApiGateway.Models.Lacrm.User;

namespace ApiGateway.Api.Managers.Lacrm;

/// <summary>
/// Defines the contract for managing LACRM specific operations.
/// </summary>
public interface ILacrmManager
{
    /// <summary>
    /// Retrieves the current user information from LACRM.
    /// </summary>
    /// <returns>A Task representing the asynchronous operation, containing the User details.</returns>
    Task<User> GetUser();

    /// <summary>
    /// Creates a new contact in LACRM.
    /// </summary>
    /// <param name="contact">The contact details to create.</param>
    /// <returns>A Task representing the asynchronous operation, containing the response from LACRM (e.g., the new ContactId).</returns>
    Task<CreateContactResponse> CreateContact(Contact contact);

    /// <summary>
    /// Edits an existing contact in LACRM.
    /// </summary>
    /// <param name="contact">The contact details to update. Ensure ContactId is populated.</param>
    /// <returns>A Task representing the asynchronous operation, containing the response from LACRM.</returns>
    Task<bool> EditContactAsync(Contact contact);

    /// <summary>
    /// Deletes a contact from LACRM.
    /// </summary>
    /// <param name="contactId">The ID of the contact to delete.</param>
    /// <returns>A Task representing the asynchronous operation, containing the response from LACRM.</returns>
    Task<bool> DeleteContactAsync(string contactId);

    /// <summary>
    /// Retrieves contacts from LACRM based on a search term.
    /// </summary>
    /// <param name="searchTerm">The term to search for in contact names.</param>
    /// <exception cref="LacrmException">Thrown when the API call fails.</exception>
    /// <returns>A list of contacts matching the search term.</returns>
    Task<IEnumerable<ContactResponse>> GetContactsAsync(string searchTerm);

    /// <summary>
    /// Creates a new note in LACRM.
    /// </summary>
    /// <param name="note">The note details to create.</param>
    /// <returns>A Task representing the asynchronous operation, containing the response from LACRM (e.g., the new NoteId).</returns>
    Task<NoteResponse> CreateNoteAsync(NoteData note);
}
