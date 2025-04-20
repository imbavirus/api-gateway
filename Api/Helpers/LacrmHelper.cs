using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using ApiGateway.Hubs;
using ApiGateway.Models.Exceptions;
using ApiGateway.Models.Table;
using ApiGateway.Managers;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.WebUtilities;

namespace ApiGateway.Helpers;

public class LacrmHelper
{
    private readonly HttpClient _httpClient;
    private readonly string _lacrmApiUrl;
    private readonly string _lacrmApiKey;
    private readonly IHubContext<DataHub> _hubContext;
    private readonly InMemoryDataStoreManager _dataStore;
    

    public LacrmHelper(
        HttpClient httpClient,
        IConfiguration configuration,
        IHubContext<DataHub> hubContext,
        InMemoryDataStoreManager dataStore
        )
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _lacrmApiUrl = configuration["Lacrm:BaseUrl"] ?? throw new ConfigurationException("LACRM API URL is not configured.");
        _lacrmApiKey = configuration["LACRM_API_KEY"] ?? throw new ConfigurationException("LACRM API Key is not configured.");
        _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
        _dataStore = dataStore ?? throw new ArgumentNullException(nameof(dataStore));
    }

    /// <summary>
    /// Calls the Less Annoying CRM API.
    /// </summary>
    /// <param name="functionName">The API function to call (e.g., "SearchContacts").</param>
    /// <param name="parameters">A dictionary of parameters for the API function.</param>
    /// <returns>A JsonElement representing the API response on success, or null on failure.</returns>
    public async Task<JsonElement?> CallLacrmApiAsync(string functionName, Dictionary<string, object>? data = null)
    {
        if (_lacrmApiKey == "YOUR_API_KEY_HERE")
        {
            throw new ConfigurationException("LACRM API Key is not configured.");
        }

        var queryParams = new Dictionary<string, string?>
        {
            { "APIToken", _lacrmApiKey }
        };

        string requestUrl = QueryHelpers.AddQueryString(_lacrmApiUrl, queryParams);

        var requestBody = new
        {
            Function = functionName,
            Parameters = data
        };

        // Serialize request body to JSON
        string jsonRequestBody = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");

        // Create the HttpRequestMessage
        using var request = new HttpRequestMessage(HttpMethod.Post, requestUrl);        
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        request.Content = content;

        // Send the request
        using var response = await _httpClient.SendAsync(request);

        // Read the response content
        string responseContent = await response.Content.ReadAsStringAsync();
        var apiRequest = new ApiRequest
        {
            StatusCode = (int)response.StatusCode,
            Endpoint = functionName,
            Time = DateTime.UtcNow,
        };
        _dataStore.AddData(apiRequest);      
        await _hubContext.Clients.All.SendAsync("ReceiveNewData", apiRequest);

        if (response.IsSuccessStatusCode)
        {
            // Deserialize successful response
            using var jsonDoc = JsonDocument.Parse(responseContent);
            return jsonDoc.RootElement.Clone();
        }
        else
        {        
            throw new LacrmException(
                $"LACRM API request failed. Response: {responseContent}",
                response.StatusCode
            );
        }
    }
}
