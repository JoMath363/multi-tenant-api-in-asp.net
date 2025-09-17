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

  [Authorize(Roles = "Admin,Manager")]
  [HttpPost]
  public async Task<IActionResult> AddNewProject([FromBody] ProjectDto dto)
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
  [HttpGet("{projectId}")]
  public async Task<IActionResult> GetProjectById(string projectId)
  {
    var tenantId = Guid.Parse(User.FindFirst("TenantId")?.Value ?? "");

    var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == Guid.Parse(projectId));

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
}