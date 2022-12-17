using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace TechBuddy.Extensions.OpenApi;

internal class ConfigureSwaggerOptions : IConfigureNamedOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider provider;
    private readonly SwaggerConfig config;

    /// <summary>
    /// The constructure with <see cref="IApiVersionDescriptionProvider"/> so DI can create by providing in case of ApiVersioning is ENABLED
    /// </summary>
    /// <param name="provider"></param>
    /// <param name="config"></param>
    public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider, SwaggerConfig config)
    {
        this.provider = provider;
        this.config = config;
    }

    /// <summary>
    /// The parameterless constructure so DI can create without providing <see cref="IApiVersionDescriptionProvider"/> in case of ApiVersioning is DISABLED
    /// </summary>
    public ConfigureSwaggerOptions(SwaggerConfig config)
    {
        this.config = config;
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
        if (string.IsNullOrWhiteSpace(fileExt) || !fileExt.Equals(".xml", StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("XMLDocFile extension must be .xml!");

        if (!File.Exists(config.XmlDocConfig.XmlFilePath))
            throw new Exception($"XMLDocFile could not found in {config.XmlDocConfig.XmlFilePath}");

        options.IncludeXmlComments(config.XmlDocConfig.XmlFilePath, true);
    }

    private OpenApiInfo CreateVersionInfo(ApiVersionDescription description)
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

