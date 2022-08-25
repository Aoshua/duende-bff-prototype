using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityServer.Pages.Zoho
{
    public class ZohoTokenModel : PageModel
    {
        public async Task OnGet(string access_token, string refresh_token)
        {
            var thing = access_token;
        }
    }
}
