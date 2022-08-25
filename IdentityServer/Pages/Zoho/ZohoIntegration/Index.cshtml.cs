using IdentityServer.Managers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace IdentityServer.Pages.Zoho.ZohoIntegration
{
    public class IndexModel : PageModel
    {
        private readonly ZohoManager manager;
        private readonly IConfiguration config;

        public ViewModel View { get; set; }

        public IndexModel(ZohoManager manager, IConfiguration config)
        {
            this.manager = manager;
            this.config = config;
        }

        public async Task<IActionResult> OnGet()
        {
            var token = await manager.GetZohoToken();
            
            if (token == null)
            {
                // We need to go authenticate against Zoho
                var url = "https://accounts.zoho.com/oauth/v2/auth";
                var queryParams = new Dictionary<string, string>
                {
                    { "scope", ZohoManager.Scope },
                    { "client_id", manager.GetZohoClientId() },
                    { "client_secret", manager.GetZohoClientSecret() },
                    { "response_type", "code" },
                    { "access_type", "offline" },
                    { "redirect_uri", "https://localhost:5001/zoho/zohocode" }
                };

                var uri = new Uri(QueryHelpers.AddQueryString(url, queryParams));

                return Redirect(uri.ToString());
            }

            var customers = await manager.GetZohoCustomers();

            View = new ViewModel
            {
                Customers = customers
            };

            return Page();
        }
    }
}
