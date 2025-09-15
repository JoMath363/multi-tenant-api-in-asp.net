using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Multi_Tenant_API.Data;
using Multi_Tenant_API.Dtos;
using Multi_Tenant_API.Models;

[ApiController]
[Route("projects")]
public class ProjectController : ControllerBase
{
  private AppDbContext _context;
  private IMapper _mapper;

  public ProjectController(AppDbContext context, IMapper mapper)
  {
    _context = context;
    _mapper = mapper;
  }

  [HttpPost]
  public void AddNewProject()
  {
    // Permission: Manager
  }

  [HttpGet]
  public void ListTenantProjects()
  {
    // Permission: All
  }

  [HttpPost("{id}/tasks")]
  public void AddProjectTask()
  {
    // Permission: All
  }

  [HttpGet("{id}/tasks")]
  public void ListProjectTasks()
  {
    // Permission: All
  }

  [HttpDelete("{id}/tasks")]
  public void RemoveProjectTask()
  {
    // Permission: All
  }
}