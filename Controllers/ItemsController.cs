using Microsoft.AspNetCore.Mvc;
using ModernApi.Services;
using ModernApi.DTOs;

namespace ModernApi.Controllers;


[ApiController]
[Route("api/items")]
public class ItemsController : ControllerBase
{
    private readonly IItemService itemService;

    public ItemsController(IItemService itemService)
    {
        this.itemService = itemService;
    }
    // GET api/items
    [HttpGet]
    public IActionResult GetItems()
    {
        var items = itemService.GetAllItems();
        var response = items.Select(x => new ItemResponse(x.Id, x.Name));
        return Ok(response);
    }

    // GET api/items/{id}
    [HttpGet("{id}")]
    public IActionResult GetItem(int id)
    {
        var item = itemService.GetItemById(id);
        return Ok(new ItemResponse(item.Id, item.Name));
    }

    // POST api/items
    [HttpPost]
    public IActionResult CreateItem(CreateItemRequest newItem)
    {
        var item = itemService.AddItem(newItem.Name);
        return CreatedAtAction(nameof(GetItem), new { id = item.Id }, item);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateItem(UpdateItemRequest request)
    {
        var item = itemService.UpdateItem(request.Id, request.Name);
        return Ok(item);
    }
}
