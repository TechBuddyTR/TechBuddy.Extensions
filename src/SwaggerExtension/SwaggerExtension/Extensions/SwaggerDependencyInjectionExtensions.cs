using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using TechBuddy.Extensions.OpenApi.Infrastructure;
using TechBuddy.Extensions.OpenApi.Infrastructure.OperationFilters;
using TechBuddy.Extensions.OpenApi.Infrastructure.ProduceResponseType;

namespace TechBuddy.Extensions.OpenApi;

/// <summary>
/// The extension class for OpenApi
/// </summary>
public static class SwaggerDependencyInjectionExtensions
{

    /// <summary>
    /// This is used to configure the Swagger implementation with default configuration
    /// </summary>
    /// <param name="services">IServiceCollection</param>
    /// <returns>returns services</returns>
    public static IServiceCollection ConfigureTechBuddySwagger(this IServiceCollection services)
    {
        return ConfigureTechBuddySwagger(services, opt =>
        {
            // Default configs
        });
    }

    /// <summary>
    /// This is used to configure the Swagger implementation
    /// </summary>
    /// <param name="services">IServiceCollection</param>
    /// <param name="configAction">The configuration to customize the Swagger</param>
    /// <returns>returns services</returns>
    public static IServiceCollection ConfigureTechBuddySwagger(this IServiceCollection services, Action<SwaggerConfig> configAction)
    {
        SwaggerConfig config = new();
        configAction(config);
        services.AddSingleton(config);

        if (config.ResponseTypeModelProviderConfig is not null)
        {
            services.ConfigureTechBuddyResponesTypemodelProvider(config.ResponseTypeModelProviderConfig);
        }

        services.AddSwaggerGen(c =>
        {
            c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());

            if (config.BearerConfig is not null)
            {
                c.AddSecurityDefinition(config.BearerConfig.HeaderKey, new OpenApiSecurityScheme()
                {
                    In = ParameterLocation.Header,
                    BearerFormat = SwaggerConstants.BearerFormat,
                    Type = SecuritySchemeType.Http,
                    Scheme = config.BearerConfig.HeaderKey,
                    Description = config.BearerConfig.BearerDescription
                });

                c.OperationFilter<AuthenticationRequirements>(config.BearerConfig);
            }

            if (config.Headers is not null)
            {
                foreach (var header in config.Headers)
                {
                    c.OperationFilter<AddRequiredHeaderParameter>(header.Key, header.Value);
                }
            }
        });

        services.ConfigureOptions<ConfigureSwaggerOptions>();

        return services;
    }


    /// <summary>
    /// This is used to enable Swagger in your WebApi
    /// </summary>
    /// <param name="app">IApplicationBuilder</param>
    /// <returns>returns app</returns>
    public static IApplicationBuilder UseTechBuddySwagger(this IApplicationBuilder app)
    {
        IApiVersionDescriptionProvider apiVersioningProvider = app.ApplicationServices.GetService<IApiVersionDescriptionProvider>();
        SwaggerConfig swaggerConfig = app.ApplicationServices.GetRequiredService<SwaggerConfig>();

        app.UseSwagger();

        app.UseSwaggerUI(options =>
        {
            SetOptionDetails(options);

            SetSwaggerEndPoint(options, swaggerConfig, apiVersioningProvider);
        });

        return app;
    }

    private static void SetOptionDetails(SwaggerUIOptions options)
    {
        options.DefaultModelExpandDepth(2);
        options.DefaultModelRendering(ModelRendering.Model);
        options.DefaultModelsExpandDepth(-1);
        options.DisplayOperationId();
        options.DisplayRequestDuration();
        options.DocExpansion(DocExpansion.List);
        options.EnableDeepLinking();
        options.EnableFilter();
        options.MaxDisplayedTags(5);
        options.ShowExtensions();
        options.ShowCommonExtensions();
        options.EnableValidator();
    }

    private static void SetSwaggerEndPoint(SwaggerUIOptions options, SwaggerConfig config, IApiVersionDescriptionProvider provider = null)
    {
        if (provider is null)
        {
            options.SwaggerEndpoint(SwaggerConstants.DefaultSwaggerEndpoint, SwaggerConstants.DefaultSwaggerApiVersion);
            return;
        }

        foreach (var description in provider.ApiVersionDescriptions)
        {
            var swaggerFileUrl = $"/{SwaggerConstants.DefaultSwaggerEndpointFileDirectory}/{description.GroupName}/{SwaggerConstants.DefaultSwaggerEndpointFileName}";
            var name = $"{config?.ProjectName} {description.GroupName.ToUpperInvariant()}";
            options.SwaggerEndpoint(swaggerFileUrl, name);
        }
    }
}