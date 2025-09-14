using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Multi_Tenant_API.Models;

namespace Multi_Tenant_API.Dtos;

public class TenantDto
{
  [Required(ErrorMessage = "Name is required.")]
  [StringLength(100, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 100 characters.")]
  public string Name { get; set; } = null!;

  [Required(ErrorMessage = "Plan is required.")]
  [EnumDataType(typeof(Plan), ErrorMessage = "Plan must be Free, Standart, or Premium.")]
  [JsonConverter(typeof(JsonStringEnumConverter))]
  public Plan Plan { get; set; }
}