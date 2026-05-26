using OurBigDay.Api.DTOs;

namespace OurBigDay.Api.Services;

public interface IWeddingDayService
{
    Task<IEnumerable<WeddingDayDto>> GetAllAsync(CancellationToken ct = default);
}
