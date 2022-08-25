using Duende.IdentityServer.AspNetIdentity;
using Duende.IdentityServer.Models;
using IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace IdentityServer;

public class TenantProfileService : ProfileService<ApplicationUser>
{
	public TenantProfileService(UserManager<ApplicationUser> userManager, 
			IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory)
		: base(userManager, claimsFactory) { }

	protected override async Task GetProfileDataAsync(ProfileDataRequestContext context, ApplicationUser user)
	{
        var principal = await GetUserClaimsAsync(user);
        var id = (ClaimsIdentity)principal.Identity;
        id.AddClaim(new Claim("tenant_id", user.TenantId.ToString()));

        context.AddRequestedClaims(principal.Claims);
    }
}
