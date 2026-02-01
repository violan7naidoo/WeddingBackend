namespace OurBigDay.Api.Entities;

public class WeddingItem
{
    public int Id { get; set; }
    public int DayId { get; set; }
    public WeddingDay WeddingDay { get; set; } = null!;

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public string Name { get; set; } = string.Empty;
    public string? VendorName { get; set; }
    public string? Notes { get; set; }

    public decimal? EstimatedCost { get; set; }
    public decimal? DepositPaid { get; set; }
    public decimal? OutstandingFees { get; set; }
    public decimal? PercentageComplete { get; set; }

    public string? AttributesJson { get; set; }
}
