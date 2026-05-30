namespace OurBigDay.Api.Entities;

public class WeddingItemPayment
{
    public int Id { get; set; }
    public int WeddingItemId { get; set; }
    public WeddingItem WeddingItem { get; set; } = null!;

    public decimal Amount { get; set; }
    public DateOnly PaidDate { get; set; }
    public string? Note { get; set; }
}
