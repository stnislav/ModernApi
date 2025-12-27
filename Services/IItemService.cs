using ModernApi.Models;
using System.Threading.Tasks;
using ModernApi.DTOs;

namespace ModernApi.Services;
public interface IItemService
{
    Task<PagedResult<ItemResponse>> GetItemsAsync(int page, int pageSize, string filter);
    Task<ItemResponse> GetItemByIdAsync(int id);
    Task<ItemResponse> AddItemAsync(string item);
    Task<ItemResponse> UpdateItemAsync(int id, string name);
    Task<bool> DeleteItemAsync(int id);
}
