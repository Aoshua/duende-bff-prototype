namespace IdentityServer.Models
{
    public class ZohoToken
    {
        public int Id { get; set; }
        public string AccessToken { get; set; } = default!;
        public string RefreshToken { get; set; } = default!;
    }
}
