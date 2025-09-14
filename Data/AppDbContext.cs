using Microsoft.EntityFrameworkCore;
using Multi_Tenant_API.Models;

namespace Multi_Tenant_API.Data;

public class AppDbContext : DbContext
{
  public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

  public DbSet<TenantModel> Tenants { get; set; }
}