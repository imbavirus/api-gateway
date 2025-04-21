using System;
using System.Net.Http;
using Xunit;
using Moq;
using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using ApiGateway.Api.Managers.Lacrm;
using ApiGateway.Api.Hubs;
using ApiGateway.Models.Exceptions;
using ApiGateway.Models.Table;
using ApiGateway.Api.Managers.Data;
using Moq.Protected;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using ApiGateway.Api.Managers.Lacrm.Implementation;

// Define a simple DTO for testing deserialization
public class TestDto
{
    public string? Message { get; set; }
    public int Value { get; set; }
}

public class LacrmHttpManagerTests
{
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private readonly HttpClient _httpClient;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly Mock<IHubContext<DataHub>> _mockHubContext;
    private readonly Mock<IHubClients> _mockHubClients;
    private readonly Mock<IClientProxy> _mockClientProxy;
    private readonly Mock<IInMemoryDataStoreManager> _mockDataStore;
    private readonly ILacrmHttpManager _lacrmHttpManager;

    private const string TestApiUrl = "http://test.lacrm.com/api";
    private const string TestApiKey = "valid-api-key";
    private const string TestFunctionName = "TestFunction";

    public LacrmHttpManagerTests()
    {
        // Mock HttpMessageHandler for HttpClient
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri(TestApiUrl) // Base address isn't strictly used by SendAsync but good practice
        };

        // Mock IConfiguration
        _mockConfiguration = new Mock<IConfiguration>();
        _mockConfiguration.Setup(static c => c["Lacrm:BaseUrl"]).Returns(TestApiUrl);
        _mockConfiguration.Setup(static c => c["LACRM_API_KEY"]).Returns(TestApiKey);

        // Mock SignalR Hub Context
        _mockHubContext = new Mock<IHubContext<DataHub>>();
        _mockHubClients = new Mock<IHubClients>();
        _mockClientProxy = new Mock<IClientProxy>();
        _mockHubContext.Setup(static h => h.Clients).Returns(_mockHubClients.Object);
        _mockHubClients.Setup(static c => c.All).Returns(_mockClientProxy.Object);

        // Mock IInMemoryDataStoreManager
        _mockDataStore = new Mock<IInMemoryDataStoreManager>(); // Adjust if it takes constructor args

