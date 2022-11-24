using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using TechBuddy.Extensions.AspNetCore.ExceptionHandling.Infrastructure.Extensions;
using TechBuddy.Extensions.AspNetCore.ExceptionHandling.Infrastructure.Models;

namespace TechBuddy.Extensions.AspNetCore.ExceptionHandling.Infrastructure.ExceptionHandlers;
internal class DefaultExceptionHandler
{
    internal static async Task Handle(HttpContext context,
                                        Exception exception,
                                        ILogger logger,
                                        bool useExceptionDetails = false)
    {

        var res = new DefaultExceptionHandlerResponseModel()
        {
            StatusCode = System.Net.HttpStatusCode.InternalServerError,
            Detail = useExceptionDetails
                        ? exception.ToString()
                        : ExceptionHandlingConstants.DefaultExceptionMessage
        };

        logger?.LogError(exception, exception.ToString());
        await context.WriteResponseAsync(res, res.StatusCode);
    }
}
