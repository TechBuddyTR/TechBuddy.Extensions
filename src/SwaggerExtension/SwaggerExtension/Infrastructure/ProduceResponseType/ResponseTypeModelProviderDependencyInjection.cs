using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.DependencyInjection;

namespace TechBuddy.Extensions.OpenApi.Infrastructure.ProduceResponseType;
/// <summary>
/// The extension class for ResponseTypeModelProviderDependencyInjection
/// </summary>
internal static class ResponseTypeModelProviderDependencyInjection
{
    internal static IServiceCollection ConfigureTechBuddyResponesTypemodelProvider(this IServiceCollection services,
        ResponseTypeModelProviderConfig config)
    {
        services.AddSingleton(config);
        services.AddSingleton<IApplicationModelProvider, ProduceResponseTypeModelProvider>();

        return services;
    }
}
