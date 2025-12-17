namespace ModernApi.DTOs;
using System.ComponentModel.DataAnnotations;
public record CreateItemRequest(
    [property: Required]
    [property: MinLength(1)]
    string Name
);