using AutoMapper;
using MediatR;
using microservices.catalog.api.Features.Categories.Dtos;
using microservices.catalog.api.Repositories;
using microservices.shared;
using microservices.shared.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace microservices.catalog.api.Features.Categories.Operations
{
    public static class GetAllCategoriesEndpoint
    {
        //QUERY
        public record GetAllCategoriesQuery : IRequest<ServiceResult<List<CategoryDto>>>;

        //HANDLER
        public class GetAllCategoriesQueryHandler(AppDbContext _context,IMapper mapper) : IRequestHandler<GetAllCategoriesQuery, ServiceResult<List<CategoryDto>>>
        {
            public async Task<ServiceResult<List<CategoryDto>>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
            {
                var categories = await _context.Categories.ToListAsync(cancellationToken);

                if (!categories.Any())
                    return ServiceResult<List<CategoryDto>>.Error("Category list is empty.",HttpStatusCode.NotFound);
                var categoriesAsDto = mapper.Map<List<CategoryDto>>(categories);

                return ServiceResult<List<CategoryDto>>.SuccessAsNoContext(categoriesAsDto);
            }
        }

        //ENDPOINT
        public static RouteGroupBuilder GetAllCategoriesGroupItemEndpoint(this RouteGroupBuilder group)
        {
            group.MapGet("/",
                async (IMediator mediator) => (await mediator.Send(new GetAllCategoriesQuery()))
                .ToGenericResult());
            return group;
        }
    }
}
