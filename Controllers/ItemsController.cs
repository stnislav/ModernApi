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
    public async Task<IActionResult> GetItems()
    {
        var items = await _itemService.GetAllItemsAsync();
        var response = items.Select(x => new ItemResponse(x.Id, x.Name));
        return Ok(response);
    }

    // GET api/items/{id}
    /// <summary>Returns an item by its ID.</summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetItem(int id)
    {
        var item = await _itemService.GetItemByIdAsync(id);
        return Ok(new ItemResponse(item.Id, item.Name));
    }

    // POST api/items
    /// <summary>Creates a new item.</summary>
    [HttpPost]
    public async Task<IActionResult> CreateItem(CreateItemRequest newItem)
    {
        var item = await _itemService.AddItemAsync(newItem.Name);
        return CreatedAtAction(nameof(GetItem), new { id = item.Id }, new ItemResponse(item.Id, item.Name));
    }

    /// <summary>Updates an existing item.</summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateItem(UpdateItemRequest request)
    {
        var item = await _itemService.UpdateItemAsync(request.Id, request.Name);
       return Ok(new ItemResponse(item.Id, item.Name));
    }

    /// <summary>Deletes an item by its ID.</summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteItem(int id)
    {
        await _itemService.DeleteItemAsync(id);
        return Ok();
    }
}
