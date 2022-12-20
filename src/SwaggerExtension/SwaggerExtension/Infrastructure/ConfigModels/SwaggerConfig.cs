using System.Reflection;
using TechBuddy.Extensions.OpenApi.Infrastructure;

namespace TechBuddy.Extensions.OpenApi;

/// <summary>
/// The config to customize Swagger implementation
/// </summary>
public class SwaggerConfig
{
    /// <summary>
    /// The list of Header
    /// </summary>
    public Dictionary<string, string> Headers { get; private set; } = new Dictionary<string, string>();


    /// <summary>
    /// The project name which will be used in Swagger UI and also documentation
    /// </summary>
    public string ProjectName { get; set; } = Assembly.GetCallingAssembly().GetName()?.Name ?? SwaggerConstants.DefaultProjectName;

    /// <summary>
    /// The Auth config if Bearer/JWT is enabled
    /// </summary>
    public SwaggerBearerConfig BearerConfig { get; set; }

    /// <summary>
    /// Xml document config to customize Swagger UI to show the doc comments
    /// </summary>
    public SwaggerDocConfig XmlDocConfig { get; set; }

    /// <summary>
    /// The config for ResponseTypeModelProvider
    /// </summary>
    public ResponseTypeModelProviderConfig ResponseTypeModelProviderConfig { get; set; }

    /// <summary>
    /// true when JsonIgnore is enabled on Swagger UI
    /// </summary>
    public bool EnabledJsonIgnoreFilter { get; set; } = true;




    /// <summary>
    /// Enables to add header value when requesting via Swagger UI
    /// </summary>
    /// <param name="key">The header key</param>
    /// <param name="value">The header value</param>
    /// <returns>return itself</returns>
    /// <exception cref="ArgumentException">This is thrown when duplicate key is added</exception>
    /// <exception cref="ArgumentNullException">This is thrown when key is null or empty</exception>
    public SwaggerConfig AddHeader(string key, string value)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentNullException(key);

        var added = Headers.TryAdd(key, value);

        return added
                ? this 
                : throw new ArgumentException($"{key} already exists in header");
    }

    /// <summary>
    /// Enables to add header value when requesting via Swagger UI
    /// </summary>
    /// <param name="key">The header key</param>
    /// <param name="valueFunc">The action to get the header value</param>
    /// <returns>return itself</returns>
    /// <exception cref="ArgumentException">This is thrown when duplicate key is added</exception>
    /// <exception cref="ArgumentNullException">This is thrown when key is null or empty</exception>
    public SwaggerConfig AddHeader(string key, Func<string> valueFunc)
    {
        var headerValue = valueFunc();
        return AddHeader(key, headerValue);
    }



}
