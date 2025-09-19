using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Multi_Tenant_API.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Multi_Tenant_API.Data;

public class AppDbContext : IdentityDbContext<AccountModel, IdentityRole<Guid>, Guid>
{
    public DbSet<TenantModel> Tenants { get; set; }
    public DbSet<AccountModel> Accounts { get; set; }
    public DbSet<ProjectModel> Projects { get; set; }
    public DbSet<TaskModel> Tasks { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<TenantModel>()
            .HasMany(t => t.Accounts)
            .WithOne(a => a.Tenant)
            .HasForeignKey(a => a.TenantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<TenantModel>()
            .HasMany(t => t.Projects)
            .WithOne(p => p.Tenant)
            .HasForeignKey(p => p.TenantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ProjectModel>()
            .HasMany(p => p.Tasks)
            .WithOne(t => t.Project)
            .HasForeignKey(t => t.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}