using Microsoft.AspNetCore.Http;
using System;
using System.Security.Claims;
using Application.Interfaces;

namespace WebApi.Services
{
    public class AuthenticatedUserService : IAuthenticatedUserService
    {
        public AuthenticatedUserService(IHttpContextAccessor httpContextAccessor)
        {
            UserId = !string.IsNullOrEmpty(httpContextAccessor.HttpContext?.User?.FindFirstValue("UserId")) ? new Guid(httpContextAccessor.HttpContext?.User?.FindFirstValue("UserId")) : new Guid();
            if (httpContextAccessor.HttpContext?.User != null)
                UserName = httpContextAccessor.HttpContext?.User.Identity.Name;
        }
        public Guid UserId { get; set; }
        public string UserName { get; set; }

    }
}
