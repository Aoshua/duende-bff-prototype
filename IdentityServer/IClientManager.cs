using Duende.IdentityServer.Models;

namespace IdentityServer;

public interface IClientManager
{
    Task<Client> FindClientByIdAsync(string clientId);
    Task<List<Client>> GetAllClientsAsync();
}
