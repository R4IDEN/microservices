using microservices.catalog.api.Features.Categories.Dtos;

namespace microservices.catalog.api.Features.Courses.Dtos
{
    public record CourseDto(
        Guid id,
        string Name,
        string Description,
        decimal Price,
        string? ImageUrl,
        CategoryDto category, 
        Feature feature);
}
