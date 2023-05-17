using System;
using System.Security.Claims;

namespace cm.BackendApi.Helpers
{
    public interface ITokenHelper
    {
        ClaimsPrincipal GetPrincipal(string token);

        Guid GetUserIdToGuid(string accessToken);

        int GetUserIdToInt(string accessToken);

        long GetUserIdToLong(string accessToken);

        string GetUsername(string accessToken);
    }
}