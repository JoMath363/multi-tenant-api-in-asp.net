namespace Multi_Tenant_API.Models;

public enum Plan
{
  Free,
  Standard,
  Premium
}

public class TenantModel
{
  public Guid Id { get; set; } = Guid.NewGuid();
  public required string Name { get; set; }
  public required Plan Plan { get; set; }
  public DateTime CreatedAt { get; set; } = DateTime.Now;
  public List<AccountModel>? Accounts { get; set; }
  public List<ProjectModel>? Projects { get; set; }
}
