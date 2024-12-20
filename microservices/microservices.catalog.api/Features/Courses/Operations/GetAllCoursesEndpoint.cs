using AutoMapper;
using MediatR;
using microservices.catalog.api.Features.Courses.Dtos;
using microservices.catalog.api.Repositories;
using microservices.shared;
using microservices.shared.Extensions;
using Microsoft.EntityFrameworkCore;

namespace microservices.catalog.api.Features.Courses.Operations
{
    public static class GetAllCoursesEndpoint
    {
        //QUERY
        public record GetAllCoursesQuery : IRequest<ServiceResult<List<CourseDto>>>;

        //HANDLER
        public class GetAllCoursesQueryHandler(AppDbContext _context, IMapper mapper) : IRequestHandler<GetAllCoursesQuery, ServiceResult<List<CourseDto>>>
        {
            public async Task<ServiceResult<List<CourseDto>>> Handle(GetAllCoursesQuery request, CancellationToken cancellationToken)
            {
                var courses = await _context.Courses.ToListAsync(cancellationToken);
                var categories = await _context.Categories.ToListAsync(cancellationToken);

                foreach (var course in courses)
                {
                    course.Category = categories.First(x => x.Id == course.CategoryId);
                }

                var coursesAsDto = mapper.Map<List<CourseDto>>(courses);
                return ServiceResult<List<CourseDto>>.SuccessAsNoContext(coursesAsDto);
            }
        }

        //ENDPOINT
        public static RouteGroupBuilder GetAllCoursesGroupItemEndpoint(this RouteGroupBuilder group)
        {
            group.MapGet("/",
                async (IMediator mediator) => (await mediator.Send(new GetAllCoursesQuery()))
                .ToGenericResult());
            return group;
        }
    }
}
