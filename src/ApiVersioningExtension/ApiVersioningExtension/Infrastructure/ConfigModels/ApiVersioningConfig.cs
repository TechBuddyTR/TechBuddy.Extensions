using Microsoft.AspNetCore.Mvc.Versioning;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechBuddy.Extensions.AspNetCore.ApiVersioning;

/// <summary>
/// The config to use when configuring ApiVersioning for your WebAPI projects
/// </summary>
public class ApiVersioningConfig
{
    internal Dictionary<ApiVersioningReaders, string> ApiVersioningReaders = new Dictionary<ApiVersioningReaders, string>();
    internal IErrorResponseProvider ErrorResponseProvider { get; set; }
    internal const string ParameterName = "x-api-version";

    /// <summary>
    /// The part of the format for api versioning controller route
    /// </summary>
    public const string ApiVersionFormat = "v{version:apiVersion}";

    /// <summary>
    /// The format for api versioning controller route
    /// </summary>
    /// <example>[Route(<see cref="ControllerRoute"/>)]</example>
    public const string ControllerRoute = $"api/{ApiVersionFormat}/[controller]";

    /// <summary>
    /// The format without api prefix for api versioning controller route
    /// </summary>
    /// <example>[Route(<see cref="ControllerRouteWithoutApi"/>)]</example>
    public const string ControllerRouteWithoutApi = $"{ApiVersionFormat}/[controller]";

    /// <summary>
    /// Set true to assume the default version is used when no api version provided
    /// </summary>
    public bool AssumeDefaultVersionWhenUnspecified { get; set; } = true;

    /// <summary>
    /// The default api version to be used when <see cref="AssumeDefaultVersionWhenUnspecified"/> is true
    /// </summary>
    public string DefaultApiVersion { get; set; } = "1.0";

    /// <summary>
    /// When true, it reports back the available versions via Response Headers
    /// </summary>
    public bool ReportApiVersions { get; set; } = true;

    /// <summary>
    /// Enables to be exploered by another App UIs for example Swagger
    /// </summary>
    public bool EnableVersionedApiExplorer { get; set; } = true;

    /// <summary>
    /// Allows to read the api version from QueryString with a specific parameter name
    /// <example><code>https://localhost/api/GetTest?x-api-version=1.0</code></example>
    /// </summary>
    /// <param name="parameterName">The parameter name</param>
    /// <returns>itself</returns>
    public ApiVersioningConfig AddQueryStringApiVersionReader(string parameterName)
    {
        ArgumentNullException.ThrowIfNull(parameterName, nameof(parameterName));

        ApiVersioningReaders.TryAdd(ApiVersioning.ApiVersioningReaders.QueryStringApiVersionReader, parameterName);

        return this;
    }

    /// <summary>
    /// Allows to read the api version from route url. Once this is used, <see cref="ApiVersionFormat"/> must be used in controller route
    /// 
    /// <example>[Route(ApiVersioningConfig.ControllerRoute)]</example>
    /// <example>
    /// <code>
    /// [ApiController]
    /// [Route(api/v{version:apiVersion}/[controller])]
    /// [ApiVersion("1.0")]
    /// public class TestController : ControllerBase 
    /// {
    /// 
    /// }
    /// </code>
    /// </example>
    /// </summary>
    /// <returns>itself</returns>
    public ApiVersioningConfig AddUrlSegmentApiVersionReader()
    {
        ApiVersioningReaders.Add(ApiVersioning.ApiVersioningReaders.UrlSegmentApiVersionReader, "");

        return this;
    }

    /// <summary>
    /// Allows to read the api version from Header
    /// </summary>
    /// <param name="headerKey">The header key name</param>
    /// <returns>itself</returns>
    public ApiVersioningConfig AddHeaderApiVersionReader(string headerKey)
    {
        ArgumentNullException.ThrowIfNull(headerKey);

        ApiVersioningReaders.Add(ApiVersioning.ApiVersioningReaders.HeaderApiVersionReader, headerKey);

        return this;
    }

    /// <summary>
    /// Adds the provided ErrorResponseProvider as custom provider to be used when any Api Versioning exception is occured
    /// </summary>
    /// <typeparam name="T">The provider derived from <see cref="IErrorResponseProvider"/></typeparam>
    /// <returns>itself</returns>
    public ApiVersioningConfig UseErrorResponseProvider<T>(T errorResponseProvider) where T : IErrorResponseProvider
    {
        ErrorResponseProvider = errorResponseProvider;

        return this;
    }

    /// <summary>
    /// Adds the ErrorResponseProvider by creating an instance of T to be used when any Api Versioning exception is occured
    /// </summary>
    /// <typeparam name="T">The provider derived from <see cref="IErrorResponseProvider"/></typeparam>
    /// <returns>itself</returns>
    public ApiVersioningConfig UseErrorResponseProvider<T>() where T : IErrorResponseProvider
    {
        return UseErrorResponseProvider(Activator.CreateInstance<T>());
    }
}

/// <summary>
/// The enum to provide ApiVersionReader method
/// </summary>
internal enum ApiVersioningReaders
{
    /// <summary>
    /// Read from Header
    /// </summary>
    HeaderApiVersionReader = 1,

    /// <summary>
    /// Read from Url Route
    /// </summary>
    UrlSegmentApiVersionReader = 2,

    /// <summary>
    /// Read from Query String
    /// </summary>
    QueryStringApiVersionReader = 4
}
