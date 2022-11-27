using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechBuddy.Extensions.OpenApi.Infrastructure.ConfigModels;

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
