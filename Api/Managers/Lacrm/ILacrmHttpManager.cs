using System.Text.Json;

namespace ApiGateway.Managers.Lacrm;

/// <summary>
/// Defines the contract for interacting with the Less Annoying CRM API.
/// </summary>
public interface ILacrmHttpManager
{
    /// <summary>
    /// Calls the Less Annoying CRM API.
    /// </summary>
    /// <param name="functionName">The API function to call (e.g., "SearchContacts").</param>
    /// <param name="data">A dictionary of parameters for the API function.</param>
    /// <returns>A object representing the API response on success, or null on failure.</returns>
    Task<object?> CallLacrmApiAsync(string functionName, Dictionary<string, object>? data = null);
}
