using OurBigDay.Api.DTOs;

namespace OurBigDay.Api.Services;

public interface IWeddingItemService
{
    Task<IEnumerable<WeddingItemDto>> GetByDayAsync(int dayId, CancellationToken ct = default);
    Task<IEnumerable<WeddingItemDto>> GetByDayAndCategoryAsync(int dayId, int categoryId, CancellationToken ct = default);
    Task<WeddingItemDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<WeddingItemDto> CreateAsync(CreateWeddingItemRequest request, CancellationToken ct = default);
    Task<WeddingItemDto> UpdateAsync(int id, UpdateWeddingItemRequest request, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}
