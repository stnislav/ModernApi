namespace ModernApi.DTOs;
public record ItemsRequest(int PageNumber, int PageSize, string? Filter);