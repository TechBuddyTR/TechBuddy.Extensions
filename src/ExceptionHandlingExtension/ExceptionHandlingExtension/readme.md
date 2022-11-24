# Exception Handling

Exception Handling is giving you the opportunity to manage all *unhandled* exceptions in a single point. By implementing this extension in your WebAPI project, 
you'll be able to catch all unhandled exceptions and return back a standard exception model as well as you'll be able to provide your own handler method which will be called once any unhandled exception is occured.

<br>
Apart from that, you can customize logging by providing an ILogger interface to log the details once the exception is cought by the handler. 
`UseExceptionDetails` is important because once the default handler is used, it'll return back the exception details. 
If `UseExceptionDetails` is true, it returns the StackTrace of the Exception. 
When it is false, however, it returns the single message ("Internal Server Error!") and 500 StatusCode. So you can use this details in your Dev or PreProd environment but not on production. 
On the other hand, logging the details is irrelevant to this parameter. No matter it is true or false, it logs everything with StackTrace.  

### Usage 1  

```csharp

app.ConfigureTechBuddyExceptionHandling(opt =>
{
    var logger = app.Services.GetService<ILogger>();
    opt.UseExceptionDetails = true; // details will be in the default response model (`DefaultExceptionHandlerResponseModel`)
    opt.UseLogger(logger); // details will be logged not matter `UseExceptionDetails` is true or false
});

```

### Usage 2  

In this usage, you must provide a handler function which will be invoked when the exception is occured. From now on, all the responsibility is on you.

```csharp
opt.UseCustomHandler((httpContext, exception, logger) => 
{
    logger.LogError("Unhandled exception occured");
    var dynamicResponseModel = new { ErrorMessage = exception.Message };

    // we can set the response but don't have to
    return httpContext.Response.WriteAsJsonAsync(dynamicResponseModel);
});
```

### Usage 3

In this usage, UseExceptionDetails is set to true and also default ILogger is used if it's enabled in the system (app.ApplicationServices.GetService<ILogger>())

```csharp

app.ConfigureTechBuddyExceptionHandling();

```


### Usage 4

In this usage, you are also able to use different handlers for different type of exceptions.

```csharp

var options = new ExceptionHandlingOptions { UseExceptionDetails = false };

options.AddCustomHandler<ValidationException>((context, ex, logger) =>
{
    logger.LogError("Unhandled exception occured");
    var dynamicResponseModel = new { ErrorMessage = ex.Message, Type = "ValidationException" };

    // we can set the response but don't have to
    return httpContext.Response.WriteAsJsonAsync(dynamicResponseModel);
});


options.AddCustomHandler<ArgumentNullException>((context, ex, logger) =>
{
    logger.LogError("ArgumentNullException occured");
    var dynamicResponseModel = new { ErrorMessage = ex.Message, Type = "ArgumentNullException" };

    // we can set the response but don't have to
    return httpContext.Response.WriteAsJsonAsync(dynamicResponseModel);
});


// All the other exceptions
opt.UseCustomHandler(async (context, ex, logger) =>
{
    logger.LogError("Unhandled exception occured");
    var dynamicResponseModel = new { ErrorMessage = ex.Message, Type = "Exception" };

    // we can set the response but don't have to
    return httpContext.Response.WriteAsJsonAsync(dynamicResponseModel);
});

app.ConfigureTechBuddyExceptionHandling(options);

```