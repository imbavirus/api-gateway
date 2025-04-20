using ApiGateway.Models.TelephonyServer;
using ApiGateway.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers.API;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class PhoneCallController : Controller
{
    private readonly IPhoneCallService _phoneCallService;

    public PhoneCallController(IPhoneCallService phoneCallService)
    {
        _phoneCallService = phoneCallService;
    }
    /// <summary>
    /// Handles HTTP POST requests to process and return the received data.
    /// </summary>
    /// <param name="data">The data sent in the request body.</param>
    /// <returns>
    /// An <see cref="IActionResult"/> containing a success message and the received data.
    /// </returns>
    /// <remarks>
    /// Ensure that the action method is properly annotated with [HttpPost] and that the 
    /// <c>ApiExplorerSettings</c> attribute is not set to ignore this action. Additionally, 
    /// verify that the Swagger generator (e.g., Swashbuckle) is configured to include this 
    /// controller and action in the API documentation.
    /// </remarks>
    [HttpPost]
    public async Task<IActionResult> PostDataAsync([FromBody] CallRequest data)
    {
        string result = await _phoneCallService.ProcessIncomingCallAsync(data);
        // Process the data here
        return Ok(new { message = "Data received successfully", receivedData = result });
    }
};