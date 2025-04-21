using ApiGateway.Models.TelephonyServer;

namespace ApiGateway.Api.Managers.PhoneCall;

/// <summary>
/// Defines the contract for managing phone call related operations.
/// </summary>
public interface IPhoneCallManager
{
    /// <summary>
    /// Processes an incoming phone call request, potentially creating a contact in the CRM.
    /// </summary>
    /// <param name="callRequest">The details of the incoming call.</param>
    /// <returns>A Task representing the asynchronous operation, returning true if processing was successful (e.g., contact created), false otherwise.</returns>
    Task<string> ProcessIncomingCallAsync(CallRequest callRequest);
}
