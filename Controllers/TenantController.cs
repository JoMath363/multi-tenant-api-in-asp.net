using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Multi_Tenant_API.Data;
using Multi_Tenant_API.Dtos;
using Multi_Tenant_API.Models;

[ApiController]
[Route("tenants")]
public class TenantController : ControllerBase
{
  private readonly AppDbContext _context;
  private readonly UserManager<AccountModel> _userManager;
  private readonly RoleManager<IdentityRole<Guid>> _roleManager;

  public TenantController(
    AppDbContext context,
    UserManager<AccountModel> userManager,
    RoleManager<IdentityRole<Guid>> roleManager
  )
  {
    _context = context;
    _userManager = userManager;
    _roleManager = roleManager;
  }

  [Authorize]
  [HttpGet("accounts")]
  public async Task<IActionResult> ListTenantAccounts()
  {
    var tenantId = Guid.Parse(User.FindFirst("TenantId")?.Value ?? "");

    var acounts = await _context.Accounts.Where(a => a.TenantId == tenantId).ToListAsync();

    var mappedAccounts = acounts.Select(a => new
    {
      userName = a.UserName,
      email = a.Email
    });

    return Ok(mappedAccounts);
  }

  [HttpPost("register")]
  public async Task<IActionResult> RegisterTenant([FromBody] RegisterTenantDto dto)
  {
    using var transaction = await _context.Database.BeginTransactionAsync();

    try
    {
      var tenant = new TenantModel
      {
        Name = dto.Name,
        Plan = dto.Plan
      };

      await _context.Tenants.AddAsync(tenant);
      await _context.SaveChangesAsync();

      var account = new AccountModel
      {
        UserName = dto.Account.UserName,
        Email = dto.Account.Email,
        TenantId = tenant.Id
      };

      var result = await _userManager.CreateAsync(account, dto.Account.Password);
      if (!result.Succeeded)
      {
        await transaction.RollbackAsync();
        return BadRequest(result.Errors);
      }

      await _userManager.AddToRoleAsync(account, "Admin");

      await transaction.CommitAsync();

      return Ok(new
      {
        TenantId = tenant.Id,
        TenantName = tenant.Name,
        AccountEmail = account.Email
      });
    }
    catch (Exception ex)
    {
      await transaction.RollbackAsync();
      return StatusCode(500, ex.Message);
    }
  }

  [Authorize(Roles = "Admin")]
  [HttpPatch("plan/{plan}")]
  public async Task<IActionResult> UpdateTenantPlan(string plan)
  {
    var tenantId = Guid.Parse(User.FindFirst("TenantId")?.Value ?? "");
    var tenant = await _context.Tenants.FirstOrDefaultAsync(t => t.Id == tenantId);

    if (tenant == null)
      return NotFound(new { error = "Tenant not found." });

    var projectsCount = await _context.Projects.CountAsync(p => p.TenantId == tenantId);

    if (!Enum.TryParse<Plan>(plan, ignoreCase: true, out var parsedPlan))
      return StatusCode(404, new { error = "Invalid plan: insert a valid plan." });

    if ((parsedPlan == Plan.Free && projectsCount > 3) ||
        (parsedPlan == Plan.Standard && projectsCount > 10))
    {
      return StatusCode(403, new { error = "Project count exceeds the limits of the selected plan." });
    }

    tenant.Plan = parsedPlan;
    await _context.SaveChangesAsync();

    return Ok(new { message = "Tenant plan updated successfully." });
  }

  [Authorize(Roles = "Admin")]
  [HttpDelete]
  public async Task<IActionResult> DeleteTenant()
  {
    var tenantId = Guid.Parse(User.FindFirst("TenantId")?.Value ?? "");

    var tenant = await _context.Tenants
    .FirstOrDefaultAsync(t => t.Id == tenantId);

    if (tenant == null)
      return NotFound(new { error = "Tenant not found." });

    _context.Tenants.Remove(tenant);
    await _context.SaveChangesAsync();

    return Ok(new { message = "Tenant deleted successfully." });
  }
}