using IdentityServer.Models;

namespace IdentityServer.Pages.Tenants.TenantsGrid
{
    public class ViewModel
    {
        public IEnumerable<Tenant> AllTenants { get; set; } = Enumerable.Empty<Tenant>();
    }
}
