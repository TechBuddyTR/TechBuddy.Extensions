using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace TechBuddy.Extensions.AspNetCore.ExceptionHandling;

/// <summary>
/// The options model for ExceptionHandling
/// </summary>

public class ExceptionHandlingOptions
{
    internal Dictionary<Type, Func<HttpContext, Exception, ILogger, Task>> ExceptionHandlersDictionary;
    internal Func<HttpContext, Exception, ILogger, Task> ExceptionHandler;
    internal ILogger Logger { get; set; }

    /// <summary>
    /// The parameterless constructor
    /// </summary>
    public ExceptionHandlingOptions()
    {
        ExceptionHandlersDictionary = new Dictionary<Type, Func<HttpContext, Exception, ILogger, Task>>();
    }

    /// <summary>
    /// Gets whether the logging is enabled when either <see cref="UseLogger()"/> or <see cref="UseLogger{T}(T)"/> is called
    /// </summary>
    public bool LoggingEnabled { get; internal set; } = false;

    /// <summary>
    /// Gets or sets whether the stack trace of the exception will be present in the response model
    /// </summary>
    public bool UseExceptionDetails { get; set; }

    /// <summary>
    /// Sets the provided <paramref name="logger"/> as the logger in the default exception handler
    /// </summary>
    /// <typeparam name="T">The type of logger</typeparam>
    /// <param name="logger">The logger to be used in default exception handler</param>
    public void UseLogger<T>(T logger) where T : ILogger
    {
        Logger = logger;
        UseLogger();
    }

    /// <summary>
    /// Enables the default ILogger for the default exception handler
    /// </summary>
    public void UseLogger()
    {
        LoggingEnabled = true;
    }

    /// <summary>
    /// Sets the provided <paramref name="exceptionHandler"/> as custom handler.
    /// This functions will be called when any unhandled exception is occured
    /// </summary>
    /// <param name="exceptionHandler">The exception handler function</param>
    public void UseCustomHandler(Func<HttpContext, Exception, ILogger, Task> exceptionHandler)
    {
        ExceptionHandler = exceptionHandler;
    }

    /// <summary>
    /// Adds the provided handler for a specific exception types. For instance, you may want to handle differently when <see cref="UnauthorizedAccessException"/> is occured
    /// </summary>
    /// <typeparam name="TException">The specific exception</typeparam>
    /// <param name="handler">The custom handler</param>
    /// <exception cref="InvalidOperationException">Thrown when duplicate exception is added</exception>
    public void AddCustomHandler<TException>(Func<HttpContext, Exception, ILogger, Task> handler) where TException : Exception
    {
        var added = ExceptionHandlersDictionary.TryAdd(typeof(TException), handler);

        if (!added)
            throw new InvalidOperationException($"{typeof(TException)} is already added for another handler");
    }


}
