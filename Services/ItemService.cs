using Microsoft.AspNetCore.Http.HttpResults;
using ModernApi.Models;
using ModernApi.Services;
using ModernApi.Exceptions;
using Microsoft.EntityFrameworkCore;
using ModernApi.Data;

namespace ModernApi.Services;
public class ItemService : IItemService
{
    private readonly AppDbContext _db;
    public ItemService(AppDbContext context)
    {
        _db = context;
    }

    public async Task<IEnumerable<Item>> GetAllItemsAsync()
    {
        return await _db.Items.AsNoTracking().OrderBy(x => x.Id).ToListAsync();
    }

    public async Task<Item> GetItemByIdAsync(int id)
    {
        var item = await _db.Items.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);


        if (item == null)
        {
            throw new NotFoundException("Item not found.");
        }

        return item;
    }

    public async Task<Item> AddItemAsync(string newItem)
    {
        var item = new Item { Name = newItem };
        _db.Items.Add(item);
        await _db.SaveChangesAsync();
        return item;
    }

    public async Task<Item> UpdateItemAsync(int id, string name)
    {
        var item = _db.Items.FirstOrDefault(x => x.Id == id);
        if(item == null)
        {
            throw new NotFoundException("Item not found.");
        }

        item.Name = name;
        await _db.SaveChangesAsync();
        return item;
    }

    public async Task DeleteItemAsync(int id)
    {
        var item = _db.Items.FirstOrDefault(x => x.Id == id);
        if (item == null)
        {
            throw new NotFoundException("Item not found.");
        }

        _db.Items.Remove(item);
        await _db.SaveChangesAsync();
    }
}
