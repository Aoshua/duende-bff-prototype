using IdentityServer.Managers;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Serilog;

namespace IdentityServer.Pages.Zoho
{
    public class ZohoCodeModel : PageModel
    {
        private readonly ZohoManager manager;

        public ZohoCodeModel(ZohoManager manager)
        {
            this.manager = manager;
        }

        public async Task OnGet(string code, string location, string accountsServer, string sourceId, string ie)
        {
            Log.Information("Zoho Code " + code);
            var client = new HttpClient();

            var url = "https://accounts.zoho.com/oauth/v2/token";

            var form = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("client_id", manager.GetZohoClientId()),
                new KeyValuePair<string, string>("client_secret", manager.GetZohoClientSecret()),
                new KeyValuePair<string, string>("redirect_uri", "https://localhost:5001/zoho/zohocode"),
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("scope", ZohoManager.Scope)
            });

            var response = await client.PostAsync(url, form);
            var content = await response.Content.ReadAsStringAsync();
            
            await manager.StoreTokenResponse(content);

            Redirect("/zoho/zohointegration");
        }
    }
}
