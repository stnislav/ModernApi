using Microsoft.AspNetCore.Mvc;
using ModernApi.Services;
using ModernApi.DTOs;

namespace ModernApi.Controllers;


[ApiController]
[Route("api/items")]
public class ItemsController : ControllerBase
{
    private readonly IItemService _itemService;

    public ItemsController(IItemService itemService)
    {
        _itemService = itemService;
    }
    
    /// <summary>Returns all items.</summary>
    [HttpGet]
    public async Task<IActionResult> GetItems([FromQuery] GetItemsRequest request, CancellationToken ct = default)
    {
        var pagedDataResult =  await _itemService.GetItemsAsync(request.Page, request.PageSize, request.Search ?? string.Empty, ct);
        return Ok(pagedDataResult);
    }

    // GET api/items/{id}
    /// <summary>Returns an item by its ID.</summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetItem(int id, CancellationToken ct = default)
    {
        var item = await _itemService.GetItemByIdAsync(id, ct);
        if(item == null)
        {
            return NotFound();
        }

        return Ok(new ItemResponse(item.Id, item.Name));
    }

    // POST api/items
    /// <summary>Creates a new item.</summary>
    [HttpPost]
    public async Task<IActionResult> CreateItem(CreateItemRequest newItem, CancellationToken ct = default)
    {
        var item = await _itemService.AddItemAsync(newItem.Name, ct);
        return CreatedAtAction(nameof(GetItem), new { id = item.Id }, new ItemResponse(item.Id, item.Name));
    }

    /// <summary>Updates an existing item.</summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateItem(int id, UpdateItemRequest request, CancellationToken ct = default)
    {
        var item = await _itemService.UpdateItemAsync(id, request.Name, ct);
        if(item == null)
        {
            return NotFound();
        }
        
       return Ok(new ItemResponse(item.Id, item.Name));
    }

    /// <summary>Deletes an item by its ID.</summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteItem(int id, CancellationToken ct = default)
    {
        var deleted = await _itemService.DeleteItemAsync(id, ct);
        if (!deleted) return NotFound();
        return NoContent();
    }
}
