using Microsoft.AspNetCore.SignalR;
using ApiGateway.Models.Table;
using ApiGateway.Api.Managers.Data.Implementation;

namespace ApiGateway.Api.Hubs;

public class DataHub : Hub
{
    private readonly InMemoryDataStoreManager _dataStore;

    // Inject the singleton data store service
    public DataHub(InMemoryDataStoreManager dataStore)
    {
        _dataStore = dataStore;
    }

    // Method for clients to call to add new data
    public async Task AddNewData(ApiRequest newData)
    {
        // Add data to the central store
        _dataStore.AddData(newData);

        // Notify *all* connected clients about the new data item
        // Clients will need a handler named "ReceiveNewData"
        await Clients.All.SendAsync("ReceiveNewData", newData);
    }

    // Override OnConnectedAsync to send the current list to the new client
    public override async Task OnConnectedAsync()
    {
        // Get all current data from the store
        var currentData = _dataStore.GetAllData();

        // Send the full list only to the client that just connected
        // Clients will need a handler named "ReceiveInitialData"
        await Clients.Caller.SendAsync("ReceiveInitialData", currentData);

        await base.OnConnectedAsync();
        Console.WriteLine($"--> Client connected to DataHub: {Context.ConnectionId}");
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        Console.WriteLine($"--> Client disconnected from DataHub: {Context.ConnectionId}");
        return base.OnDisconnectedAsync(exception);
    }
}
