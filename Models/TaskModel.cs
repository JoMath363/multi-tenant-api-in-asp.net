namespace Multi_Tenant_API.Models;

public enum Status
{
  Pending,
  InProgress,
  Blocked,
  Completed,
  Cancelled
}

public class TaskModel
{
  public Guid Id { get; set; } = Guid.NewGuid();
  public required string Title { get; set; }
  public required string Description { get; set; }
  public Status Status { get; set; } = Status.Pending;
  public DateTime? DueDate { get; set; }
  public required Guid TenantId { get; set; }
  public required Guid ProjectId { get; set; }
  public ProjectModel? Project { get; set; }
}
