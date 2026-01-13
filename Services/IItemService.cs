using ModernApi.Models;
using System.Threading.Tasks;
using ModernApi.DTOs;

namespace ModernApi.Services;
public interface IItemService
{
    Task<PagedResult<ItemResponse>> GetItemsAsync(int page, int pageSize, string filter, CancellationToken ct = default);
    Task<ItemResponse> GetItemByIdAsync(int id, CancellationToken ct = default);
    Task<ItemResponse> AddItemAsync(string item, CancellationToken ct = default);
    Task<ItemResponse> UpdateItemAsync(int id, string name, CancellationToken ct = default);
    Task<bool> DeleteItemAsync(int id, CancellationToken ct = default);
}