        // Create the instance of the class under test
        _lacrmHttpManager = new LacrmHttpManager(
            _httpClient,
            _mockConfiguration.Object,
            _mockHubContext.Object,
            _mockDataStore.Object
        );
    }

    // --- Helper Method to Setup HttpResponse ---
    private void SetupHttpResponse(HttpStatusCode statusCode, string? jsonContent)
    {
        HttpResponseMessage httpResponse = new HttpResponseMessage(statusCode);
        if (jsonContent != null)
        {
            httpResponse.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        }

        _mockHttpMessageHandler
            .Protected() // Use Protected() for SendAsync
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(httpResponse);
    }

    // --- Test Cases ---

    [Fact]
    public async Task CallLacrmApiAsync_Success_ReturnsDeserializedObjectAndLogsRequest()
    {
        // Arrange
        TestDto expectedDto = new() { Message = "Success", Value = 123 };
        string responseJson = JsonSerializer.Serialize(expectedDto);
        SetupHttpResponse(HttpStatusCode.OK, responseJson);

        Dictionary<string, object?> requestData = new Dictionary<string, object?> { { "Param1", "Value1" } };

        // Act
        TestDto? result = await _lacrmHttpManager.CallLacrmApiAsync<TestDto>(TestFunctionName, requestData);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedDto.Message, result.Message);
        Assert.Equal(expectedDto.Value, result.Value);

        // Verify HTTP Request details (optional but recommended)
        _mockHttpMessageHandler
            .Protected()
            .Verify(
                "SendAsync",
                Times.Once(), // Ensure it was called exactly once
                ItExpr.Is<HttpRequestMessage>(static req =>
                    req.Method == HttpMethod.Post &&
                    req.RequestUri!.ToString().StartsWith($"{TestApiUrl}?APIToken={TestApiKey}") && // Check URL and token
                    req.Headers.Accept.ToString() == "application/json" && // Check Accept header
                    req.Content!.Headers.ContentType!.ToString() == "application/json; charset=utf-8" // Check Content-Type
                    // Optionally, deserialize req.Content and check Function/Parameters
                ),
                ItExpr.IsAny<CancellationToken>()
            );

        // Verify DataStore logging
        _mockDataStore.Verify(static ds => ds.AddData(It.Is<ApiRequest>(static x =>
            x.StatusCode == (int)HttpStatusCode.OK &&
            x.Endpoint == TestFunctionName
        )), Times.Once);

        // Verify SignalR broadcast
        _mockClientProxy.Verify(static c => c.SendCoreAsync(
            "ReceiveNewData",
            It.Is<object[]>(static x => x.Length == 1 && ((ApiRequest)x[0]).StatusCode == (int)HttpStatusCode.OK),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CallLacrmApiAsync_Success_WithNullData_SendsNullParameters()
    {
        // Arrange
        TestDto expectedDto = new TestDto { Message = "Success", Value = 456 };
        string responseJson = JsonSerializer.Serialize(expectedDto);
        // SetupHttpResponse(HttpStatusCode.OK, responseJson); // Replace this simple setup

        HttpRequestMessage? capturedRequest = null; // Variable to capture the request

        // Setup SendAsync to capture the request and return the response
        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            // Capture the request when SendAsync is called
            .Callback<HttpRequestMessage, CancellationToken>((req, _) => capturedRequest = req)
            // Return the desired response
            .ReturnsAsync(() => {
                 HttpResponseMessage httpResponse = new HttpResponseMessage(HttpStatusCode.OK);
                 httpResponse.Content = new StringContent(responseJson, Encoding.UTF8, "application/json");
                 return httpResponse;
            });


        // Act
        TestDto? result = await _lacrmHttpManager.CallLacrmApiAsync<TestDto>(TestFunctionName, null); // Pass null data

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedDto.Message, result.Message);
        Assert.Equal(expectedDto.Value, result.Value);

        // Verify basic HTTP Request details (optional but good)
        // We remove the content check from this Verify call
        _mockHttpMessageHandler
            .Protected()
            .Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(static req =>
                    req.Method == HttpMethod.Post &&
                    req.RequestUri!.ToString().StartsWith($"{TestApiUrl}?APIToken={TestApiKey}") // Basic checks are fine here
                ),
                ItExpr.IsAny<CancellationToken>()
            );

        // Assert on the captured request's content AFTER the Act phase
        Assert.NotNull(capturedRequest); // Ensure the request was captured
        Assert.NotNull(capturedRequest.Content);
        // Now it's safe to read the content using your helper
        Assert.True(await ContentHasNullParameters(capturedRequest.Content), "Request body should have null Parameters");

         // Verify logging and SignalR (these should already be correct)
        _mockDataStore.Verify(static ds => ds.AddData(It.Is<ApiRequest>(x => x.StatusCode == (int)HttpStatusCode.OK)), Times.Once);
        _mockClientProxy.Verify(static c => c.SendCoreAsync("ReceiveNewData", It.Is<object[]>(x => x != null && x.Length > 0), It.IsAny<CancellationToken>()), Times.Once);
    }

    // Helper to check request body content
    private async Task<bool> ContentHasNullParameters(HttpContent? content)
    {
        if (content == null) return false;
        string jsonString = await content.ReadAsStringAsync();
        using JsonDocument jsonDoc = JsonDocument.Parse(jsonString);
        return jsonDoc.RootElement.TryGetProperty("Function", out JsonElement func) &&
               func.GetString() == TestFunctionName &&
               jsonDoc.RootElement.TryGetProperty("Parameters", out JsonElement parameters) &&
               parameters.ValueKind == JsonValueKind.Null; // Check if Parameters is explicitly null
    }


    [Fact]
    public async Task CallLacrmApiAsync_ApiError_ThrowsLacrmExceptionAndLogsRequest()
    {
        // Arrange
        string errorResponseContent = "{\"Error\":\"Something went wrong\"}";
        SetupHttpResponse(HttpStatusCode.InternalServerError, errorResponseContent);

        // Act & Assert
        LacrmException exception = await Assert.ThrowsAsync<LacrmException>(() =>
            _lacrmHttpManager.CallLacrmApiAsync<TestDto>(TestFunctionName)
        );

        // Assert Exception details
        Assert.Equal(HttpStatusCode.InternalServerError, exception.StatusCode);
        Assert.Contains(errorResponseContent, exception.Message); // Check if the response content is in the message

        // Verify DataStore logging
        _mockDataStore.Verify(static ds => ds.AddData(It.Is<ApiRequest>(x =>
            x.StatusCode == (int)HttpStatusCode.InternalServerError &&
            x.Endpoint == TestFunctionName
        )), Times.Once);

        // Verify SignalR broadcast
        _mockClientProxy.Verify(static c => c.SendCoreAsync(
            "ReceiveNewData",
            It.Is<object[]>(static x => x.Length == 1 && ((ApiRequest)x[0]).StatusCode == (int)HttpStatusCode.InternalServerError),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

     [Fact]
    public async Task CallLacrmApiAsync_InvalidJsonResponse_ThrowsJsonException()
    {
        // Arrange
        string invalidJson = "this is not json";
        SetupHttpResponse(HttpStatusCode.OK, invalidJson); // Success status but invalid content

        // Act & Assert
        // JsonSerializer.Deserialize throws JsonException directly
        await Assert.ThrowsAsync<JsonException>(() =>
            _lacrmHttpManager.CallLacrmApiAsync<TestDto>(TestFunctionName)
        );

        // Verify logging and SignalR still happened before the exception
         _mockDataStore.Verify(static ds => ds.AddData(It.Is<ApiRequest>(x => x.StatusCode == (int)HttpStatusCode.OK)), Times.Once);
         _mockClientProxy.Verify(static c => c.SendCoreAsync("ReceiveNewData", It.Is<object[]>(x => x != null), It.IsAny<CancellationToken>()), Times.Once);
    }


    [Fact]
    public async Task CallLacrmApiAsync_ApiKeyNotConfigured_ThrowsConfigurationException()
    {
        // Arrange
        // Override the default setup for this specific test
        _mockConfiguration.Setup(c => c["LACRM_API_KEY"]).Returns("YOUR_API_KEY_HERE");

        // Re-create the manager with the bad configuration for this test
        LacrmHttpManager managerWithBadKey = new LacrmHttpManager(
            _httpClient,
            _mockConfiguration.Object,
            _mockHubContext.Object,
            _mockDataStore.Object
        );

        // Act & Assert
        ConfigurationException exception = await Assert.ThrowsAsync<ConfigurationException>(() =>
            managerWithBadKey.CallLacrmApiAsync<TestDto>(TestFunctionName)
        );

        Assert.Equal("LACRM API Key is not configured.", exception.Message);

        // Verify nothing else was called
        _mockHttpMessageHandler.Protected().Verify("SendAsync", Times.Never(), ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());
        _mockDataStore.Verify(static ds => ds.AddData(It.IsAny<ApiRequest>()), Times.Never);
        _mockClientProxy.Verify(static c => c.SendCoreAsync(It.IsAny<string>(), It.Is<object[]>(x => x != null), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CallLacrmApiAsync_HttpRequestException_ThrowsHttpRequestException()
    {
        // Arrange
        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ThrowsAsync(new HttpRequestException("Network error")); // Simulate network issue

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() =>
            _lacrmHttpManager.CallLacrmApiAsync<TestDto>(TestFunctionName)
        );

        // Verify nothing was logged/sent *after* the exception (as SendAsync failed)
        _mockDataStore.Verify(static ds => ds.AddData(It.IsAny<ApiRequest>()), Times.Never);
        _mockClientProxy.Verify(static c => c.SendCoreAsync(It.IsAny<string>(), It.Is<object[]>(x => x != null), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")] // Whitespace
    public async Task CallLacrmApiAsync_NullOrWhitespaceFunctionName_ThrowsNullReferenceException(string? invalidFunctionName)
    {
        // Arrange
        var data = new Dictionary<string, object?> { { "key", "value" } }; // Dummy data

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NullReferenceException>(() =>
            _lacrmHttpManager.CallLacrmApiAsync<object>(invalidFunctionName!, data)
        );

        Assert.Equal("Function name cannot be null or empty.", exception.Message);

        // Verify no HTTP call was made
        _mockHttpMessageHandler
            .Protected()
            .Verify(
                "SendAsync",
                Times.Never(), // Crucial verification
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            );

        // Verify no interaction with Hub or DataStore
        _mockDataStore.Verify(static ds => ds.AddData(It.IsAny<ApiRequest>()), Times.Never);
        _mockClientProxy.Verify(static c => c.SendCoreAsync(It.IsAny<string>(), It.Is<object[]>(x => x != null), It.IsAny<CancellationToken>()), Times.Never);
    }
}
