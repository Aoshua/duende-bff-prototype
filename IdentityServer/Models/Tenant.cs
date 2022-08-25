namespace IdentityServer.Models;

public class Tenant
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;

    public virtual ICollection<ApplicationUser> Users { get; set; } = default!;
}
