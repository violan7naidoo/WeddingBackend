using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OurBigDay.Api.Data;
using OurBigDay.Api.DTOs;
using OurBigDay.Api.Entities;

namespace OurBigDay.Api.Controllers;

[Route("api/wedding/days")]
[ApiController]
[Authorize(Roles = "Admin,Family")]
public class CategoriesController : ControllerBase
{
    private readonly AppDbContext _context;

    public CategoriesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("{dayId:int}/categories")]
    public async Task<ActionResult<DayCategoriesResponse>> GetCategoriesByDay(int dayId, CancellationToken cancellationToken = default)
    {
        var day = await _context.WeddingDays.FindAsync(new object[] { dayId }, cancellationToken);
        if (day == null)
            return NotFound("Day not found.");

        var categories = await _context.DayCategories
            .Where(dc => dc.DayId == dayId)
            .OrderBy(dc => dc.DisplayOrder)
            .Include(dc => dc.Category)
            .Select(dc => new CategoryDto(dc.Category.Id, dc.Category.Name, dc.DisplayOrder))
            .ToListAsync(cancellationToken);

        return Ok(new DayCategoriesResponse(dayId, day.ThemeName, categories));
    }

    [HttpPost("{dayId:int}/categories")]
    public async Task<ActionResult<CategoryDto>> CreateCategory(int dayId, CreateCategoryRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return BadRequest("Category name is required.");

        var day = await _context.WeddingDays.FindAsync(new object[] { dayId }, cancellationToken);
        if (day == null)
            return NotFound("Day not found.");

        var name = request.Name.Trim();

        // Reuse existing category with the same name, or create a new one.
        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Name == name, cancellationToken);
        if (category == null)
        {
            category = new Category { Name = name };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync(cancellationToken);
        }

        // Prevent duplicate links on the same day.
        var alreadyLinked = await _context.DayCategories
            .AnyAsync(dc => dc.DayId == dayId && dc.CategoryId == category.Id, cancellationToken);
        if (alreadyLinked)
            return Conflict("A table with that name already exists for this day.");

        var maxOrder = await _context.DayCategories
            .Where(dc => dc.DayId == dayId)
            .MaxAsync(dc => (int?)dc.DisplayOrder, cancellationToken) ?? 0;

        var dayCategory = new DayCategory { DayId = dayId, CategoryId = category.Id, DisplayOrder = maxOrder + 1 };
        _context.DayCategories.Add(dayCategory);
        await _context.SaveChangesAsync(cancellationToken);

        return Ok(new CategoryDto(category.Id, category.Name, dayCategory.DisplayOrder));
    }
}
