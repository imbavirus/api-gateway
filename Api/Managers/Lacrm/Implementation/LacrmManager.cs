using ApiGateway.Models.Lacrm.User;
using ApiGateway.Managers.Lacrm;
using ApiGateway.Models.Exceptions;

namespace Api.Managers.Lacrm.Implementation;

public class LacrmManager : ILacrmManager
{
    private readonly ILacrmHttpManager _lacrmHttpManager;

    public LacrmManager(ILacrmHttpManager lacrmHttpManager)
    {
        _lacrmHttpManager = lacrmHttpManager;
    }

    public async Task<User> GetUser()
    {
        return (User?)await _lacrmHttpManager.CallLacrmApiAsync("GetUser") ?? throw new LacrmException("Failed to retrieve user data from LACRM.", System.Net.HttpStatusCode.Unauthorized);
    }

}
