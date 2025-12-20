using Microsoft.AspNetCore.Http.HttpResults;
using ModernApi.Models;
using ModernApi.Services;
using ModernApi.Exceptions;
using Microsoft.EntityFrameworkCore;
using ModernApi.Data;
using ModernApi.DTOs;

namespace ModernApi.Services;
public class ItemService : IItemService
{
    private readonly AppDbContext _db;
    public ItemService(AppDbContext context)
    {
        _db = context;
    }

    public async Task<PagedResult<ItemResponse>> GetItemsAsync(int page, int pageSize, string filter)
    {
        // safety limits
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;
        if (pageSize > 100) pageSize = 100;

        var query = _db.Items.AsNoTracking();

        if (!string.IsNullOrEmpty(filter))
        {
            query = query.Where(x => x.Name.Contains(filter));
        }

        var items = await query
                    .OrderBy(x => x.Id)                
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(x => new ItemResponse(x.Id, x.Name))
                    .ToListAsync();
                    
        var total = await query.CountAsync();

        return new PagedResult<ItemResponse>(items, page, pageSize, total);
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
        var item = await _db.Items.FirstOrDefaultAsync(x => x.Id == id);
        if(item == null)
        {
            throw new NotFoundException("Item not found.");
        }

        item.Name = name;
        await _db.SaveChangesAsync();
        return item;
    }

    public async Task<bool> DeleteItemAsync(int id)
    {
        var item = await _db.Items.FirstOrDefaultAsync(x => x.Id == id);
        if (item == null)
        {
            return false;
        }

        _db.Items.Remove(item);
        await _db.SaveChangesAsync();
        return true;
    }
}
