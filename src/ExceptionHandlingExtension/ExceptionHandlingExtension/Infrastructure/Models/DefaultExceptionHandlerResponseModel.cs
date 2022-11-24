using System.Net;

namespace TechBuddy.Extensions.AspNetCore.ExceptionHandling.Infrastructure.Models;

/// <summary>
/// The model to be used in when Exception handled but handler is not customized
/// </summary>
public class DefaultExceptionHandlerResponseModel
{
    /// <summary>
    /// The error detail
    /// </summary>
    public string Detail { get; set; }

    /// <summary>
    /// The http status code
    /// </summary>
    public HttpStatusCode StatusCode { get; set; }

    /// <summary>
    /// The parameterless constructor
    /// </summary>
    public DefaultExceptionHandlerResponseModel()
    {
        StatusCode = HttpStatusCode.InternalServerError;
    }
}
