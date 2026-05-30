using System.Text.Json;
using OurBigDay.Api.DTOs;
using OurBigDay.Api.Entities;

namespace OurBigDay.Api.Mappers;

public static class WeddingItemMapper
{
    public static WeddingItemDto ToDto(WeddingItem item) => new(
        item.Id,
        item.DayId,
        item.CategoryId,
        item.Category.Name,
        item.Name,
        item.VendorName,
        item.Notes,
        item.EstimatedCost,
        item.DepositPaid,
        item.OutstandingFees,
        item.PercentageComplete,
        ParseImages(item.ImagesJson),
        item.Payments
            .OrderBy(p => p.PaidDate)
            .Select(p => new PaymentDto(p.Id, p.Amount, p.PaidDate.ToString("yyyy-MM-dd"), p.Note))
            .ToList()
    );

    private static List<string> ParseImages(string? json)
    {
        if (string.IsNullOrEmpty(json)) return [];
        try { return JsonSerializer.Deserialize<List<string>>(json) ?? []; }
        catch { return []; }
    }
}
