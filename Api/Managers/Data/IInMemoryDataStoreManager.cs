using System.Collections.Generic;
using ApiGateway.Models.Table;

namespace ApiGateway.Managers;

/// <summary>
/// Defines the contract for an in-memory data store for API requests.
/// </summary>
public interface IInMemoryDataStoreManager
{
    /// <summary>
    /// Adds a new API request record to the store.
    /// </summary>
    /// <param name="data">The API request data to add.</param>
    void AddData(ApiRequest data);

    /// <summary>
    /// Retrieves all API request records currently in the store.
    /// </summary>
    /// <returns>An enumerable collection of all stored API requests.</returns>
    IEnumerable<ApiRequest> GetAllData();
}
