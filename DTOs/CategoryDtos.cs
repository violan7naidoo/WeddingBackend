namespace OurBigDay.Api.DTOs;

public record CategoryDto(int Id, string Name, int DisplayOrder);

public record DayCategoriesResponse(int DayId, string DayThemeName, List<CategoryDto> Categories);

public record CreateCategoryRequest(string Name);
