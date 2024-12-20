using AutoMapper;
using microservices.catalog.api.Features.Courses.Dtos;
using static microservices.catalog.api.Features.Courses.Operations.CreateCourseEndpoint;

namespace microservices.catalog.api.Features.Courses
{
    public class CourseMapping : Profile
    {
        public CourseMapping() 
        {
            CreateMap<CreateCourseCommand, Course>();

            CreateMap<Course, CourseDto>().ReverseMap();
        }
    }
}
