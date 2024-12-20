using AutoMapper;
using FluentValidation;
using MassTransit;
using MediatR;
using microservices.catalog.api.Repositories;
using microservices.shared;
using microservices.shared.Extensions;
using microservices.shared.Filters;
using Microsoft.EntityFrameworkCore;
using System.Net;
using static microservices.catalog.api.Features.Categories.Operations.CreateCateogryEndpoint;

namespace microservices.catalog.api.Features.Courses.Operations
{
    public static class CreateCourseEndpoint
    {
        //COMMAND
        public record CreateCourseCommand(string Name, string Description, decimal Price, string? ImageUrl, Guid CategoryId) : IRequest<ServiceResult<Guid>>;

        //VALIDATOR
        public class CreateCourseCommandValidator : AbstractValidator<CreateCourseCommand> 
        {
            public CreateCourseCommandValidator() 
            {
                RuleFor(x => x.Name).NotEmpty().WithMessage("{PropertyName} name is required").MaximumLength(100).WithMessage("{PropertyName} must not exceed 100 characters.");
                RuleFor(x => x.Description).NotEmpty().WithMessage("{PropertyName} name is required").MaximumLength(1000).WithMessage("{PropertyName} must not exceed 1000 characters.");
                RuleFor(x => x.Price).GreaterThan(0).WithMessage("Course price must be greater than 0");
            }
        }

        //HANDLER
        public class CreateCourseCommandHandler(AppDbContext _context, IMapper _mapper) : IRequestHandler<CreateCourseCommand, ServiceResult<Guid>>
        {
            public async Task<ServiceResult<Guid>> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
            {
                //course'un category'si var mı kontrol et
                var hasCategory = await _context.Categories.AnyAsync(x => x.Id == request.CategoryId, cancellationToken);

                if(!hasCategory)
                    return ServiceResult<Guid>.Error("Category not found", $"Category (id = {request.CategoryId}) not found", HttpStatusCode.NotFound);

                //course daha önce eklenmiş mi kontrol et
                var existingCourse = await _context.Courses.AnyAsync(x => x.Name == request.Name, cancellationToken);
                if(existingCourse)
                    return ServiceResult<Guid>.Error("Course already exists", $"Course (name = {request.Name}) already exists", HttpStatusCode.BadRequest);

                //course ekleme işlemi
                var course = _mapper.Map<Course>(request);
                course.CreatedAt = DateTime.Now;
                course.Id = NewId.NextSequentialGuid();
                course.Feature = new Feature()
                {
                    Duration = 10,
                    Rating = 45,
                    TeacherName = "sarı çizmeli aga"
                };
                
                _context.Courses.Add(course);
                await _context.SaveChangesAsync(cancellationToken);

                //course ekleme işlemi başarılı ise
                return ServiceResult<Guid>.SuccessAsCreated(course.Id, $"/api/course/{course.Id}");
            }
        }

        //ENDPOINT
        public static RouteGroupBuilder CreateCourseGroupItemEndpoint(this RouteGroupBuilder group)
        {
            group.MapPost("/",
                async (CreateCourseCommand cmd, IMediator mediator) => (await mediator.Send(cmd))
                .ToGenericResult())
                .MapToApiVersion(1, 0)
                .AddEndpointFilter<ValidationFilter<CreateCourseCommand>>();
            
            return group;
        }
    }
}
