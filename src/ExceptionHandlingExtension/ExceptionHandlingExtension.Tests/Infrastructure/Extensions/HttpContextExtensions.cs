using Microsoft.AspNetCore.Http;
using TechBuddy.Extensions.Tests.Common.TestCommon.Constants;
using System.Net;
using System.Text;
using System.Text.Json;

namespace ExceptionHandlingExtension.Tests.Infrastructure.Extensions;
internal static class HttpContextExtensions
{
    internal static async Task WriteResponseAsync(this HttpContext context, object resultObj, HttpStatusCode statusCode)
    {
        context.Response.ContentType = TestConstants.ContentType;
        context.Response.StatusCode = (int)statusCode;

        var json = JsonSerializer.Serialize(resultObj, GeneralConstants.JsonOptions);
        await context.Response.WriteAsync(json, Encoding.UTF8);
    }
}
