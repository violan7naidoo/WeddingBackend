using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OurBigDay.Api.DTOs;
using OurBigDay.Api.Exceptions;
using OurBigDay.Api.Services;

namespace OurBigDay.Api.Controllers;

[Route("api/wedding/days")]
[ApiController]
[Authorize(Roles = "Admin,Family")]
public class CategoriesController : ApiControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet("{dayId:int}/categories")]
    public async Task<ActionResult<DayCategoriesResponse>> GetCategoriesByDay(int dayId, CancellationToken ct = default)
    {
        try { return Ok(await _categoryService.GetByDayAsync(dayId, ct)); }
        catch (BusinessException ex) { return HandleException(ex); }
    }

    [HttpPost("{dayId:int}/categories")]
    public async Task<ActionResult<CategoryDto>> CreateCategory(int dayId, CreateCategoryRequest request, CancellationToken ct = default)
    {
        try { return Ok(await _categoryService.CreateAsync(dayId, request, ct)); }
        catch (BusinessException ex) { return HandleException(ex); }
    }

    [HttpDelete("{dayId:int}/categories/{categoryId:int}")]
    public async Task<ActionResult> DeleteCategory(int dayId, int categoryId, CancellationToken ct = default)
    {
        try { await _categoryService.DeleteAsync(dayId, categoryId, ct); return NoContent(); }
        catch (BusinessException ex) { return HandleException(ex); }
    }
}
