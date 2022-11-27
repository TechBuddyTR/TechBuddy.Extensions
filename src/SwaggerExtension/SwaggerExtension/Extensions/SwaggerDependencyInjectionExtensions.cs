using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechBuddy.Extensions.OpenApi.Infrastructure;
using TechBuddy.Extensions.OpenApi.Infrastructure.ConfigModels;
using TechBuddy.Extensions.OpenApi.Infrastructure.OperationFilters;
using TechBuddy.Extensions.OpenApi.Infrastructure.ProduceResponseType;

namespace TechBuddy.Extensions.OpenApi.Extensions;

/// <summary>
/// The extension class for OpenApi
/// </summary>
public static class SwaggerDependencyInjectionExtensions
{
    private static SwaggerConfig config;

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
    public static IServiceCollection ConfigureTechBuddySwagger(this IServiceCollection services,
                                                                Action<SwaggerConfig> configAction)
    {
        config = new SwaggerConfig();
        configAction(config);

        if (config.ResponseTypeModelProviderConfig is not null)
        {
            services.ConfigureTechBuddyResponesTypemodelProvider(config.ResponseTypeModelProviderConfig);
        }

        services.AddSwaggerGen(c =>
        {
            c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());

            if (config.BearerConfig is not null)
            {
                c.AddSecurityDefinition(config.BearerConfig.HeaderKey, new Microsoft.OpenApi.Models.OpenApiSecurityScheme()
                {
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    BearerFormat = SwaggerConstants.BearerFormat,
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
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
        return UseTechBuddySwaggerWithApiVersioning(app, null);
    }

    /// <summary>
    /// This is used to enable Swagger with ApiVersioning Features
    /// </summary>
    /// <param name="app">IApplicationBuilder</param>
    /// <param name="provider">IApiVersionDescriptionProvider</param>
    /// <returns>returns app</returns>
    public static IApplicationBuilder UseTechBuddySwaggerWithApiVersioning(this IApplicationBuilder app,
        IApiVersionDescriptionProvider provider)
    {
        app.UseSwagger();

        app.UseSwaggerUI(options =>
        {
            SetOptionDetails(options);

            SetSwaggerEndPoint(options, provider);
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

    private static void SetSwaggerEndPoint(SwaggerUIOptions options,
                                           IApiVersionDescriptionProvider provider = null)
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

    private class ConfigureSwaggerOptions : IConfigureNamedOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider provider;

        /// <summary>
        /// The constructure with <see cref="IApiVersionDescriptionProvider"/> so DI can create by providing in case of ApiVersioning is ENABLED
        /// </summary>
        /// <param name="provider"></param>
        public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
        {
            this.provider = provider;
        }

        /// <summary>
        /// The parameterless constructure so DI can create without providing <see cref="IApiVersionDescriptionProvider"/> in case of ApiVersioning is DISABLED
        /// </summary>
        public ConfigureSwaggerOptions()
        {

        }

        public void Configure(string name, SwaggerGenOptions options)
        {
            Configure(options);
        }

        public void Configure(SwaggerGenOptions options)
        {
            if (provider?.ApiVersionDescriptions is not null)
            {
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerDoc(description.GroupName, CreateVersionInfo(description));
                }
            }

            if (config.XmlDocConfig is null || !config.XmlDocConfig.XmlDocEnabled)
                return;

            var fileExt = Path.GetExtension(config.XmlDocConfig.XmlFilePath);
            if (!fileExt.Equals(".xml", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("XMLDocFile extension must be .xml!");

            if (!File.Exists(config.XmlDocConfig.XmlFilePath))
                throw new Exception($"XMLDocFile could not found in {config.XmlDocConfig.XmlFilePath}");

            options.IncludeXmlComments(config.XmlDocConfig.XmlFilePath, true);
        }

        private static OpenApiInfo CreateVersionInfo(ApiVersionDescription description)
        {
            var info = new OpenApiInfo()
            {
                Title = config.ProjectName,
                Version = description.ApiVersion.ToString(),
                Contact = new OpenApiContact()
                {
                    Name = "https://youtube.com/c/TechBuddyTR"
                }
            };

            if (description.IsDeprecated)
            {
                info.Description += " This API has been deprecated.";
            }

            return info;
        }
    }
}


