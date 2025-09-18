using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Multi_Tenant_API.Models;

namespace Multi_Tenant_API.Dtos;

public class RegisterProjectDto
{
  [Required(ErrorMessage = "Name is required.")]
  [StringLength(100, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 100 characters.")]
  public required string Name { get; set; }

  [Required(ErrorMessage = "Plan is required.")]
  [StringLength(200, ErrorMessage = "Description must be between 3 and 100 characters.")]
  public required string Description { get; set; }
}

public class UpdateProjectDto
{
  [StringLength(100, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 100 characters.")]
  public string? Name { get; set; }

  [StringLength(200, ErrorMessage = "Description must be between 3 and 100 characters.")]
  public string? Description { get; set; }
}