using Microsoft.AspNetCore.Identity;

namespace Multi_Tenant_API.Models;

public enum Role
{
  Admin,
  Manager,
  User
}

public class AccountModel : IdentityUser<Guid>
{
  public Guid TenantId { get; set; }
  public TenantModel Tenant { get; set; } = null!;
  public DateTime CreatedAt { get; set; } = DateTime.Now;
}
