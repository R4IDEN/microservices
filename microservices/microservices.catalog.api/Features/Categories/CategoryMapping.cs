using AutoMapper;
using microservices.catalog.api.Features.Categories.Dtos;

namespace microservices.catalog.api.Features.Categories
{
    public class CategoryMapping:Profile
    {
        public CategoryMapping()
        {
            CreateMap<Category, CategoryDto>().ReverseMap();
        }
    }
}
