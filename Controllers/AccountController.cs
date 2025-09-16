using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Multi_Tenant_API.Data;
using Multi_Tenant_API.Models;

[ApiController]
[Route("accounts")]
public class AccountController : ControllerBase
{
  private readonly AppDbContext _context;
  private readonly UserManager<AccountModel> _userManager;
  private readonly RoleManager<IdentityRole<Guid>> _roleManager;

  public AccountController(
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
  [HttpGet("tenant")]
  public async Task<IActionResult> GetAccountTenant()
  {
    var tenantId = Guid.Parse(User.FindFirst("TenantId")?.Value ?? "");

    var tenant = await _context.Tenants.Where(t => t.Id == tenantId).FirstOrDefaultAsync();

    return Ok(tenant);
  }

  [Authorize(Roles = "Admin")]
  [HttpPatch("{id}/role/{role}")]
  public async Task<IActionResult> UpdateAccountRole(string id, string role)
  {
    var tenantId = Guid.Parse(User.FindFirst("TenantId")?.Value ?? "");

    var user = await _userManager.FindByIdAsync(id);
    if (user == null)
      return NotFound(new { message = "User not found" });

    if (user.TenantId != tenantId)
      return Forbid();

    if (!await _roleManager.RoleExistsAsync(role))
      return BadRequest(new { message = $"Role '{role}' does not exist" });

    var currentRoles = await _userManager.GetRolesAsync(user);
    var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
    if (!removeResult.Succeeded)
      return BadRequest(removeResult.Errors);

    var addResult = await _userManager.AddToRoleAsync(user, role);
    if (!addResult.Succeeded)
      return BadRequest(addResult.Errors);

    return Ok(new { message = $"User {user.UserName} role updated to {role}" });
  }

  [Authorize(Roles = "Admin")]
  [HttpDelete("{id}")]
  public void DeleteAccount()
  {
    // Permission: Admin
  }
}