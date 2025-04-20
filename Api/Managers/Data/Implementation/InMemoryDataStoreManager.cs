using System.Collections.Concurrent;
using ApiGateway.Models.Table;

namespace ApiGateway.Api.Managers.Data.Implementation;

public class InMemoryDataStoreManager : IInMemoryDataStoreManager
{
    // Using ConcurrentBag for thread-safe additions and enumeration
    private readonly ConcurrentBag<ApiRequest> _dataStore = new ConcurrentBag<ApiRequest>();

    public void AddData(ApiRequest data)
    {
        _dataStore.Add(data);
    }

    public IEnumerable<ApiRequest> GetAllData()
    {
        // Return a snapshot of the data
        return _dataStore.ToList();
    }
}
