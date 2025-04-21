using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using ApiGateway.Api.Hubs;
using ApiGateway.Api.Managers.Data;
using ApiGateway.Models.Exceptions;
using ApiGateway.Models.Table;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.WebUtilities;

namespace ApiGateway.Api.Managers.Lacrm.Implementation;

public class LacrmHttpManager : ILacrmHttpManager
{
    private readonly HttpClient _httpClient;
    private readonly string _lacrmApiUrl;
    private readonly string _lacrmApiKey;
    private readonly IHubContext<DataHub> _hubContext;
    private readonly IInMemoryDataStoreManager _dataStore;
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    

    public LacrmHttpManager(
        HttpClient httpClient,
        IConfiguration configuration,
        IHubContext<DataHub> hubContext,
        IInMemoryDataStoreManager dataStore
        )
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _lacrmApiUrl = configuration["Lacrm:BaseUrl"] ?? throw new ConfigurationException("LACRM API URL is not configured.");
        _lacrmApiKey = configuration["LACRM_API_KEY"] ?? throw new ConfigurationException("LACRM API Key is not configured.");
        _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
        _dataStore = dataStore ?? throw new ArgumentNullException(nameof(dataStore));

        // Initialize and cache serializer options
        _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            ReferenceHandler = ReferenceHandler.Preserve
        };
    }

    /// <summary>
    /// Calls the Less Annoying CRM API and deserializes the response.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the successful JSON response into.</typeparam>
    /// <param name="functionName">The API function to call (e.g., "SearchContacts").</param>
    /// <param name="data">A dictionary of parameters for the API function.</param>
    /// <returns>A deserialized object of type T on success, or null/default if the response is null or empty JSON.</returns>
    /// <exception cref="LacrmException">Thrown if the API call fails (non-success status code) or deserialization fails.</exception>
    /// <exception cref="ConfigurationException">Thrown for configuration issues.</exception>
    /// <exception cref="HttpRequestException">Thrown for network issues during the HTTP call.</exception>
    public async Task<T?> CallLacrmApiAsync<T>(string functionName, Dictionary<string, object?>? data = null)
    {
        if (string.IsNullOrWhiteSpace(functionName))
        {
            throw new NullReferenceException("Function name cannot be null or empty.");
        }
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
        var request = new HttpRequestMessage(HttpMethod.Post, requestUrl);        
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
            var result = JsonSerializer.Deserialize<T?>(responseContent, _jsonSerializerOptions);
            return result;
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
