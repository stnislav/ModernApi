using ModernApi.Models;
using System.Threading.Tasks;
using ModernApi.DTOs;

namespace ModernApi.Services;
public interface IItemService
{
    Task<PagedResult<ItemResponse>> GetItemsAsync(int page, int pageSize, string filter);
    Task<Item> GetItemByIdAsync(int id);
    Task<Item> AddItemAsync(string item);
    Task<Item> UpdateItemAsync(int id, string name);
    Task<bool> DeleteItemAsync(int id);
}
