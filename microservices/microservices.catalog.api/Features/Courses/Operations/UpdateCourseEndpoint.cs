using AutoMapper;
using FluentValidation;
using MediatR;
using microservices.catalog.api.Repositories;
using microservices.shared;
using microservices.shared.Extensions;
using microservices.shared.Filters;
using System.Net;
using static microservices.catalog.api.Features.Courses.Operations.CreateCourseEndpoint;

namespace microservices.catalog.api.Features.Courses.Operations
{
    public static class UpdateCourseEndpoint
    {
        //COMMAND
        public record UpdateCourseCommand(Guid Id, string Name, string Description, decimal Price, string? ImageUrl, Guid CategoryId) : IRequest<ServiceResult>;

        //VALIDATOR
        public class UpdateCourseCommandValidator : AbstractValidator<UpdateCourseCommand>
        {
            public UpdateCourseCommandValidator()
            {
                RuleFor(x => x.Name).NotEmpty().WithMessage("{PropertyName} name is required").MaximumLength(100).WithMessage("{PropertyName} must not exceed 100 characters.");
                RuleFor(x => x.Description).NotEmpty().WithMessage("{PropertyName} name is required").MaximumLength(1000).WithMessage("{PropertyName} must not exceed 1000 characters.");
                RuleFor(x => x.Price).GreaterThan(0).WithMessage("Course price must be greater than 0");
            }
        }

        //HANDLER
        public class UpdateCourseCommandHandler(AppDbContext _context) : IRequestHandler<UpdateCourseCommand, ServiceResult>
        {
            public async Task<ServiceResult> Handle(UpdateCourseCommand request, CancellationToken cancellationToken)
            {
                var course = await _context.Courses.FindAsync(request.Id, cancellationToken);
                if(course is null)
                    return ServiceResult<Guid>.Error("Course not found", $"Course (id = {request.Id}) not found", HttpStatusCode.NotFound);

                course.Name = request.Name;
                course.Description = request.Description;
                course.Price = request.Price;
                course.ImageUrl = request.ImageUrl;
                course.CategoryId = request.CategoryId;

                _context.Courses.Update(course);
                await _context.SaveChangesAsync(cancellationToken);

                return ServiceResult.SuccessAsNoContext();
            }
        }

        //ENDPOINT
        public static RouteGroupBuilder UpdateCourseGroupItemEndpoint(this RouteGroupBuilder group)
        {
            group.MapPut("/",
                async (UpdateCourseCommand cmd, IMediator mediator) => (await mediator.Send(cmd))
                .ToGenericResult())
                .AddEndpointFilter<ValidationFilter<UpdateCourseCommand>>();

            return group;
        }
    }
}
