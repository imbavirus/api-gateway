using ApiGateway.Helpers;
using ApiGateway.Models.TelephonyServer;
using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Services;

/// <summary>
/// Implements the logic for processing phone call events by interacting with LACRM.
/// </summary>
public class PhoneCallService : IPhoneCallService
{
    private readonly LacrmHelper _lacrmHelper; // The "manager" for LACRM interactions
    private readonly ILogger<PhoneCallService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="PhoneCallService"/> class.
    /// </summary>
    /// <param name="lacrmHelper">The injected LACRM helper instance.</param>
    /// <param name="logger">The injected logger instance.</param>
    public PhoneCallService(LacrmHelper lacrmHelper, ILogger<PhoneCallService> logger)
    {
        _lacrmHelper = lacrmHelper ?? throw new ArgumentNullException(nameof(lacrmHelper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
            var searchResult = await _lacrmHelper.CallLacrmApiAsync("GetUser");
            return Helper.StringifyObject(searchResult);
    }
}