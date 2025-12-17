using ModernApi.Models;

namespace ModernApi.Services;
public interface IItemService
{
    IEnumerable<Item> GetAllItems();
    Item GetItemById(int id);
    Item AddItem(string item);

    Item UpdateItem(int id, string name);
}
