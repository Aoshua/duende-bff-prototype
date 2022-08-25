using IdentityServer.Data;
using IdentityServer.Models;
using IdentityServer.Models.Zoho;
using IdentityServer.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace IdentityServer.Managers;

public class ZohoManager
{
    private readonly ApplicationDbContext context;
    private readonly IConfiguration config;
    private readonly ZohoClientService client;
    public static string Scope = "ZohoSubscriptions.fullaccess.all";

    public ZohoManager(ApplicationDbContext context,
                        IConfiguration config,
                        ZohoClientService client)
    {
        this.context = context;
        this.config = config;
        this.client = client;
    }

    public async Task CreateZohoToken(ZohoToken token)
    {
        // First delete everything
        var tokens = await context.ZohoTokens.ToListAsync();
        context.RemoveRange(tokens);
        await context.SaveChangesAsync();

        await context.ZohoTokens.AddAsync(token);
        await context.SaveChangesAsync();
    }

    public async Task<ZohoToken> GetZohoToken()
        => await context.ZohoTokens.FirstOrDefaultAsync();

    public string GetZohoClientSecret()
    {
        var zoho = config.GetSection("Zoho");

#if DEBUG
        var dev = zoho.GetSection("Development");
        return dev["ClientSecret"];
#else

#endif
    }

    public string GetZohoClientId()
    {
        var zoho = config.GetSection("Zoho");

#if DEBUG
        var dev = zoho.GetSection("Development");
        return dev["ClientId"];
#else

#endif
    }

    public async Task StoreTokenResponse(string json)
    {
        var parsed = JsonSerializer.Deserialize<ZohoAccessToken>(json);

        await CreateZohoToken(new ZohoToken
        {
            AccessToken = parsed.access_token,
            RefreshToken = parsed.refresh_token
        });
    }

    public async Task<IEnumerable<Customer>> GetZohoCustomers()
    {
        var token = await GetZohoToken();
        client.SetAuthorization(token.AccessToken);

        return await client.GetAllCustomers();
    }

    public class ZohoAccessToken
    {
        public string access_token { get; set; }
        public string refresh_token { get; set; }
    }
}
