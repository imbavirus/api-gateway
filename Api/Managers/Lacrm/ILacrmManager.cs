using ApiGateway.Models.Lacrm.User;

namespace Api.Managers.Lacrm; // Interface namespace

/// <summary>
/// Defines the contract for managing LACRM specific operations.
/// </summary>
public interface ILacrmManager
{
    /// <summary>
    /// Retrieves the current user information from LACRM.
    /// </summary>
    /// <returns>A Task representing the asynchronous operation, containing the User details.</returns>
    Task<User> GetUser();
}
