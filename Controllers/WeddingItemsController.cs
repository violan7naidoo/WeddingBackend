using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OurBigDay.Api.DTOs;
using OurBigDay.Api.Exceptions;
using OurBigDay.Api.Services;

namespace OurBigDay.Api.Controllers;

[Route("api/wedding")]
[ApiController]
[Authorize(Roles = "Admin,Family")]
public class WeddingItemsController : ApiControllerBase
{
    private readonly IWeddingItemService _itemService;

    public WeddingItemsController(IWeddingItemService itemService)
    {
        _itemService = itemService;
    }

    [HttpGet("days/{dayId:int}/items")]
    public async Task<ActionResult<IEnumerable<WeddingItemDto>>> GetItemsByDay(int dayId, CancellationToken ct = default)
    {
        try { return Ok(await _itemService.GetByDayAsync(dayId, ct)); }
        catch (BusinessException ex) { return HandleException(ex); }
    }

    [HttpGet("days/{dayId:int}/categories/{categoryId:int}/items")]
    public async Task<ActionResult<IEnumerable<WeddingItemDto>>> GetItemsByDayAndCategory(int dayId, int categoryId, CancellationToken ct = default) =>
        Ok(await _itemService.GetByDayAndCategoryAsync(dayId, categoryId, ct));

    [HttpGet("items/{id:int}")]
    public async Task<ActionResult<WeddingItemDto>> GetItemById(int id, CancellationToken ct = default)
    {
        var item = await _itemService.GetByIdAsync(id, ct);
        return item == null ? NotFound() : Ok(item);
    }

    [HttpPost("items")]
    public async Task<ActionResult<WeddingItemDto>> CreateItem([FromBody] CreateWeddingItemRequest request, CancellationToken ct = default)
    {
        try
        {
            var item = await _itemService.CreateAsync(request, ct);
            return CreatedAtAction(nameof(GetItemById), new { id = item.Id }, item);
        }
        catch (BusinessException ex) { return HandleException(ex); }
    }

    [HttpPut("items/{id:int}")]
    public async Task<ActionResult<WeddingItemDto>> UpdateItem(int id, [FromBody] UpdateWeddingItemRequest request, CancellationToken ct = default)
    {
        try { return Ok(await _itemService.UpdateAsync(id, request, ct)); }
        catch (BusinessException ex) { return HandleException(ex); }
    }

    [HttpDelete("items/{id:int}")]
    public async Task<ActionResult> DeleteItem(int id, CancellationToken ct = default)
    {
        try { await _itemService.DeleteAsync(id, ct); return NoContent(); }
        catch (BusinessException ex) { return HandleException(ex); }
    }
}
