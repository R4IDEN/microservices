using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace microservices.shared.Filters
{
    public class ValidationFilter<T> : IEndpointFilter
    {
        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            //endpoint çalışmadan önce buraya düşer
            var validator = context.HttpContext.RequestServices.GetService<IValidator<T>>();
            
            if (validator is null)
                return await next(context);

            var firstParameter = context.Arguments.OfType<T>().FirstOrDefault();
            if (firstParameter is null)
                return await next(context);

            var validateResult = await validator.ValidateAsync(firstParameter);
            if (!validateResult.IsValid)
                return Results.ValidationProblem(validateResult.ToDictionary());

            return await next(context);
            //await next'ten sonra : endpoint çalıştıktan sonra buraya düşer
        }
    }
}
