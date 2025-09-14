namespace Multi_Tenant_API.Models;

public enum Plan
{
  Free,
  Standart,
  Premium
}

public class TenantModel
{
  public Guid Id { get; set; } = Guid.NewGuid();
  public required string Name { get; set; }
  public required Plan Plan { get; set; }
  public DateTime CreatedAt { get; set; } = DateTime.Now;
}
