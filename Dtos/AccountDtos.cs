using System.ComponentModel.DataAnnotations;

namespace Multi_Tenant_API.Dtos;

public class RegisterAccountDto
{
  [Required(ErrorMessage = "UserName is required.")]
  public required string UserName { get; set; }

  [Required(ErrorMessage = "Email is required.")]
  public required string Email { get; set; }

  [Required(ErrorMessage = "Password is required.")]
  public required string Password { get; set; }
  public Guid TenantId { get; set; }
}

public class LoginDto
{
  [Required(ErrorMessage = "Email is required.")]
  public required string Email { get; set; }

  [Required(ErrorMessage = "Password is required.")]
  public required string Password { get; set; }
}

public class AuthResponseDto
{
  public string Token { get; set; } = null!;
  public DateTime ExpiresAt { get; set; }
}