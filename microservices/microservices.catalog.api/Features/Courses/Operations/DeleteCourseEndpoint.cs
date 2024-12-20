using MediatR;
using microservices.catalog.api.Repositories;
using microservices.shared;
using microservices.shared.Extensions;
using System.Net;

namespace microservices.catalog.api.Features.Courses.Operations
{
    public static class DeleteCourseEndpoint
    {
        //COMMAND
        public record DeleteCourseCommand(Guid Id) : IRequest<ServiceResult>;

        //HANDLER
        public class DeleteCourseCommandHandler(AppDbContext _context) : IRequestHandler<DeleteCourseCommand, ServiceResult>
        {
            public async Task<ServiceResult> Handle(DeleteCourseCommand request, CancellationToken cancellationToken)
            {
                var course = await _context.Courses.FindAsync(request.Id, cancellationToken);
                if (course is null)
                    return ServiceResult<Guid>.Error("Course not found", $"Course (id = {request.Id}) not found", HttpStatusCode.NotFound);

                _context.Courses.Remove(course);
                await _context.SaveChangesAsync(cancellationToken);
                return ServiceResult.SuccessAsNoContext();
            }
        }

        //ENDPOINT
        public static RouteGroupBuilder DeleteCourseGroupItemEndpoint(this RouteGroupBuilder group)
        {
            group.MapDelete("/{id}",
                async (Guid id, IMediator mediator) => (await mediator.Send(new DeleteCourseCommand(id)))
                .ToGenericResult())
                .MapToApiVersion(1, 0);
            return group;
        }
    }
}



