using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
        TenantId = tenant.Id,
        CreatedAt = DateTime.UtcNow
      };

      var result = await _userManager.CreateAsync(account, dto.Account.Password);
      if (!result.Succeeded)
      {
        await transaction.RollbackAsync();
        return BadRequest(result.Errors);
      }

      if (!await _roleManager.RoleExistsAsync("Admin"))
        await _roleManager.CreateAsync(new IdentityRole<Guid>("Admin"));

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

  [HttpGet("me")]
  public void GetUserTenant()
  {
    // Permission: All
  }

  [HttpPatch("upgrade")]
  public void UpgradeTenantPlan()
  {
    // Permission: Admin
  }
}