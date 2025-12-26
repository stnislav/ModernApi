using Microsoft.AspNetCore.Http.HttpResults;
using ModernApi.Models;
using ModernApi.Services;
using ModernApi.Exceptions;
using Microsoft.EntityFrameworkCore;
using ModernApi.Data;
using ModernApi.DTOs;
using Microsoft.Extensions.Caching.Memory;

namespace ModernApi.Services;
public class ItemService : IItemService
{
    private readonly AppDbContext _db;
    readonly IMemoryCache _cache;
    private const string ListVersionKey = "items:list:version";

    public ItemService(AppDbContext context, IMemoryCache cache)
    {
        _db = context;
        _cache = cache;
    }

    private int GetListVersion()
    {
        return _cache.GetOrCreate(ListVersionKey, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(6);
            return 1;
        });
    }
    private void BumpListVersion()
    {
        var current = GetListVersion();
        _cache.Set(ListVersionKey, current + 1);
    }

    public async Task<PagedResult<ItemResponse>> GetItemsAsync(int page, int pageSize, string filter)
    {
        // safety limits
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;
        if (pageSize > 100) pageSize = 100;

        var version = GetListVersion();
        var cacheKey = $"items:list:v={version}:p={page}:ps={pageSize}:q={filter}";

        if (_cache.TryGetValue<PagedResult<ItemResponse>>(cacheKey, out var cached))
        {
            return cached;
        }

        var query = _db.Items.AsNoTracking();

        if (!string.IsNullOrEmpty(filter))
        {
            query = query.Where(x => x.Name.Contains(filter));
        }

        var total = await query.CountAsync();
        var items = await query
                    .OrderBy(x => x.Id)                
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(x => new ItemResponse(x.Id, x.Name))
                    .ToListAsync();
                    
        var result = new PagedResult<ItemResponse>(items, total, page, pageSize);

        _cache.Set(cacheKey, result, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(20)
        });

        return result;
    }

    public async Task<Item> GetItemByIdAsync(int id)
    {
        var cacheKey = $"item:{id}";

        if (_cache.TryGetValue<Item>(cacheKey, out var cached))
            return cached;

        var item = await _db.Items.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

        if (item == null)
        {
            throw new NotFoundException("Item not found.");
        }

        _cache.Set(cacheKey, item, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60),
            SlidingExpiration = TimeSpan.FromSeconds(30),
            Size = 1
        }); 
        
        return item;
    }

    public async Task<Item> AddItemAsync(string newItem)
    {
        var item = new Item { Name = newItem };
        _db.Items.Add(item);
        await _db.SaveChangesAsync();
        //invalidate cache
        BumpListVersion();

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

        InvalidateCache(id);
        
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

        InvalidateCache(id);

        return true;
    }

    private void InvalidateCache(int id)
    {
         _cache.Remove($"item:{id}");
        BumpListVersion();
    }
}
