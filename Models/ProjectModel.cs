namespace Multi_Tenant_API.Models;

public class ProjectModel
{
  public Guid Id { get; set; } = Guid.NewGuid();
  public required string Name { get; set; }
  public required string Description { get; set; }
  public DateTime CreatedAt { get; set; } = DateTime.Now;
  public Guid TenantId { get; set; }
  public TenantModel? Tenant { get; set; }
  public List<TaskModel>? Tasks { get; set; }
}
