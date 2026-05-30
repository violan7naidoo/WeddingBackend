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

    Task<WeddingItemDto> AddPaymentAsync(int itemId, AddPaymentRequest request, CancellationToken ct = default);
    Task<WeddingItemDto> DeletePaymentAsync(int itemId, int paymentId, CancellationToken ct = default);

    Task<WeddingItemDto> AddImageAsync(int itemId, string imageBase64, CancellationToken ct = default);
    Task<WeddingItemDto> DeleteImageAsync(int itemId, int imageIndex, CancellationToken ct = default);
}
