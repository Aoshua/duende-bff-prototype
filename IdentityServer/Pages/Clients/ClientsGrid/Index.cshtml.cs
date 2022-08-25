using IdentityServer.Pages.Clients.ClientsGrid;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityServer.Pages.Clients
{
    public class ClientsGridModel : PageModel
    {
        private readonly IClientManager manager;

        public ViewModel View { get; set; }

        public ClientsGridModel(IClientManager manager)
        {
            this.manager = manager;
        }

        public async Task<IActionResult> OnGet()
        {
            View = new ViewModel
            {
                AllClients = await manager.GetAllClientsAsync()
            };

            return Page();
        }
    }
}
