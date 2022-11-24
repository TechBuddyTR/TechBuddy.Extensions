using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TechBuddy.Extensions.AspNetCore.ExceptionHandling.Infrastructure.ExceptionHandlers;

namespace TechBuddy.Extensions.AspNetCore.ExceptionHandling;

/// <summary>
/// The ExceptionHandling extension methods for WebApi projects
/// </summary>
public static class ExceptionHandlingDependencyInjectionExtensions
{
    /// <summary>
    /// Configures the ExceptionHandling Middleware with default opt.
    /// With default opt, Logging with details will be enabled
    /// </summary>
    /// <param name="app">The IApplicationBuilder</param>
    /// <returns>The IApplicationBuilder</returns>
    public static Task ConfigureTechBuddyExceptionHandling(this IApplicationBuilder app)
    {
        return app.ConfigureTechBuddyExceptionHandling(options =>
        {
            options.UseLogger(true);
        });
    }

    /// <summary>
    /// Configures the ExceptionHandling Middleware with provided opt.
    /// </summary>
    /// <param name="app">The IApplicationBuilder</param>
    /// <param name="optionsAction">The ExceptionHandlingConfig Action</param>
    /// <returns>The IApplicationBuilder</returns>
    public static Task ConfigureTechBuddyExceptionHandling(this IApplicationBuilder app,
        Action<ExceptionHandlingOptions> optionsAction)
    {
        ExceptionHandlingOptions opt = new();
        optionsAction(opt); // Fill the options

        return ConfigureTechBuddyExceptionHandling(app, opt);
    }

    /// <summary>
    /// Configures the ExceptionHandling Middleware with provided opt.
    /// </summary>
    /// <param name="app">The IApplicationBuilder</param>
    /// <param name="opt">The ExceptionHandlingConfig</param>
    /// <returns>The IApplicationBuilder</returns>
    public static async Task ConfigureTechBuddyExceptionHandling(this IApplicationBuilder app,
        ExceptionHandlingOptions opt)
    {
        var logger = opt.Logger
            ?? (opt.LoggingEnabled
                    ? app.ApplicationServices.GetService<ILogger>()
                    : null);

        app.UseExceptionHandler(options =>
        {
            if (opt.ExceptionHandler is not null)
            {
                RegisterHandlers(options,
                                 logger,
                                 opt.ExceptionHandler,
                                 opt);
            }
            else
            {
                RegisterHandlers(options,
                                logger,
                                (c, e, l) => DefaultExceptionHandler.Handle(c, e, l, opt.UseExceptionDetails),
                                opt);
            }
        });
    }


    private static Task RegisterHandlers(IApplicationBuilder app,
                                         ILogger logger,
                                         Func<HttpContext, Exception, ILogger, Task> exceptionHandler,
                                         ExceptionHandlingOptions options)
    {
        app.Run(context =>
        {
            var exceptionObject = context.Features.Get<IExceptionHandlerFeature>();
            var type = exceptionObject.Error.GetType();
            options.ExceptionHandlersDictionary.TryGetValue(type, out var handler);

            if (handler is not null)
            {
                handler.Invoke(context, exceptionObject.Error, logger);
            }
            else
            {
                exceptionHandler.Invoke(context, exceptionObject.Error, logger);
            }

            return Task.CompletedTask;
        });

        return Task.CompletedTask;
    }



}
