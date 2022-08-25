using IdentityServer.Data;
using IdentityServer.Models;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer.Managers;

public class TenantManager
{
	private readonly ApplicationDbContext context;

	public TenantManager(ApplicationDbContext context)
	{
		this.context = context;
	}

	public async Task CreateTenantAsync(Tenant tenant)
	{
        await context.Tenants.AddAsync(tenant);
		await context.SaveChangesAsync();
    }

	public async Task<Tenant> GetTenantById(Guid id)
		=> await context.Tenants.FindAsync(id);

	public async Task<List<Tenant>> GetAllTenantsAsync()
		=> await context.Tenants.ToListAsync();

	public async Task<List<Tenant>> GetAllTenantsGraphedAsync()
		=> await context.Tenants.Include(x => x.Users).ToListAsync();
}
