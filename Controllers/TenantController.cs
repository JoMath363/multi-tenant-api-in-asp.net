using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Multi_Tenant_API.Data;
using Multi_Tenant_API.Dtos;
using Multi_Tenant_API.Models;

[ApiController]
[Route("tenants")]
public class TenantController : ControllerBase
{
  private AppDbContext _context;
  private IMapper _mapper;

  public TenantController(AppDbContext context, IMapper mapper)
  {
    _context = context;
    _mapper = mapper;
  }

  [HttpPost("register")]
  public async Task<IActionResult> RegisterNewTenant([FromBody] TenantDto dto)
  {
    var tenant = _mapper.Map<TenantModel>(dto);

    await _context.Tenants.AddAsync(tenant);
    await _context.SaveChangesAsync();

    return Ok();
  }

  [HttpGet("me")]
  public void GetCurrentTenant()
  {

  }
  
  [HttpPatch("upgrade")]
  public void UpgradeTenantPlan()
  {
    
  }
}