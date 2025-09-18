using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Multi_Tenant_API.Models;

namespace Multi_Tenant_API.Dtos;

public class RegisterTaskDto
{
  [Required(ErrorMessage = "Name is required.")]
  [StringLength(100, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 100 characters.")]
  public required string Title { get; set; }

  [Required(ErrorMessage = "Name is required.")]
  [StringLength(200, ErrorMessage = "Description cannot exceed 200 characters.")]
  public required string Description { get; set; }

  [Required(ErrorMessage = "DueDate is required.")]
  public required DateTime DueDate { get; set; }
}

public class UpdateTaskDto
{
  [StringLength(100, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 100 characters.")]
  public string? Title { get; set; }

  [StringLength(200, ErrorMessage = "Description cannot exceed 200 characters.")]
  public string? Description { get; set; }
  public DateTime? DueDate { get; set; }

  [EnumDataType(typeof(Status), ErrorMessage = "Status must be Pending, InProgress, Blocked, Completed or Cancelled.")]
  [JsonConverter(typeof(JsonStringEnumConverter))]
  public Status? Status { get; set; }
}