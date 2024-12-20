using AutoMapper;
using MediatR;
using microservices.catalog.api.Features.Categories.Dtos;
using microservices.catalog.api.Repositories;
using microservices.shared;
using microservices.shared.Extensions;
using System.Net;

namespace microservices.catalog.api.Features.Categories.Operations
{
    public static class GetCategoryByIdEndpoint
    {
        //QUERY
        public record GetCategoryByIdQuery(Guid id) : IRequest<ServiceResult<CategoryDto>>;

        //HANDLER
        public class GetCategoryByIdQueryHandler(AppDbContext _context, IMapper mapper) : IRequestHandler<GetCategoryByIdQuery, ServiceResult<CategoryDto>>
        {
            public async Task<ServiceResult<CategoryDto>> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
            {
                var category = await _context.Categories.FindAsync(request.id);

                if (category is null)
                    return ServiceResult<CategoryDto>.Error("Category not found", $"Category (id = {request.id}) not found.", HttpStatusCode.NotFound);

                var categoryAsDto = mapper.Map<CategoryDto>(category);
                return ServiceResult<CategoryDto>.SuccessAsNoContext(categoryAsDto);
            }
        }

        //ENDPOINT
        public static RouteGroupBuilder GetCategoryByIdGroupItemEndpoint(this RouteGroupBuilder group)
        {
            group.MapGet("/{id:guid}",
                async (IMediator mediator, Guid id) => (await mediator.Send(new GetCategoryByIdQuery(id)))
                .ToGenericResult())
                .MapToApiVersion(1, 0); ;
            return group;
        }
    }  
}
