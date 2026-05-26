using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OurBigDay.Api.DTOs;
using OurBigDay.Api.Services;

namespace OurBigDay.Api.Controllers;

[Route("api/wedding")]
[ApiController]
[Authorize]
public class WeddingDaysController : ApiControllerBase
{
    private readonly IWeddingDayService _dayService;

    public WeddingDaysController(IWeddingDayService dayService)
    {
        _dayService = dayService;
    }

    [HttpGet("days")]
    public async Task<ActionResult<IEnumerable<WeddingDayDto>>> GetDays(CancellationToken ct = default) =>
        Ok(await _dayService.GetAllAsync(ct));
}
