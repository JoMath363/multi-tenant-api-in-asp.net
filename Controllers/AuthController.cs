using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
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
  private readonly IMapper _mapper;
  private readonly IConfiguration _configuration;

  public AuthController(
    UserManager<AccountModel> userManager,
    IMapper mapper,
    RoleManager<IdentityRole<Guid>> roleManager,
    IConfiguration configuration
  )
  {
    _userManager = userManager;
    _roleManager = roleManager;
    _mapper = mapper;
    _configuration = configuration;
  }

  [HttpPost("register")]
  public async Task<IActionResult> Register([FromBody] RegisterAccountDto dto)
  {
    var userExists = await _userManager.FindByEmailAsync(dto.Email);
    if (userExists != null)
      return BadRequest("User already exists");

    var user = _mapper.Map<AccountModel>(dto);

    var result = await _userManager.CreateAsync(user, dto.Password);
    if (!result.Succeeded)
      return BadRequest(result.Errors);

    if (!await _roleManager.RoleExistsAsync("User"))
      await _roleManager.CreateAsync(new IdentityRole<Guid>("User"));

    await _userManager.AddToRoleAsync(user, "User");

    return Ok("User created successfully");
  }

  [HttpPost("login")]
  public async Task<IActionResult> Login([FromBody] LoginDto dto)
  {
    var user = await _userManager.FindByEmailAsync(dto.Email);
    if (user == null)
      return Unauthorized("Invalid credentials");

    var passwordValid = await _userManager.CheckPasswordAsync(user, dto.Password);
    if (!passwordValid)
      return Unauthorized("Invalid credentials");

    var token = GenerateJwtToken(user);

    return Ok(token);
  }

  private async Task<AuthResponseDto> GenerateJwtToken(AccountModel user)
  {
    var tokenHandler = new JwtSecurityTokenHandler();
    var jwtKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured");
    var key = Encoding.UTF8.GetBytes(jwtKey);

    var claims = new List<Claim>
    {
      new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
      new Claim(ClaimTypes.Email, user.Email ?? ""),
      new Claim("TenantId", user.TenantId.ToString())
    };

    var roles = await _userManager.GetRolesAsync(user);
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