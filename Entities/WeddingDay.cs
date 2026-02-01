namespace OurBigDay.Api.Entities;

public class WeddingDay
{
    public int Id { get; set; }
    public int DayNumber { get; set; }
    public string ThemeName { get; set; } = string.Empty;
    public DateOnly Date { get; set; }

    public ICollection<DayCategory> DayCategories { get; set; } = new List<DayCategory>();
    public ICollection<WeddingItem> WeddingItems { get; set; } = new List<WeddingItem>();
}
