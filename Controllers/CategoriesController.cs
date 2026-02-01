using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OurBigDay.Api.Data;
using OurBigDay.Api.DTOs;

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
}
