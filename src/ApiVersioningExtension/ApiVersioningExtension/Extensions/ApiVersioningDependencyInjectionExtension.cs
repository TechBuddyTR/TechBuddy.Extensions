using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.DependencyInjection;

namespace TechBuddy.Extensions.AspNetCore.ApiVersioning;
/// <summary>
/// The extensions for ApiVersioning
/// </summary>
public static class ApiVersioningDependencyInjectionExtension
{
    /// <summary>
    /// This is used to enabled ApiVersioning in your WebAPI with default configs
    /// </summary>
    /// <param name="services">IServiceCollection</param>
    /// <returns>returns services</returns>
    public static IServiceCollection AddTechBuddyApiVersioning(this IServiceCollection services)
    {
        return services.AddTechBuddyApiVersioning(options =>
        {
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.DefaultApiVersion = "1.0";
            options.ReportApiVersions = true;

            options.AddQueryStringApiVersionReader(ApiVersioningConfig.ParameterName);
        });
    }

    /// <summary>
    /// This is used to enabled ApiVersioning in your WebAPI
    /// </summary>
    /// <param name="services">IServiceCollection</param>
    /// <param name="options">The configuration to customize the api versioning feature</param>
    /// <returns>services</returns>
    public static IServiceCollection AddTechBuddyApiVersioning(this IServiceCollection services, Action<ApiVersioningConfig> options)
    {
        ApiVersioningConfig config = new();
        options.Invoke(config);

        services.AddApiVersioning(config);


        return services;
    }

    private static void AddApiVersioning(this IServiceCollection services, ApiVersioningConfig config)
    {
        services.AddApiVersioning(opt =>
        {
            // Set the error response provider
            opt.ErrorResponses = config.ErrorResponseProvider ?? new DefaultErrorResponseProvider();

            // this configuration will allow the api to automaticaly take api_version=1.0 in case it was not specify
            opt.AssumeDefaultVersionWhenUnspecified = config.AssumeDefaultVersionWhenUnspecified;
            opt.ReportApiVersions = config.ReportApiVersions;

            #region Default Version Parsing

            ApiVersion defaultVersion = ApiVersion.Default;
            if (!string.IsNullOrWhiteSpace(config.DefaultApiVersion))
            {
                if (!ApiVersion.TryParse(config.DefaultApiVersion.Replace("v", ""), out defaultVersion))
                    throw new ArgumentException($"Not valid version found({config.DefaultApiVersion}). Usage Example: 1.0 or 1 or v1.0 or v1");
            }

            opt.DefaultApiVersion = defaultVersion!;

            #endregion

            #region ApiVersionReaders

            if (config.ApiVersioningReaders is { Count: > 0 })
            {
                var readers = new List<IApiVersionReader>();

                foreach (var reader in config.ApiVersioningReaders)
                {
                    IApiVersionReader apiVersionReader = reader.Key switch
                    {
                        ApiVersioningReaders.HeaderApiVersionReader => new HeaderApiVersionReader(reader.Value),
                        ApiVersioningReaders.UrlSegmentApiVersionReader => new UrlSegmentApiVersionReader(),
                        ApiVersioningReaders.QueryStringApiVersionReader => new QueryStringApiVersionReader(reader.Value),
                        _ => throw new ArgumentException("Invalid value for reader")
                    };

                    readers.Add(apiVersionReader);
                }

                // The way we read the api version
                opt.ApiVersionReader = ApiVersionReader.Combine(readers);
            }

            #endregion
        });
        if (config.EnableVersionedApiExplorer)
            services.AddApiVersioningExplorer();
    }

    private static void AddApiVersioningExplorer(this IServiceCollection services)
    {
        services.AddVersionedApiExplorer(options =>
        {
            // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
            // note: the specified format code will format the version as "'v'major[.minor][-status]"
            options.GroupNameFormat = "'v'VVV";

            // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
            // can also be used to control the format of the API version in route templates
            options.SubstituteApiVersionInUrl = true;
        });
    }

}
