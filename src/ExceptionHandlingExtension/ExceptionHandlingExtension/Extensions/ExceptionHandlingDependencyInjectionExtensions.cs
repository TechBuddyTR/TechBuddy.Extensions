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
            options.UseLogger();
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
        ILogger logger = opt.Logger;

        if (logger is null && opt.LoggingEnabled)
        {
            logger = GetLoggerService(app.ApplicationServices);
        }

        app.UseExceptionHandler(async options =>
        {
            if (opt.ExceptionHandler is not null)
            {
                await RegisterHandlers(options,
                                 logger,
                                 opt.ExceptionHandler,
                                 opt);
            }
            else
            {
                await RegisterHandlers(options,
                                logger,
                                (c, e, l) => DefaultExceptionHandler.Handle(c, e, l, opt.UseExceptionDetails),
                                opt);
            }
        });

        await Task.CompletedTask;
    }


    private static ILogger GetLoggerService(IServiceProvider sp)
    {
        // Try to get ILogger first
        var serviceLogger = sp.GetService<ILogger>();

        if (serviceLogger is not null)
            return serviceLogger;

        // If no ILogger, try to create yours by using ILoggerFactory
        var logFactory = sp.GetService<ILoggerFactory>();
        serviceLogger = logFactory.CreateLogger<ExceptionHandlingOptions>();

        return serviceLogger;
    }

    private static async Task RegisterHandlers(IApplicationBuilder app,
                                         ILogger logger,
                                         Func<HttpContext, Exception, ILogger, Task> exceptionHandler,
                                         ExceptionHandlingOptions options)
    {
        app.Run(async context =>
        {
            var exceptionObject = context.Features.Get<IExceptionHandlerFeature>();
            var type = exceptionObject.Error.GetType();
            options.ExceptionHandlersDictionary.TryGetValue(type, out var handler);

            if (handler is not null)
            {
                await handler.Invoke(context, exceptionObject.Error, logger);
            }
            else
            {
                await exceptionHandler.Invoke(context, exceptionObject.Error, logger);
            }
        });

        await Task.CompletedTask;
    }



}
