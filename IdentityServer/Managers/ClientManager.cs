using Duende.IdentityServer.EntityFramework.DbContexts;
using Microsoft.EntityFrameworkCore;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.EntityFramework.Mappers;

namespace IdentityServer.Managers
{
    public class ClientManager : IClientManager
    {
        private readonly ConfigurationDbContext context;

        public ClientManager(ConfigurationDbContext context)
        {
            this.context = context;
        }

        public async Task<Client> FindClientByIdAsync(string clientId)
            => (await context.Clients.FindAsync(clientId)).ToModel();

        public async Task<List<Client>> GetAllClientsAsync()
            => (await context.Clients
                                .Include(x => x.AllowedGrantTypes)
                                .ToListAsync())
                .Select(x => x.ToModel())
                .ToList();
    }
}
