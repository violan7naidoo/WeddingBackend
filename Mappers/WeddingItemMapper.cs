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
        item.PercentageComplete
    );
}
