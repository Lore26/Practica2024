using Microsoft.AspNetCore.Identity;

namespace TrainsAPI.Services;

public interface IUsersService
{
    Task<IdentityUser?> GetUser();
}
