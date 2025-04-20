using ApiGateway.Models.TelephonyServer;
using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Services;

/// <summary>
/// Defines the contract for processing phone call events.
/// </summary>
public interface IPhoneCallService
{
    /// <summary>
    /// Processes an incoming phone call event.
    /// </summary>
    /// <param name="callRequest">The details of the incoming call.</param>
    /// <returns>A result indicating success or failure, potentially with additional data.</returns>
    Task<string> ProcessIncomingCallAsync(CallRequest callRequest);
}