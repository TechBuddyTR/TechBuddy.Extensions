using System.Net;

namespace TechBuddy.Extensions.OpenApi;

/// <summary>
/// The DefaultHttpStatusCodeForHttpMethod to save HttpStatusCode for HttpMethod
/// </summary>
public class DefaultHttpStatusCodeForHttpMethod
{
    /// <summary>
    /// The consturctor to create this class
    /// </summary>
    /// <param name="httpMethod">The http method</param>
    /// <param name="httpStatusCode">The http status code</param>
    public DefaultHttpStatusCodeForHttpMethod(HttpMethod httpMethod, HttpStatusCode httpStatusCode)
    {
        HttpMethod = httpMethod;
        HttpStatusCode = httpStatusCode;
    }

    /// <summary>
    /// The http method you want to add the status code for
    /// </summary>
    public HttpMethod HttpMethod { get; set; }

    /// <summary>
    /// The http status code
    /// </summary>
    public HttpStatusCode HttpStatusCode { get; set; }
}
