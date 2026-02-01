using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OurBigDay.Api.Data;
using OurBigDay.Api.Entities;

namespace OurBigDay.Api.Controllers;

[Route("api/wedding")]
[ApiController]
[Authorize]
public class WeddingDaysController : ControllerBase
{
    private readonly AppDbContext _context;

    public WeddingDaysController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("days")]
    public async Task<ActionResult<IEnumerable<WeddingDay>>> GetDays(CancellationToken cancellationToken = default)
    {
        var days = await _context.WeddingDays
            .OrderBy(d => d.DayNumber)
            .ToListAsync(cancellationToken);
        return Ok(days);
    }
}
