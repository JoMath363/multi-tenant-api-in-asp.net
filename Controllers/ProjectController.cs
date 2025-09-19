using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Multi_Tenant_API.Data;
using Multi_Tenant_API.Dtos;
using Multi_Tenant_API.Models;

[ApiController]
[Route("projects")]
public class ProjectController : ControllerBase
{
  private AppDbContext _context;

  public ProjectController(AppDbContext context)
  {
    _context = context;
  }

  [Authorize]
  [HttpGet]
  public async Task<IActionResult> GetProjects()
  {
    var tenantId = Guid.Parse(User.FindFirst("TenantId")?.Value ?? "");

    var projects = await _context.Projects.Where(p => p.TenantId == tenantId).ToListAsync();

    var mappedProjects = projects.Select(p => new
    {
      id = p.Id,
      name = p.Name,
      description = p.Description,
      createdAt = p.CreatedAt
    });

    return Ok(mappedProjects);
  }

  [Authorize]
  [HttpGet("{projectId}")]
  public async Task<IActionResult> GetProjectById(string projectId)
  {
    var tenantId = Guid.Parse(User.FindFirst("TenantId")?.Value ?? "");

    if (!Guid.TryParse(projectId, out Guid projectGuid))
      return BadRequest(new { error = "Invalid project ID." });

    var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectGuid);

    if (project == null)
      return NotFound(new { error = "Project not found." });

    if (project.TenantId != tenantId)
      return Forbid("This project is not from your tenant.");

