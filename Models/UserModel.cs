namespace Multi_Tenant_API.Models;

public enum Role
{
  Admin,
  User,
  Guest
}

public class UserModel
{
  public Guid Id { get; set; } = Guid.NewGuid();
  public required string Email { get; set; }
  public required  string PasswordHash { get; set; }
  public required Role Role { get; set; }
  public Guid TenantId { get; set; }
  public DateTime CreatedAt { get; set; } = DateTime.Now;
}
