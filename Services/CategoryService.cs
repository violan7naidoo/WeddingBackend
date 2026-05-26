using Microsoft.EntityFrameworkCore;
using OurBigDay.Api.Data;
using OurBigDay.Api.DTOs;
using OurBigDay.Api.Entities;
using OurBigDay.Api.Exceptions;

namespace OurBigDay.Api.Services;

public class CategoryService : ICategoryService
{
    private readonly AppDbContext _context;

    public CategoryService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<DayCategoriesResponse> GetByDayAsync(int dayId, CancellationToken ct = default)
    {
        var day = await _context.WeddingDays.FindAsync(new object[] { dayId }, ct)
            ?? throw new NotFoundException("Day not found.");

        var categories = await _context.DayCategories
            .Where(dc => dc.DayId == dayId)
            .OrderBy(dc => dc.DisplayOrder)
            .Include(dc => dc.Category)
            .Select(dc => new CategoryDto(dc.Category.Id, dc.Category.Name, dc.DisplayOrder))
            .ToListAsync(ct);

        return new DayCategoriesResponse(dayId, day.ThemeName, categories);
    }

    public async Task<CategoryDto> CreateAsync(int dayId, CreateCategoryRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new BusinessException("Category name is required.");

        if (!await _context.WeddingDays.AnyAsync(d => d.Id == dayId, ct))
            throw new NotFoundException("Day not found.");

        var name = request.Name.Trim();

        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Name == name, ct);
        if (category == null)
        {
            category = new Category { Name = name };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync(ct);
        }

        var alreadyLinked = await _context.DayCategories
            .AnyAsync(dc => dc.DayId == dayId && dc.CategoryId == category.Id, ct);
        if (alreadyLinked)
            throw new ConflictException("A table with that name already exists for this day.");

        var maxOrder = await _context.DayCategories
            .Where(dc => dc.DayId == dayId)
            .MaxAsync(dc => (int?)dc.DisplayOrder, ct) ?? 0;

        var dayCategory = new DayCategory { DayId = dayId, CategoryId = category.Id, DisplayOrder = maxOrder + 1 };
        _context.DayCategories.Add(dayCategory);
        await _context.SaveChangesAsync(ct);

        return new CategoryDto(category.Id, category.Name, dayCategory.DisplayOrder);
    }

    public async Task DeleteAsync(int dayId, int categoryId, CancellationToken ct = default)
    {
        var dayCategory = await _context.DayCategories
            .FindAsync(new object[] { dayId, categoryId }, ct)
            ?? throw new NotFoundException("Category not found for this day.");

        var items = await _context.WeddingItems
            .Where(wi => wi.DayId == dayId && wi.CategoryId == categoryId)
            .ToListAsync(ct);
        _context.WeddingItems.RemoveRange(items);
        _context.DayCategories.Remove(dayCategory);

        await _context.SaveChangesAsync(ct);
    }
}
