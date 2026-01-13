using ModernApi.Models;
using ModernApi.Exceptions;
using Microsoft.EntityFrameworkCore;
using ModernApi.Data;
using ModernApi.DTOs;
using ModernApi.Abstractions.Caching;

namespace ModernApi.Services;

public class ItemService : IItemService
{
    private readonly AppDbContext _db;
    private readonly IAppCache _cache;
    private readonly IListVersionStore _listVersionStore;
    
    public ItemService(AppDbContext context, IAppCache cache, IListVersionStore listVersionStore)
    {
        _db = context;
        _cache = cache;
        _listVersionStore = listVersionStore;
    }
    private async Task<long> GetListVersionAsync(CancellationToken ct)
    {
        return await _listVersionStore.GetAsync(ct);
    }

    public async Task<PagedResult<ItemResponse>> GetItemsAsync(int page, int pageSize, string filter, CancellationToken ct)
    {
        // safety limits
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;
        if (pageSize > 100) pageSize = 100;

        var version = await GetListVersionAsync(ct);
        
        filter = (filter ?? string.Empty).Trim().ToLowerInvariant();
        var cacheKey = $"modernapi:items:list:v={version}:p={page}:ps={pageSize}:q={filter}";

        var cached = await _cache.GetAsync<PagedResult<ItemResponse>>(cacheKey, ct);
        if (cached != null)
        {
            return cached;
        }

        var query = _db.Items.AsNoTracking();

       
        if (!string.IsNullOrEmpty(filter))
        {
            query = query.Where(x => EF.Functions.ILike(x.Name, $"%{filter}%"));
        }

        var total = await query.CountAsync(ct);
        var items = await query
                    .OrderBy(x => x.Id)                
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(x => new ItemResponse(x.Id, x.Name))
                    .ToListAsync(ct);
                    
        var result = new PagedResult<ItemResponse>(items, total, page, pageSize);

        await _cache.SetAsync(cacheKey, result, TimeSpan.FromSeconds(120), ct);
        return result;
    }

    public async Task<ItemResponse> GetItemByIdAsync(int id, CancellationToken ct = default)
    {
        var cacheKey = $"item:{id}";

        var cached = await _cache.GetAsync<ItemResponse>(cacheKey, ct);
        if (cached != null)
            return cached;

        var item = await _db.Items
        .AsNoTracking()
        .Where(x => x.Id == id)
        .Select(x => new ItemResponse(x.Id, x.Name))
        .FirstOrDefaultAsync(ct); 

        if (item == null)
        {
            throw new NotFoundException("Item not found.");
        }

        await _cache.SetAsync(cacheKey, item,TimeSpan.FromSeconds(60), ct); 
        
        return item;
    }

    public async Task<ItemResponse> AddItemAsync(string newItem, CancellationToken ct = default)
    {
        var item = new Item { Name = newItem };
        await _db.Items.AddAsync(item, ct);
        await _db.SaveChangesAsync(ct);
        //invalidate cache
        await _listVersionStore.BumpAsync(ct);

        return new ItemResponse(item.Id, item.Name);
    }

    public async Task<ItemResponse> UpdateItemAsync(int id, string name, CancellationToken ct = default)
    {
        var item = await _db.Items.FirstOrDefaultAsync(x => x.Id == id, ct);
        if(item == null)
        {
            throw new NotFoundException("Item not found.");
        }

        item.Name = name;
        await _db.SaveChangesAsync(ct);

        await InvalidateCache(id, ct);

        return new ItemResponse(item.Id, item.Name);
    }

    public async Task<bool> DeleteItemAsync(int id, CancellationToken ct = default)
    {
        var item = await _db.Items.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (item == null)
        {
            return false;
        }

        _db.Items.Remove(item);
        await _db.SaveChangesAsync(ct);

        await InvalidateCache(id, ct);

        return true;
    }

    private async Task InvalidateCache(int id, CancellationToken ct = default)
    {
        await _cache.RemoveAsync($"item:{id}", ct);
        await _listVersionStore.BumpAsync(ct);
    }
}
