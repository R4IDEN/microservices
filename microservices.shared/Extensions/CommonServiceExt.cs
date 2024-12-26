using FluentValidation;
using FluentValidation.AspNetCore;
using microservices.shared.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace microservices.shared.Extensions
{
    public static class CommonServiceExt
    {
        public static IServiceCollection AddCommonServiceExt(this IServiceCollection services, Type assembly)
        {
            services.AddHttpContextAccessor();
            services.AddMediatR(x => x.RegisterServicesFromAssemblyContaining(assembly));
            services.AddFluentValidationAutoValidation();
            services.AddAutoMapper(assembly);
            services.AddValidatorsFromAssemblyContaining(assembly);
            services.AddScoped<IIdentityService, IdentityServiceFake>();

            return services;
        }
    }
}
