using Microsoft.EntityFrameworkCore;
using OurBigDay.Api.Data;
using OurBigDay.Api.DTOs;

namespace OurBigDay.Api.Services;

public class WeddingDayService : IWeddingDayService
{
    private readonly AppDbContext _context;

    public WeddingDayService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<WeddingDayDto>> GetAllAsync(CancellationToken ct = default) =>
        await _context.WeddingDays
            .OrderBy(d => d.DayNumber)
            .Select(d => new WeddingDayDto(d.Id, d.DayNumber, d.ThemeName, d.Date.ToString("yyyy-MM-dd")))
            .ToListAsync(ct);
}
