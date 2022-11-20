using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System.Text;
using System.Text.Json;
using TechBuddy.Extensions.Tests.Common.TestCommon.Constants;

namespace TechBuddy.Extensions.Tests.Common.TestCommon.Builders;

/// <summary>
/// The HttpRequestBuilder to build httprequest for testing purpose
/// </summary>
public class HttpRequestBuilder
{
    private readonly DefaultHttpContext context;
    private readonly HttpRequest request;
    private readonly Dictionary<string, List<string>> headerCollectionBuilderDictionary;
    private HttpMethod httpMethod = HttpMethod.Get;
    private string requestUrl;
    private string bodyJson;

    /// <summary>
    /// Create a new builder
    /// </summary>
    /// <returns>new HttpRequestBuilder</returns>
    public static HttpRequestBuilder CreateRequest(string url = "", HttpMethod method = null)
    {
        var builder = new HttpRequestBuilder
        {
            httpMethod = method,
            requestUrl = url
        };

        return builder;
    }

    /// <summary>
    /// Instantiates a new instance - internal to promote static CreateRequest
    /// </summary>
    internal HttpRequestBuilder()
    {
        context = new DefaultHttpContext();
        request = context.Request;
        headerCollectionBuilderDictionary = new Dictionary<string, List<string>>();
    }

    /// <summary>
    /// Add a value or set of values to the header collection against the specified key
    /// </summary>
    /// <param name="key">Header key</param>
    /// <param name="value">Values for key</param>
    /// <returns>HttpRequestBuilder</returns>
    public HttpRequestBuilder AddHeader(string key, params string[] value)
    {
        if (headerCollectionBuilderDictionary.ContainsKey(key))
        {
            headerCollectionBuilderDictionary[key].AddRange(value);
        }
        else
        {
            headerCollectionBuilderDictionary.Add(key, new List<string>(value));
        }

        return this;
    }

    /// <summary>
    /// Sets the body of the request to a stream containing the given text
    /// </summary>
    /// <param name="stringBodyContent">Body string content</param>
    /// <returns>Http Request Builder</returns>
    public HttpRequestBuilder SetBody(string stringBodyContent)
    {
        bodyJson = stringBodyContent;

        var ms = new MemoryStream();
        using (var writer = new StreamWriter(ms, Encoding.Default, 4096, true))
        {
            writer.Write(stringBodyContent);
        }

        ms.Seek(0, SeekOrigin.Begin);
        request.Body = ms;

        return this;
    }

    /// <summary>
    /// Sets the body of the request to a stream containing the given object
    /// </summary>
    /// <typeparam name="T">The type of the object</typeparam>
    /// <param name="obj">The object to serialize</param>
    /// <returns>Http Request Builder</returns>
    public HttpRequestBuilder SetBody<T>(T obj) where T : new()
    {
        var objJson = JsonSerializer.Serialize(obj, GeneralConstants.JsonOptions);

        return SetBody(objJson);
    }

    /// <summary>
    /// Return the HttpRequest from the builder actions
    /// </summary>
    /// <returns></returns>
    public HttpRequest ToRequest()
    {
        if (headerCollectionBuilderDictionary.Any())
        {
            foreach (var header in headerCollectionBuilderDictionary)
            {
                request.Headers.Add(header.Key, new StringValues(header.Value.ToArray()));
            }
        }

        request.Method = httpMethod?.ToString();

        return request;
    }

    public HttpContent ToHttpContent()
    {
        return new StreamContent(request.Body);
    }

    public HttpRequestMessage ToRequestMessage()
    {
        var message = new HttpRequestMessage(httpMethod, requestUrl)
        {
            Content = new StringContent(bodyJson, Encoding.UTF8, "application/json"),
            RequestUri = new Uri(requestUrl, UriKind.RelativeOrAbsolute)
        };

        return message;
    }
}