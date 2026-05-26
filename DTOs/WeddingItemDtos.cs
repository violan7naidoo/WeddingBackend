namespace OurBigDay.Api.DTOs;

public record WeddingItemDto(
    int Id,
    int DayId,
    int CategoryId,
    string CategoryName,
    string Name,
    string? VendorName,
    string? Notes,
    decimal? EstimatedCost,
    decimal? DepositPaid,
    decimal? OutstandingFees,
    decimal? PercentageComplete
);

public record CreateWeddingItemRequest(
    int DayId,
    int CategoryId,
    string Name,
    string? VendorName,
    string? Notes,
    decimal? EstimatedCost,
    decimal? DepositPaid,
    decimal? PercentageComplete
);

public record UpdateWeddingItemRequest(
    string Name,
    string? VendorName,
    string? Notes,
    decimal? EstimatedCost,
    decimal? DepositPaid,
    decimal? PercentageComplete
);
