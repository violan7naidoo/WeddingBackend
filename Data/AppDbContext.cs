using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OurBigDay.Api.Entities;

namespace OurBigDay.Api.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<WeddingDay> WeddingDays { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<DayCategory> DayCategories { get; set; }
    public DbSet<WeddingItem> WeddingItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<WeddingDay>().HasData(
            new WeddingDay { Id = 4, DayNumber = 1, ThemeName = "Mendhi", Date = new DateOnly(2026, 12, 16) },
            new WeddingDay { Id = 5, DayNumber = 2, ThemeName = "Haldi", Date = new DateOnly(2026, 12, 17) },
            new WeddingDay { Id = 1, DayNumber = 3, ThemeName = "Hindu wedding and night before/Sangeet", Date = new DateOnly(2026, 12, 18) },
            new WeddingDay { Id = 3, DayNumber = 4, ThemeName = "Christian wedding and reception", Date = new DateOnly(2026, 12, 19) }
        );

        modelBuilder.Entity<DayCategory>()
            .HasKey(dc => new { dc.DayId, dc.CategoryId });
        modelBuilder.Entity<DayCategory>()
            .HasOne(dc => dc.WeddingDay)
            .WithMany(d => d.DayCategories)
            .HasForeignKey(dc => dc.DayId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<DayCategory>()
            .HasOne(dc => dc.Category)
            .WithMany(c => c.DayCategories)
            .HasForeignKey(dc => dc.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<WeddingItem>()
            .HasOne(wi => wi.WeddingDay)
            .WithMany(d => d.WeddingItems)
            .HasForeignKey(wi => wi.DayId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<WeddingItem>()
            .HasOne(wi => wi.Category)
            .WithMany(c => c.WeddingItems)
            .HasForeignKey(wi => wi.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<WeddingItem>()
            .Property(wi => wi.EstimatedCost)
            .HasPrecision(18, 2);
        modelBuilder.Entity<WeddingItem>()
            .Property(wi => wi.DepositPaid)
            .HasPrecision(18, 2);
        modelBuilder.Entity<WeddingItem>()
            .Property(wi => wi.OutstandingFees)
            .HasPrecision(18, 2);
        modelBuilder.Entity<WeddingItem>()
            .Property(wi => wi.PercentageComplete)
            .HasPrecision(5, 2);

        SeedCategoriesAndDayCategories(modelBuilder);
    }

    private static void SeedCategoriesAndDayCategories(ModelBuilder modelBuilder)
    {
        var categories = new[]
        {
            new { Id = 1, Name = "Venue" },
            new { Id = 2, Name = "Food" },
            new { Id = 3, Name = "Photographer" },
            new { Id = 4, Name = "Garlands" },
            new { Id = 5, Name = "Clothes" },
            new { Id = 6, Name = "Pundith" },
            new { Id = 7, Name = "Cool drinks/water" },
            new { Id = 8, Name = "Bridal wear (Hindu)" },
            new { Id = 9, Name = "Drinks" },
            new { Id = 10, Name = "Decor" },
            new { Id = 11, Name = "Music" },
            new { Id = 12, Name = "Mendi outfit" },
            new { Id = 13, Name = "Invites" },
            new { Id = 14, Name = "Pastor" },
            new { Id = 15, Name = "Photography" },
            new { Id = 16, Name = "Bridal wear (Christian)" },
            new { Id = 17, Name = "MC" },
            new { Id = 18, Name = "Cake" },
            new { Id = 19, Name = "Party favours" },
            new { Id = 20, Name = "Bridesmaids boxes" },
            new { Id = 21, Name = "Groomsmen boxes" },
            new { Id = 22, Name = "Flowers" },
            new { Id = 23, Name = "Confetti" },
            new { Id = 24, Name = "Violinist" },
            new { Id = 25, Name = "Alcohol" },
            new { Id = 26, Name = "360 Booth" },
            new { Id = 27, Name = "Band" },
            new { Id = 28, Name = "DJ" },
            new { Id = 29, Name = "Speeches" },
            new { Id = 30, Name = "Vows" },
            new { Id = 31, Name = "Handfasting" },
            new { Id = 32, Name = "Rehearsals" },
            new { Id = 33, Name = "Mendhi Artists" },
        };
        foreach (var c in categories)
            modelBuilder.Entity<Category>().HasData(new Category { Id = c.Id, Name = c.Name });

        var dayCategories = new[]
        {
            // Day 4 - Mendhi
            new { DayId = 4, CategoryId = 1, DisplayOrder = 1 },
            new { DayId = 4, CategoryId = 2, DisplayOrder = 2 },
            new { DayId = 4, CategoryId = 33, DisplayOrder = 3 },
            new { DayId = 4, CategoryId = 12, DisplayOrder = 4 },
            // Day 5 - Haldi
            new { DayId = 5, CategoryId = 1, DisplayOrder = 1 },
            new { DayId = 5, CategoryId = 2, DisplayOrder = 2 },
            new { DayId = 5, CategoryId = 5, DisplayOrder = 3 },
            new { DayId = 5, CategoryId = 10, DisplayOrder = 4 },
            // Day 1 - Hindu wedding and night before/Sangeet (merged Sangeet + Night Before)
            new { DayId = 1, CategoryId = 1, DisplayOrder = 1 },
            new { DayId = 1, CategoryId = 2, DisplayOrder = 2 },
            new { DayId = 1, CategoryId = 3, DisplayOrder = 3 },
            new { DayId = 1, CategoryId = 4, DisplayOrder = 4 },
            new { DayId = 1, CategoryId = 5, DisplayOrder = 5 },
            new { DayId = 1, CategoryId = 6, DisplayOrder = 6 },
            new { DayId = 1, CategoryId = 7, DisplayOrder = 7 },
            new { DayId = 1, CategoryId = 8, DisplayOrder = 8 },
            new { DayId = 1, CategoryId = 9, DisplayOrder = 9 },
            new { DayId = 1, CategoryId = 10, DisplayOrder = 10 },
            new { DayId = 1, CategoryId = 11, DisplayOrder = 11 },
            new { DayId = 1, CategoryId = 12, DisplayOrder = 12 },
            new { DayId = 3, CategoryId = 13, DisplayOrder = 1 },
            new { DayId = 3, CategoryId = 1, DisplayOrder = 2 },
            new { DayId = 3, CategoryId = 14, DisplayOrder = 3 },
            new { DayId = 3, CategoryId = 10, DisplayOrder = 4 },
            new { DayId = 3, CategoryId = 15, DisplayOrder = 5 },
            new { DayId = 3, CategoryId = 16, DisplayOrder = 6 },
            new { DayId = 3, CategoryId = 17, DisplayOrder = 7 },
            new { DayId = 3, CategoryId = 18, DisplayOrder = 8 },
            new { DayId = 3, CategoryId = 19, DisplayOrder = 9 },
            new { DayId = 3, CategoryId = 20, DisplayOrder = 10 },
            new { DayId = 3, CategoryId = 21, DisplayOrder = 11 },
            new { DayId = 3, CategoryId = 2, DisplayOrder = 12 },
            new { DayId = 3, CategoryId = 22, DisplayOrder = 13 },
            new { DayId = 3, CategoryId = 23, DisplayOrder = 14 },
            new { DayId = 3, CategoryId = 24, DisplayOrder = 15 },
            new { DayId = 3, CategoryId = 25, DisplayOrder = 16 },
            new { DayId = 3, CategoryId = 26, DisplayOrder = 17 },
            new { DayId = 3, CategoryId = 27, DisplayOrder = 18 },
            new { DayId = 3, CategoryId = 28, DisplayOrder = 19 },
            new { DayId = 3, CategoryId = 29, DisplayOrder = 20 },
            new { DayId = 3, CategoryId = 30, DisplayOrder = 21 },
            new { DayId = 3, CategoryId = 4, DisplayOrder = 22 },
            new { DayId = 3, CategoryId = 31, DisplayOrder = 23 },
            new { DayId = 3, CategoryId = 32, DisplayOrder = 24 },
            new { DayId = 3, CategoryId = 3, DisplayOrder = 25 },
            new { DayId = 3, CategoryId = 33, DisplayOrder = 26 },
        };
        foreach (var dc in dayCategories)
            modelBuilder.Entity<DayCategory>().HasData(new DayCategory { DayId = dc.DayId, CategoryId = dc.CategoryId, DisplayOrder = dc.DisplayOrder });
    }
}
