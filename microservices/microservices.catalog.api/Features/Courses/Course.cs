using microservices.catalog.api.Features.Categories;
using microservices.catalog.api.Features.Categories.Dtos;
using microservices.catalog.api.Repositories;

namespace microservices.catalog.api.Features.Courses
{
    public class Course:BaseEntity
    {
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public decimal Price { get; set; }
        public Guid UserId { get; set; }
        public string? ImageUrl { get; set; }
        public DateTimeOffset CreatedAt { get; set; }

        public Guid CategoryId { get; set; }
        public Category Category { get; set; } = default!; //Navigation property

        public Feature Feature { get; set; } = default!;
    }
}
