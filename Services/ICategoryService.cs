using OurBigDay.Api.DTOs;

namespace OurBigDay.Api.Services;

public interface ICategoryService
{
    Task<DayCategoriesResponse> GetByDayAsync(int dayId, CancellationToken ct = default);
    Task<CategoryDto> CreateAsync(int dayId, CreateCategoryRequest request, CancellationToken ct = default);
    Task DeleteAsync(int dayId, int categoryId, CancellationToken ct = default);
}
