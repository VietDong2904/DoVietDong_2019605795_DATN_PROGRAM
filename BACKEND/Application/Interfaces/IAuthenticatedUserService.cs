using System;

namespace Application.Interfaces
{
    public interface IAuthenticatedUserService
    {
        Guid UserId { get; set; }
        string UserName { get; set; }
    }
    
}
