using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OurBigDay.Api.Data;
using OurBigDay.Api.DTOs;
using OurBigDay.Api.Entities;

namespace OurBigDay.Api.Controllers;

[Route("api/wedding")]
[ApiController]
[Authorize(Roles = "Admin,Family")]
public class WeddingItemsController : ControllerBase
{
    private readonly AppDbContext _context;

    public WeddingItemsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("days/{dayId:int}/items")]
    public async Task<ActionResult<IEnumerable<WeddingItemDto>>> GetItemsByDay(int dayId, CancellationToken cancellationToken = default)
    {
        var exists = await _context.WeddingDays.AnyAsync(d => d.Id == dayId, cancellationToken);
        if (!exists)
            return NotFound("Day not found.");

        var items = await _context.WeddingItems
            .Where(wi => wi.DayId == dayId)
            .Include(wi => wi.Category)
            .OrderBy(wi => wi.Category.Name)
            .ThenBy(wi => wi.Name)
            .Select(wi => ToDto(wi))
            .ToListAsync(cancellationToken);
        return Ok(items);
    }

    [HttpGet("days/{dayId:int}/categories/{categoryId:int}/items")]
    public async Task<ActionResult<IEnumerable<WeddingItemDto>>> GetItemsByDayAndCategory(int dayId, int categoryId, CancellationToken cancellationToken = default)
    {
        var items = await _context.WeddingItems
            .Where(wi => wi.DayId == dayId && wi.CategoryId == categoryId)
            .Include(wi => wi.Category)
            .OrderBy(wi => wi.Name)
            .Select(wi => ToDto(wi))
            .ToListAsync(cancellationToken);
        return Ok(items);
    }

    [HttpPost("items")]
    public async Task<ActionResult<WeddingItemDto>> CreateItem([FromBody] CreateWeddingItemRequest request, CancellationToken cancellationToken = default)
    {
        var dayExists = await _context.WeddingDays.AnyAsync(d => d.Id == request.DayId, cancellationToken);
        if (!dayExists)
            return BadRequest("Day not found.");
        var categoryExists = await _context.Categories.AnyAsync(c => c.Id == request.CategoryId, cancellationToken);
        if (!categoryExists)
            return BadRequest("Category not found.");

        var item = new WeddingItem
        {
            DayId = request.DayId,
            CategoryId = request.CategoryId,
            Name = request.Name,
            VendorName = request.VendorName,
            Notes = request.Notes,
            EstimatedCost = request.EstimatedCost,
            DepositPaid = request.DepositPaid,
            OutstandingFees = request.OutstandingFees,
            PercentageComplete = request.PercentageComplete,
            AttributesJson = request.AttributesJson,
        };
        _context.WeddingItems.Add(item);
        await _context.SaveChangesAsync(cancellationToken);
        await _context.Entry(item).Reference(wi => wi.Category).LoadAsync(cancellationToken);
        return CreatedAtAction(nameof(GetItemById), new { id = item.Id }, ToDto(item));
    }

    [HttpGet("items/{id:int}")]
    public async Task<ActionResult<WeddingItemDto>> GetItemById(int id, CancellationToken cancellationToken = default)
    {
        var item = await _context.WeddingItems
            .Include(wi => wi.Category)
            .FirstOrDefaultAsync(wi => wi.Id == id, cancellationToken);
        if (item == null)
            return NotFound();
        return Ok(ToDto(item));
    }

    [HttpPut("items/{id:int}")]
    public async Task<ActionResult<WeddingItemDto>> UpdateItem(int id, [FromBody] UpdateWeddingItemRequest request, CancellationToken cancellationToken = default)
    {
        var item = await _context.WeddingItems.Include(wi => wi.Category).FirstOrDefaultAsync(wi => wi.Id == id, cancellationToken);
        if (item == null)
            return NotFound();

        item.Name = request.Name;
        item.VendorName = request.VendorName;
        item.Notes = request.Notes;
        item.EstimatedCost = request.EstimatedCost;
        item.DepositPaid = request.DepositPaid;
        item.OutstandingFees = request.OutstandingFees;
        item.PercentageComplete = request.PercentageComplete;
        item.AttributesJson = request.AttributesJson;
        await _context.SaveChangesAsync(cancellationToken);
        return Ok(ToDto(item));
    }

    [HttpDelete("items/{id:int}")]
    public async Task<ActionResult> DeleteItem(int id, CancellationToken cancellationToken = default)
    {
        var item = await _context.WeddingItems.FindAsync(new object[] { id }, cancellationToken);
        if (item == null)
            return NotFound();
        _context.WeddingItems.Remove(item);
        await _context.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    private static WeddingItemDto ToDto(WeddingItem wi)
        => new WeddingItemDto(
            wi.Id,
            wi.DayId,
            wi.CategoryId,
            wi.Category.Name,
            wi.Name,
            wi.VendorName,
            wi.Notes,
            wi.EstimatedCost,
            wi.DepositPaid,
            wi.OutstandingFees,
            wi.PercentageComplete,
            wi.AttributesJson
        );
}
