namespace OurBigDay.Api.DTOs;

public record PaymentDto(int Id, decimal Amount, string PaidDate, string? Note);

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
    decimal? PercentageComplete,
    List<string> Images,
    List<PaymentDto> Payments
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

public record AddPaymentRequest(decimal Amount, string PaidDate, string? Note);

public record AddImageRequest(string ImageBase64);
