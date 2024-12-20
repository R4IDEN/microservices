using AutoMapper;
using MediatR;
using microservices.catalog.api.Features.Courses.Dtos;
using microservices.catalog.api.Repositories;
using microservices.shared;
using microservices.shared.Extensions;
using System.Net;

namespace microservices.catalog.api.Features.Courses.Operations
{
    public static class GetCourseById
    {
        //QUERY
        public record GetCourseByIdQuery(Guid id) : IRequest<ServiceResult<CourseDto>>;

        //HANDLER
        public class GetCourseByIdQueryHandler(AppDbContext _context, IMapper mapper) : IRequestHandler<GetCourseByIdQuery, ServiceResult<CourseDto>>
        {
            public async Task<ServiceResult<CourseDto>> Handle(GetCourseByIdQuery request, CancellationToken cancellationToken)
            {
                var course = await _context.Courses.FindAsync(request.id);
                if (course is null)
                    return ServiceResult<CourseDto>.Error("Course not found", $"Course (id = {request.id}) not found.", HttpStatusCode.NotFound);

                var category = await _context.Categories.FindAsync(course.CategoryId);

                if (category is null)
                    return ServiceResult<CourseDto>.Error("Category not found for course", $"Category (id = {course.CategoryId}) for Course (id = {request.id}) not found.", HttpStatusCode.NotFound);

                course.Category = category;

                var courseAsDto = mapper.Map<CourseDto>(course);
                return ServiceResult<CourseDto>.SuccessAsNoContext(courseAsDto);
            }
        }

        //ENDPOINT
        public static RouteGroupBuilder GetCourseByIdGroupItemEndpoint(this RouteGroupBuilder group)
        {
            group.MapGet("/{id:guid}",
                async (IMediator mediator, Guid id) => (await mediator.Send(new GetCourseByIdQuery(id)))
                .ToGenericResult())
                .MapToApiVersion(1, 0);
            return group;
        }
    }
}
