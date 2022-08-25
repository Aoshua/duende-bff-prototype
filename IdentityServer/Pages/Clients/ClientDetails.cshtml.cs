using Duende.IdentityServer.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityServer.Pages.Clients
{
    public class ClientDetailsModel : PageModel
    {
        private readonly IClientManager manager;

        public ClientDetailsModel(IClientManager manager)
        {
            this.manager = manager;
        }

        public async Task<Client> OnGet(string clientId)
        {
            return await manager.FindClientByIdAsync(clientId);
        }
    }
}
