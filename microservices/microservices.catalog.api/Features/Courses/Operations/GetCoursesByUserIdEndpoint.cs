using AutoMapper;
using MediatR;
using microservices.catalog.api.Features.Courses.Dtos;
using microservices.catalog.api.Repositories;
using microservices.shared;
using static microservices.catalog.api.Features.Courses.Operations.GetCourseById;
using System.Net;
using Microsoft.EntityFrameworkCore;
using microservices.shared.Extensions;

namespace microservices.catalog.api.Features.Courses.Operations
{
    public static class GetCoursesByUserIdEndpoint
    {
        //QUERY
        public record GetCoursesByUserIdQuery(Guid UserId) : IRequest<ServiceResult<List<CourseDto>>>;

        //HANDLER 
        public class GetCoursesByUserIdQueryHandler(AppDbContext _context, IMapper _mapper) : IRequestHandler<GetCoursesByUserIdQuery, ServiceResult<List<CourseDto>>>
        {
            public async Task<ServiceResult<List<CourseDto>>> Handle(GetCoursesByUserIdQuery request, CancellationToken cancellationToken)
            {
                var courses = await _context.Courses.Where(x => x.UserId == request.UserId).ToListAsync(cancellationToken);
                var categories = await _context.Categories.ToListAsync(cancellationToken);

                foreach (var course in courses)
                {
                    course.Category = categories.First(x => x.Id == course.CategoryId);
                }

                var coursesAsDto = _mapper.Map<List<CourseDto>>(courses);
                return ServiceResult<List<CourseDto>>.SuccessAsNoContext(coursesAsDto);
            }
        }

        //ENDPOINT
        public static RouteGroupBuilder GetCoursesByUserIdGroupItemEndpoint(this RouteGroupBuilder group)
        {
            group.MapGet("/{id:guid}",
                async (IMediator mediator, Guid id) => (await mediator.Send(new GetCoursesByUserIdQuery(id)))
                .ToGenericResult())
                .MapToApiVersion(1, 0);
            return group;
        }

    }
}
