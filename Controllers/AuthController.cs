using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Multi_Tenant_API.Dtos;
using Multi_Tenant_API.Models;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
  private readonly UserManager<AccountModel> _userManager;
  private readonly RoleManager<IdentityRole<Guid>> _roleManager;
  private readonly IConfiguration _configuration;

  public AuthController(
    UserManager<AccountModel> userManager,
    RoleManager<IdentityRole<Guid>> roleManager,
    IConfiguration configuration
  )
  {
    _userManager = userManager;
    _roleManager = roleManager;
    _configuration = configuration;
  }

  [Authorize(Roles = "Admin")]
  [HttpPost("register")]
  public async Task<IActionResult> Register([FromBody] RegisterAccountDto dto)
  {
    var accountExists = await _userManager.FindByEmailAsync(dto.Email);
    if (accountExists != null)
      return BadRequest("User already exists");

    var tenantId = Guid.Parse(User.FindFirst("TenantId")?.Value ?? "");

    var account = new AccountModel
    {
      UserName = dto.UserName,
      Email = dto.Email,
      TenantId = tenantId
    };

    var result = await _userManager.CreateAsync(account, dto.Password);
    if (!result.Succeeded)
      return BadRequest(result.Errors);

    if (!await _roleManager.RoleExistsAsync("User"))
      await _roleManager.CreateAsync(new IdentityRole<Guid>("User"));

    await _userManager.AddToRoleAsync(account, "User");

    return Ok("Account created successfully");
  }

  [HttpPost("login")]
  public async Task<IActionResult> Login([FromBody] LoginDto dto)
  {
    var account = await _userManager.FindByEmailAsync(dto.Email);
    if (account == null)
      return Unauthorized("Invalid credentials");

    var passwordValid = await _userManager.CheckPasswordAsync(account, dto.Password);
    if (!passwordValid)
      return Unauthorized("Invalid credentials");

    var token = GenerateJwtToken(account);

    return Ok(token);
  }

  private async Task<AuthResponseDto> GenerateJwtToken(AccountModel account)
  {
    var tokenHandler = new JwtSecurityTokenHandler();
    var jwtKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured");
    var key = Encoding.UTF8.GetBytes(jwtKey);

    var claims = new List<Claim>
    {
      new Claim(ClaimTypes.NameIdentifier, account.Id.ToString()),
      new Claim(ClaimTypes.Email, account.Email ?? ""),
      new Claim("TenantId", account.TenantId.ToString())
    };

    var roles = await _userManager.GetRolesAsync(account);
    claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

    var tokenDescriptor = new SecurityTokenDescriptor
    {
      Subject = new ClaimsIdentity(claims),
      Expires = DateTime.UtcNow.AddHours(8),
      SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
      Issuer = _configuration["Jwt:Issuer"],
      Audience = _configuration["Jwt:Audience"]
    };

    var token = tokenHandler.CreateToken(tokenDescriptor);

    return new AuthResponseDto
    {
      Token = tokenHandler.WriteToken(token),
      ExpiresAt = token.ValidTo
    };
  }
}