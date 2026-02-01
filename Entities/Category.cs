namespace OurBigDay.Api.Entities;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<DayCategory> DayCategories { get; set; } = new List<DayCategory>();
    public ICollection<WeddingItem> WeddingItems { get; set; } = new List<WeddingItem>();
}
