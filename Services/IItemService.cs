using ModernApi.Models;
using System.Threading.Tasks;

namespace ModernApi.Services;
public interface IItemService
{
    Task<IEnumerable<Item>> GetAllItemsAsync();
    Task<Item> GetItemByIdAsync(int id);
    Task<Item> AddItemAsync(string item);
    Task<Item> UpdateItemAsync(int id, string name);
    Task DeleteItemAsync(int id);
}
