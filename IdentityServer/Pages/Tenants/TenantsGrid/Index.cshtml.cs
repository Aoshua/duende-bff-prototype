using IdentityServer.Managers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityServer.Pages.Tenants.TenantsGrid
{
    public class IndexModel : PageModel
    {
        private readonly TenantManager manager;

        public ViewModel View { get; set; }

        public IndexModel(TenantManager manager)
        {
            this.manager = manager;
        }

        public async Task<IActionResult> OnGet()
        {
            View = new ViewModel
            {
                AllTenants = await manager.GetAllTenantsGraphedAsync()
            };

            return Page();
        }
    }
}
