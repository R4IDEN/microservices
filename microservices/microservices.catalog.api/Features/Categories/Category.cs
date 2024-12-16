using microservices.catalog.api.Features.Courses;
using microservices.catalog.api.Repositories;

namespace microservices.catalog.api.Features.Categories
{
    public class Category : BaseEntity
    {
        public string Name { get; set; } = default!;
        public List<Course>? Courses { get; set; }
    }
}
