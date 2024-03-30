using Microsoft.Extensions.DependencyInjection;

namespace Handlers.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddHandlers(this IServiceCollection services)
    {
        services.AddSingleton<PrefixHandler>();
        services.AddSingleton<InteractionHandler>();
        return services;
    }
}