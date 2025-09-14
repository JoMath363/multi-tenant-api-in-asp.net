namespace Multi_Tenant_API.Models;

public class ProjectModel
{
  public Guid Id { get; set; } = Guid.NewGuid();
  public required string Name { get; set; }
  public required string Description { get; set; }
  public Guid TenantId { get; set; }
  public DateTime CreatedAt { get; set; } = DateTime.Now;
}
