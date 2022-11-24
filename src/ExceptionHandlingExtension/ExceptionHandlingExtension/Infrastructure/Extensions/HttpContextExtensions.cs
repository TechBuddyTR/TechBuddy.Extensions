using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text;
using System.Text.Json;
using TechBuddy.Extensions.AspNetCore.ExceptionHandling.Infrastructure;

namespace TechBuddy.Extensions.AspNetCore.ExceptionHandling.Infrastructure.Extensions;
internal static class HttpContextExtensions
{
    internal static async Task WriteResponseAsync(this HttpContext context, object resultObj, HttpStatusCode statusCode)
    {
        context.Response.ContentType = ExceptionHandlingConstants.DefaultResponseContentType;
        context.Response.StatusCode = (int)statusCode;

        var json = JsonSerializer.Serialize(resultObj, ExceptionHandlingConstants.JsonOptions);
        await context.Response.WriteAsync(json, Encoding.UTF8);
    }
}