    return Ok(new
    {
      id = project.Id,
      name = project.Name,
      description = project.Description,
      createdAt = project.CreatedAt
    });
  }

  [Authorize(Roles = "Admin,Manager")]
  [HttpPost]
  public async Task<IActionResult> AddNewProject([FromBody] RegisterProjectDto dto)
  {
    var tenantId = Guid.Parse(User.FindFirst("TenantId")?.Value ?? "");
    var tenant = await _context.Tenants.FirstOrDefaultAsync(t => t.Id == tenantId);

    if (tenant == null)
      return NotFound(new { error = "Tenant not found." });

    var projectsCount = await _context.Projects.CountAsync(p => p.TenantId == tenantId);

    if (tenant.Plan != Plan.Premium)
    {
      if ((tenant.Plan == Plan.Free && projectsCount >= 3) ||
          (tenant.Plan == Plan.Standard && projectsCount >= 10))
      {
        return StatusCode(403, new { error = "Tenant projects limit exceeded." });
      }
    }

    var project = new ProjectModel
    {
      Name = dto.Name,
      Description = dto.Description,
      TenantId = tenantId
    };

    await _context.Projects.AddAsync(project);
    await _context.SaveChangesAsync();

    return Ok(new
    {
      id = project.Id,
      name = project.Name,
      description = project.Description,
      createdAt = project.CreatedAt
    });
  }

  [Authorize]
  [HttpPatch("{projectId}")]
  public async Task<IActionResult> UpdateProjectById([FromBody] UpdateProjectDto dto, string projectId)
  {
    var tenantId = Guid.Parse(User.FindFirst("TenantId")?.Value ?? "");

    if (!Guid.TryParse(projectId, out Guid projectGuid))
      return BadRequest(new { error = "Invalid project ID." });

    var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectGuid);

    if (project == null)
      return NotFound(new { error = "Project not found." });

    if (project.TenantId != tenantId)
      return Forbid("This project is not from your tenant.");

    project.Name = dto.Name ?? project.Name;
    project.Description = dto.Description ?? project.Description;

    await _context.SaveChangesAsync();

    return Ok(new { message = "Project upadted successfully" });
  }

  [Authorize]
  [HttpDelete("{projectId}")]
  public async Task<IActionResult> DeleteProjectById(string projectId)
  {
    var tenantId = Guid.Parse(User.FindFirst("TenantId")?.Value ?? "");

    if (!Guid.TryParse(projectId, out Guid projectGuid))
      return BadRequest(new { error = "Invalid project ID." });

    var project = await _context.Projects
    .FirstOrDefaultAsync(p => p.Id == projectGuid);

    if (project == null)
      return NotFound(new { error = "Project not found." });

    if (project.TenantId != tenantId)
      return Forbid("This project does not belong to your tenant.");

    _context.Projects.Remove(project);
    await _context.SaveChangesAsync();

    return Ok(new { message = "Project deleted successfully." });
  }

  [Authorize]
  [HttpGet("{projectId}/tasks")]
  public async Task<IActionResult> GetTasks(string projectId)
  {
    var tenantId = Guid.Parse(User.FindFirst("TenantId")?.Value ?? "");

    if (!Guid.TryParse(projectId, out Guid projectGuid))
      return BadRequest(new { error = "Invalid project ID." });

    var tasks = await _context.Tasks
    .Where(t => t.ProjectId == projectGuid && t.TenantId == tenantId)
    .ToListAsync();

    var mappedTasks = tasks.Select(t => new
    {
      id = t.Id,
      title = t.Title,
      description = t.Description,
      status = Enum.GetName(typeof(Status), t.Status),
      dueDate = t.DueDate
    });

    return Ok(mappedTasks);
  }

  [Authorize]
  [HttpGet("{projectId}/tasks/{taskId}")]
  public async Task<IActionResult> GetTaskById(string projectId, string taskId)
  {
    var tenantId = Guid.Parse(User.FindFirst("TenantId")?.Value ?? "");

    if (!Guid.TryParse(projectId, out Guid projectGuid))
      return BadRequest(new { error = "Invalid project ID." });

    if (!Guid.TryParse(taskId, out Guid taskGuid))
      return BadRequest(new { error = "Invalid task ID." });

    var task = await _context.Tasks
    .FirstOrDefaultAsync(t => t.Id == taskGuid && t.ProjectId == projectGuid);

    if (task == null)
      return NotFound(new { error = "Task not found." });

    if (task.TenantId != tenantId)
      return Forbid("This project is not from your tenant.");

    return Ok(new
    {
      id = task.Id,
      title = task.Title,
      description = task.Description,
      status = Enum.GetName(typeof(Status), task.Status),
      dueDate = task.DueDate
    });
  }

  [Authorize(Roles = "Admin,Manager")]
  [HttpPost("{projectId}/tasks")]
  public async Task<IActionResult> AddNewTask([FromBody] RegisterTaskDto dto, string projectId)
  {
    var tenantId = Guid.Parse(User.FindFirst("TenantId")?.Value ?? "");

    if (!Guid.TryParse(projectId, out Guid projectGuid))
      return BadRequest(new { error = "Invalid project ID." });

    var tasksCount = await _context.Tasks.CountAsync(p => p.ProjectId == projectGuid);

    if (tasksCount >= 20)
      return StatusCode(403, new { error = "Project tasks limit exceeded." });

    var task = new TaskModel
    {
      Title = dto.Title,
      Description = dto.Description,
      Status = Status.Pending,
      DueDate = dto.DueDate,
      ProjectId = projectGuid,
      TenantId = tenantId
    };

    await _context.Tasks.AddAsync(task);
    await _context.SaveChangesAsync();

    return Ok(new
    {
      id = task.Id,
      title = task.Title,
      description = task.Description,
      status = Enum.GetName(typeof(Status), task.Status),
      dueDate = task.DueDate
    });
  }

  [Authorize]
  [HttpPatch("{projectId}/tasks/{taskId}")]
  public async Task<IActionResult> UpdateTaskById([FromBody] UpdateTaskDto dto, string projectId, string taskId)
  {
    var tenantId = Guid.Parse(User.FindFirst("TenantId")?.Value ?? "");

    if (!Guid.TryParse(projectId, out Guid projectGuid))
      return BadRequest(new { error = "Invalid project ID." });

    if (!Guid.TryParse(taskId, out Guid taskGuid))
      return BadRequest(new { error = "Invalid task ID." });

    var task = await _context.Tasks
    .FirstOrDefaultAsync(t => t.Id == taskGuid && t.ProjectId == projectGuid);

    if (task == null)
      return NotFound(new { error = "Task not found." });

    if (task.TenantId != tenantId)
      return Forbid("This task is not from your tenant.");

    task.Title = dto.Title ?? task.Title;
    task.Description = dto.Description ?? task.Description;
    task.DueDate = dto.DueDate ?? task.DueDate;
    task.Status = dto.Status ?? task.Status;

    await _context.SaveChangesAsync();

    return Ok(new { message = "Task upadted successfully" });
  }

  [Authorize(Roles = "Admin,Manager")]
  [HttpDelete("{projectId}/tasks/{taskId}")]
  public async Task<IActionResult> DeleteTaskById(string projectId, string taskId)
  {
    var tenantId = Guid.Parse(User.FindFirst("TenantId")?.Value ?? "");

    if (!Guid.TryParse(projectId, out Guid projectGuid))
      return BadRequest(new { error = "Invalid project ID." });

    if (!Guid.TryParse(taskId, out Guid taskGuid))
      return BadRequest(new { error = "Invalid task ID." });

    var task = await _context.Tasks
    .FirstOrDefaultAsync(t => t.Id == taskGuid && t.ProjectId == projectGuid);

    if (task == null)
      return NotFound(new { error = "Task not found." });

    if (task.TenantId != tenantId)
      return Forbid("This task does not belong to your tenant.");

    _context.Tasks.Remove(task);
    await _context.SaveChangesAsync();

    return Ok(new { message = "Task deleted successfully." });
  }
}