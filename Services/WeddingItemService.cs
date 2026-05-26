using Microsoft.EntityFrameworkCore;
using OurBigDay.Api.Data;
using OurBigDay.Api.DTOs;
using OurBigDay.Api.Entities;
using OurBigDay.Api.Exceptions;
using OurBigDay.Api.Mappers;

namespace OurBigDay.Api.Services;

public class WeddingItemService : IWeddingItemService
{
    private readonly AppDbContext _context;

    public WeddingItemService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<WeddingItemDto>> GetByDayAsync(int dayId, CancellationToken ct = default)
    {
        if (!await _context.WeddingDays.AnyAsync(d => d.Id == dayId, ct))
            throw new NotFoundException($"Day {dayId} not found.");

        return await _context.WeddingItems
            .Where(wi => wi.DayId == dayId)
            .Include(wi => wi.Category)
            .OrderBy(wi => wi.Category.Name)
            .ThenBy(wi => wi.Name)
            .Select(wi => WeddingItemMapper.ToDto(wi))
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<WeddingItemDto>> GetByDayAndCategoryAsync(int dayId, int categoryId, CancellationToken ct = default) =>
        await _context.WeddingItems
            .Where(wi => wi.DayId == dayId && wi.CategoryId == categoryId)
            .Include(wi => wi.Category)
            .OrderBy(wi => wi.Name)
            .Select(wi => WeddingItemMapper.ToDto(wi))
            .ToListAsync(ct);

    public async Task<WeddingItemDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var item = await _context.WeddingItems
            .Include(wi => wi.Category)
            .FirstOrDefaultAsync(wi => wi.Id == id, ct);
        return item == null ? null : WeddingItemMapper.ToDto(item);
    }

    public async Task<WeddingItemDto> CreateAsync(CreateWeddingItemRequest request, CancellationToken ct = default)
    {
        if (!await _context.WeddingDays.AnyAsync(d => d.Id == request.DayId, ct))
            throw new NotFoundException("Day not found.");
        if (!await _context.Categories.AnyAsync(c => c.Id == request.CategoryId, ct))
            throw new NotFoundException("Category not found.");

        var item = new WeddingItem
        {
            DayId = request.DayId,
            CategoryId = request.CategoryId,
            Name = request.Name,
            VendorName = request.VendorName,
            Notes = request.Notes,
            EstimatedCost = request.EstimatedCost,
            DepositPaid = request.DepositPaid,
            OutstandingFees = CalculateOutstanding(request.EstimatedCost, request.DepositPaid),
            PercentageComplete = request.PercentageComplete,
        };

        _context.WeddingItems.Add(item);
        await _context.SaveChangesAsync(ct);
        await _context.Entry(item).Reference(wi => wi.Category).LoadAsync(ct);
        return WeddingItemMapper.ToDto(item);
    }

    public async Task<WeddingItemDto> UpdateAsync(int id, UpdateWeddingItemRequest request, CancellationToken ct = default)
    {
        var item = await _context.WeddingItems
            .Include(wi => wi.Category)
            .FirstOrDefaultAsync(wi => wi.Id == id, ct)
            ?? throw new NotFoundException("Item not found.");

        item.Name = request.Name;
        item.VendorName = request.VendorName;
        item.Notes = request.Notes;
        item.EstimatedCost = request.EstimatedCost;
        item.DepositPaid = request.DepositPaid;
        item.OutstandingFees = CalculateOutstanding(request.EstimatedCost, request.DepositPaid);
        item.PercentageComplete = request.PercentageComplete;

        await _context.SaveChangesAsync(ct);
        return WeddingItemMapper.ToDto(item);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var item = await _context.WeddingItems.FindAsync(new object[] { id }, ct)
            ?? throw new NotFoundException("Item not found.");

        _context.WeddingItems.Remove(item);
        await _context.SaveChangesAsync(ct);
    }

    private static decimal? CalculateOutstanding(decimal? estimated, decimal? deposit) =>
        (estimated.HasValue || deposit.HasValue)
            ? Math.Max(0, (estimated ?? 0) - (deposit ?? 0))
            : null;
}
