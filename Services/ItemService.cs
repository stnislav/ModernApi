using Microsoft.AspNetCore.Http.HttpResults;
using ModernApi.Models;
using ModernApi.Services;
using ModernApi.Exceptions;

namespace ModernApi.Services;
public class ItemService : IItemService
{
    private readonly List<Item> _items = new() { 
        new Item { Id = 1, Name = "Item1" }, 
        new Item { Id = 2, Name = "Item2" }, 
        new Item { Id = 3, Name = "Item3" } };

    public IEnumerable<Item> GetAllItems()
    {
        Console.WriteLine("Fetching all items.");
        return _items;
    }

    public Item GetItemById(int id)
    {
        if (id < 1 || id > _items.Count)
        {
            throw new NotFoundException("Item not found.");
            // throw new ArgumentOutOfRangeException(nameof(id), "Item not found.");
        }

        return _items[id - 1];
    }

    public Item AddItem(string newItem)
    {
        var item = new Item { Id = _items.Count + 1, Name = newItem };
        _items.Add(item);
        return item;
    }

    public Item UpdateItem(int id, string name)
    {
        var item = GetItemById(id);
        item.Name = name;
        return item;
    }
}
