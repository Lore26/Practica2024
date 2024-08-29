using Microsoft.AspNetCore.Identity;

namespace TrainsAPI.Services;

public class UsersService(
    IHttpContextAccessor httpContextAccessor,
    UserManager<IdentityUser> userManager) : IUsersService
{
    public async Task<IdentityUser?> GetUser()
    {
        var emailClaim = httpContextAccessor.HttpContext!
            .User.Claims.FirstOrDefault(x => x.Type == "email");

        if (emailClaim is null)
        {
            return null;
        }

        var email = emailClaim.Value;
        return await userManager.FindByEmailAsync(email);
    }
}
