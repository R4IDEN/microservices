using FluentValidation;
using MassTransit;
using MediatR;
using microservices.catalog.api.Repositories;
using microservices.shared;
using microservices.shared.Extensions;
using microservices.shared.Filters;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace microservices.catalog.api.Features.Categories.Operations
{
    public static class CreateCateogryEndpoint
    {
        //COMMAND
        public record CreateCategoryCommand(string Name) : IRequest<ServiceResult<CreateCategoryResponse>>;

        //RESPONSE
        public record CreateCategoryResponse(Guid Id);

        //HANDLER
        public class CreateCategoryCommandHandler(AppDbContext _context) : IRequestHandler<CreateCategoryCommand, ServiceResult<CreateCategoryResponse>>
        {
            async Task<ServiceResult<CreateCategoryResponse>> IRequestHandler<CreateCategoryCommand, ServiceResult<CreateCategoryResponse>>.Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
            {
                var existingCategory = await _context.Categories.AnyAsync(x => x.Name == request.Name, cancellationToken);

                if (existingCategory)
                    return ServiceResult<CreateCategoryResponse>.Error("Category already exists", $"Category ({request.Name}) already exist.", HttpStatusCode.BadRequest);

                var category = new Category { Name = request.Name, Id = NewId.NextSequentialGuid() };

                await _context.Categories.AddAsync(category, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                return ServiceResult<CreateCategoryResponse>.SuccessAsCreated(new CreateCategoryResponse(category.Id), "<empty>");
            }
        }

        //VALIDATOR
        public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
        {
            public CreateCategoryCommandValidator()
            {
                RuleFor(x => x.Name).NotEmpty()
                    .WithMessage("Category name is required")
                    .Length(4,25).WithMessage("{PropertyName} must be between 4 and 25 characters");
            }
        }

        //ENDPOINT
        public static RouteGroupBuilder CreateCategoryGroupItemEndpoint(this RouteGroupBuilder group)
        {
            group.MapPost("/", 
                async (CreateCategoryCommand cmd, IMediator mediator) => (await mediator.Send(cmd))
                .ToGenericResult())
                .AddEndpointFilter<ValidationFilter<CreateCategoryCommand>>();

            return group;
        }
    }
}
