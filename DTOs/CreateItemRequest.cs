namespace ModernApi.DTOs;
using System.ComponentModel.DataAnnotations;
public record CreateItemRequest(
    [Required]
    [MinLength(1)]
    string Name
);