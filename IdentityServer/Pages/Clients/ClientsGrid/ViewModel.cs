using Duende.IdentityServer.Models;

namespace IdentityServer.Pages.Clients.ClientsGrid
{
    public class ViewModel
    {
        public IEnumerable<Client> AllClients { get; set; } = Enumerable.Empty<Client>();
    }
}
