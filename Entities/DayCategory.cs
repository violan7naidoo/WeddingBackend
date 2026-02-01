namespace OurBigDay.Api.Entities;

public class DayCategory
{
    public int DayId { get; set; }
    public WeddingDay WeddingDay { get; set; } = null!;

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public int DisplayOrder { get; set; }
}
