using IdentityServer.Models.Zoho;
using System.Net.Http.Headers;
using System.Text.Json;

namespace IdentityServer.Services
{
    public class ZohoClientService
    {
        private ZohoClient client;
        private const string subscriptionsUrl = "https://subscriptions.zoho.com/api/v1/";

        public ZohoClientService(IConfiguration configuration)
        {
            client = new ZohoClient(configuration);
            // client.BaseAddress = new Uri(subscriptionsUrl);
        }

        public void SetAuthorization(string token)
        {
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<IEnumerable<Customer>> GetAllCustomers()
        {
            var result = await client.GetAsync(subscriptionsUrl + "customers");

            result.EnsureSuccessStatusCode();

            var response = await result.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<AllCustomersResponse>(response);
            return data.customers;
        }
    }

    public class ZohoClient : HttpClient
    {
        private readonly IConfiguration config;

        public ZohoClient(IConfiguration config)
        {
            this.config = config;
        }

        public new async Task<HttpResponseMessage> GetAsync(string requestUri)
        {
            if (!HasAuthorization()) 
                throw new Exception("Authorization header not found for Zoho!");

            AttachOrganizationHeader();

            // Todo: Implemeent logic to look at access token expiration, yada yada

            return await base.GetAsync(requestUri);
        }

        public bool HasAuthorization()
            => DefaultRequestHeaders.Authorization != null;

        public void AttachOrganizationHeader()
        {
            var zoho = config.GetSection("Zoho");
#if DEBUG
            var dev = zoho.GetSection("Development");
            var orgId = dev["OrganizationId"];
            DefaultRequestHeaders.Add("X-com-zoho-subscriptions-organizationid", orgId);
#else
            var prod = zoho.GetSection("Production");
            var orgId = prod["OrganizationId"];
            DefaultRequestHeaders.Add("X-com-zoho-subscriptions-organizationid", orgId);
#endif

        }
    }

    public class ZohoWrapper
    {
        public int code { get; set; }
        public string message { get; set; }
    }

    public class AllCustomersResponse : ZohoWrapper
    {
        public IEnumerable<Customer> customers { get; set; }
    }
}
