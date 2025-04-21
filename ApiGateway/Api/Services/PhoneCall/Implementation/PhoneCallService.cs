using ApiGateway.Api.Helpers;
using ApiGateway.Models.TelephonyServer;
using Microsoft.AspNetCore.Mvc;
using ApiGateway.Api.Managers.PhoneCall;

namespace ApiGateway.Api.Services.PhoneCall.Implementation;

/// <summary>
/// Implements the logic for processing phone call events by interacting with LACRM.
/// </summary>
public class PhoneCallService : IPhoneCallService
{
    private readonly IPhoneCallManager _phoneCallManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="PhoneCallService"/> class.
    /// </summary>
    /// <param name="phoneCallManager">The injected LACRM helper instance.</param>
    /// <param name="logger">The injected logger instance.</param>
    public PhoneCallService(IPhoneCallManager phoneCallManager)
    {
        _phoneCallManager = phoneCallManager ?? throw new ArgumentNullException(nameof(phoneCallManager));
    }

    /// <summary>
    /// Processes an incoming phone call event. It searches for the contact in LACRM
    /// and potentially logs the call activity (implementation detail).
    /// </summary>
    /// <param name="callRequest">The details of the incoming call.</param>
    /// <returns>An <see cref="ActionResult"/> indicating the outcome.</returns>
    public async Task<string> ProcessIncomingCallAsync(CallRequest callRequest)
    {
            // Call the manager method
            string message = await _phoneCallManager.ProcessIncomingCallAsync(callRequest);
            return message;
    }
}