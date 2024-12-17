using microservices.catalog.api.Options;
using Microsoft.Extensions.Options;
using ZstdSharp.Unsafe;

namespace microservices.catalog.api.Options
{
    public static class OptionsExt
    {
        public static IServiceCollection AddOptionsExt(this IServiceCollection _services)
        {
            _services.AddOptions<MongoOption>()
                .BindConfiguration(nameof(MongoOption))
                .ValidateDataAnnotations()
                .ValidateOnStart();
            
            _services.AddSingleton<MongoOption>(sp => sp.GetRequiredService<IOptions<MongoOption>>().Value);
            return _services;
        }
    }
}

