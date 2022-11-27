using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TechBuddy.Extensions.OpenApi.Infrastructure.ConfigModels;

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
