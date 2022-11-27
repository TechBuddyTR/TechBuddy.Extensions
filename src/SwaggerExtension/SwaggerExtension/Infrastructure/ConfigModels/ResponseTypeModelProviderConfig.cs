using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TechBuddy.Extensions.OpenApi.Infrastructure.ConfigModels;

/// <summary>
/// The config to customize how our endpoints will return back the response
/// </summary>
public class ResponseTypeModelProviderConfig
{
    private readonly List<Type> DefaultTypesConsts
        = new Type[]
        { typeof(Task), typeof(Task<IActionResult>), typeof(Task<ActionResult>), typeof(IActionResult), typeof(ActionResult) }.ToList();

    internal List<HttpStatusCode> DefaultHttpStatusCodes = new()
    {
        HttpStatusCode.OK, HttpStatusCode.BadRequest, HttpStatusCode.InternalServerError
    };

    /// <summary>
    /// The list of DefaultHttpMethods to be used for mapping HttpMethod and HttpStatusCode in <see cref="DefaultStatusCodes"/>
    /// </summary>
    public List<HttpMethod> DefaultHttpMethods { get; }
        = new[] { HttpMethod.Get, HttpMethod.Post, HttpMethod.Put, HttpMethod.Patch, HttpMethod.Head, HttpMethod.Delete }.ToList();

    /// <summary>
    /// The list of Default Response Types for which we'll set the <see cref="DefaultStatusCodes"/>
    /// </summary>
    /// <example>IActionResult or Task<IActionResult>. In this case, we'll set the DefaultStatus code for these endpoints. Otherwise, we'll use the ResponseType of the endpoint</IActionResult></example>
    public List<Type> DefaultTypes { get; private set; }

    /// <summary>
    /// The list of DefaultHttpStatus codes that ALL your endpoints with provided HttpMethod will return. No specific model here. Only the status code.
    /// To specify the type use <see cref="TypeBaseStatusCodes"/>
    /// </summary>
    public List<DefaultHttpStatusCodeForHttpMethod> DefaultStatusCodes { get; private set; }

    /// <summary>
    /// The Dictionary of type and httpstatus codes.
    /// </summary>
    public Dictionary<HttpStatusCode, Type> TypeBaseStatusCodes { get; private set; }

    internal List<string> ExcludedActions { get; set; }
    internal List<string> ExcludedControllers { get; set; }


    /// <summary>
    /// The constructor sets the internal lists and also generate default values for <see cref="DefaultTypes"/> and <see cref="DefaultStatusCodes"/> depending on parameters
    /// </summary>
    /// <param name="generateDefaultTypes">set true to generate default types as [Task, IActionResult, ActionResult]</param>
    /// <param name="generateDefaultStatusCodes">set true to generate default HttpStatusCode as [200(OK), 400(BadRequest)]</param>
    public ResponseTypeModelProviderConfig(bool generateDefaultTypes = true,
                                           bool generateDefaultStatusCodes = true)
    {
        DefaultTypes = generateDefaultTypes
                        ? DefaultTypesConsts
                        : new List<Type>();

        DefaultStatusCodes = generateDefaultStatusCodes
                            ? GenerateDefaultHttpStatusCodesForHttpMethods()
                            : new List<DefaultHttpStatusCodeForHttpMethod>();

        TypeBaseStatusCodes = new Dictionary<HttpStatusCode, Type>();
        ExcludedActions = new List<string>();
        ExcludedControllers = new List<string>();
    }

    /// <summary>
    /// Creates the config with default values
    /// </summary>
    /// <returns>returns config</returns>
    public static ResponseTypeModelProviderConfig CreateDefault() => new ResponseTypeModelProviderConfig(true, true);

    /// <summary>
    /// Adds the provided HttpStatusCode as Response Code for all HttpMethods. Also checks if <see cref="TypeBaseStatusCodes"/> has the same HttpStatusCode
    /// </summary>
    /// <param name="statusCode">The statuscode</param>
    /// <returns>returns config</returns>
    /// <exception cref="ArgumentException">thrown when status code is duplice, <see cref="TypeBaseStatusCodes"/> has the same StatusCode </exception>
    public ResponseTypeModelProviderConfig AddDefaultResponseHttpStatusCodeForAll(HttpStatusCode statusCode)
    {
        foreach (var httpMethod in DefaultHttpMethods)
        {
            AddDefaultResponseHttpStatusCodeForHttpMethods(httpMethod, statusCode);
        }

        return this;
    }

    /// <summary>
    /// Adds the provided HttpStatusCode as Response Code for all endpoints that has provided HttpMethod. Also checks if <see cref="TypeBaseStatusCodes"/> has the same HttpStatusCode
    /// </summary>
    /// <param name="statusCode">The statuscode</param>
    /// <param name="httpMethod">The httpStatusCode</param>
    /// <returns>returns config</returns>
    /// <exception cref="ArgumentException">thrown when status code is duplice, <see cref="TypeBaseStatusCodes"/> has the same StatusCode </exception>
    public ResponseTypeModelProviderConfig AddDefaultResponseHttpStatusCodeForHttpMethods(HttpMethod httpMethod,
                                                                                          HttpStatusCode statusCode)
    {
        if (DefaultStatusCodes.Any(i => i.HttpMethod == httpMethod && i.HttpStatusCode == statusCode))
            throw new ArgumentException($"{httpMethod} for {statusCode} is already added");

        DefaultStatusCodes.Add(new DefaultHttpStatusCodeForHttpMethod(httpMethod, statusCode));

        return this;
    }

    /// <summary>
    /// Clears the list of the added default HttpStatusCode
    /// </summary>
    public void ClearDefaultResponseHttpStatusCodes()
    {
        DefaultStatusCodes.Clear();
    }

    /// <summary>
    /// Adds the provided type as default response type of any action method
    /// </summary>
    /// <param name="type">The response type</param>
    /// <returns>returns config</returns>
    /// <exception cref="ArgumentException">thrown when the provided type is already added</exception>
    public ResponseTypeModelProviderConfig AddDefaultType(Type type)
    {
        if (DefaultTypes.Contains(type))
            throw new ArgumentException($"{type} is already added");

        DefaultTypes.Add(type);
        return this;
    }

    /// <summary>
    /// Adds provided types as default response type of any action method
    /// </summary>
    /// <param name="types">The multiple response types</param>
    /// <returns>returns config</returns>
    /// <exception cref="ArgumentException">thrown when the provided type is already added</exception>
    public ResponseTypeModelProviderConfig AddDefaultType(params Type[] types)
    {
        foreach (var type in types)
        {
            AddDefaultType(type);
        }

        return this;
    }

    /// <summary>
    /// Clears the DefaultTypes.
    /// </summary>
    public void ClearDefaultTypes()
    {
        DefaultTypes.Clear();
    }

    /// <summary>
    /// Adds the specific type for a result when the HttpStatusCode in result is <paramref name="statusCode"/>
    /// </summary>
    /// <param name="statusCode">The statuscode you want to check</param>
    /// <param name="type">The type to return when statuscode matchs</param>
    /// <returns>returns config</returns>
    /// <exception cref="ArgumentException">thrown when the provided type is already added</exception>
    public ResponseTypeModelProviderConfig AddSpecificTypeForSpecificHttpStatusCode(HttpStatusCode statusCode, Type type)
    {
        if (TypeBaseStatusCodes.ContainsValue(type))
            throw new ArgumentException($"{type.Name} is already added");

        TypeBaseStatusCodes.Add(statusCode, type);

        return this;
    }


    #region Exclude Methods

    /// <summary>
    /// Excludes the action method from this Provider's context. This action will have no ResponseType
    /// </summary>
    /// <param name="controllerName">The name of the controller that action is belong to</param>
    /// <param name="actionName">The name of the action method</param>
    /// <returns>returns config</returns>
    /// <exception cref="ArgumentException">thrown when controller or action name is empty or added twice</exception>
    public ResponseTypeModelProviderConfig ExcludeAction(string controllerName, string actionName)
    {
        ArgumentNullException.ThrowIfNull(controllerName, nameof(controllerName));
        ArgumentNullException.ThrowIfNull(actionName, nameof(actionName));

        var fullName = $"{controllerName}.{actionName}";

        return ExcludeAction(fullName);
    }

    /// <summary>
    /// Excludes the action method from this Provider's context. This action will have no ResponseType
    /// </summary>
    /// <param name="actionName">The name of the action method. This can be used like nameof(TestController.TestAction)</param>
    /// <returns>returns config</returns>
    /// <exception cref="ArgumentException">thrown when <paramref name="actionName"/> is null or added twice</exception>
    public ResponseTypeModelProviderConfig ExcludeAction(string actionName)
    {
        ArgumentNullException.ThrowIfNull(actionName, nameof(actionName));

        if (ExcludedActions.Contains(actionName))
            throw new ArgumentException($"{actionName} is already added");

        ExcludedActions.Add(actionName);

        return this;
    }

    /// <summary>
    /// Excludes any controller from this Provider's context. Any action methods under this controller will have no ResponseType
    /// </summary>
    /// <param name="controllerName">The name of the controller. This can be used like nameof(TestController)</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException">thrown when controller name is empty or added twice</exception>
    public ResponseTypeModelProviderConfig ExcludeController(string controllerName)
    {
        ArgumentNullException.ThrowIfNull(controllerName, nameof(controllerName));

        if (ExcludedControllers.Contains(controllerName))
            throw new ArgumentException($"{controllerName} is already added");

        ExcludedControllers.Add(controllerName);

        return this;
    }

    #endregion

    #region Private Methods

    private List<DefaultHttpStatusCodeForHttpMethod> GenerateDefaultHttpStatusCodesForHttpMethods()
    {
        // Cartesian
        var result = DefaultHttpMethods
                        .Join(DefaultHttpStatusCodes, m => true, s => true, (m, n) => new { HttpMethod = m, StatusCode = n })
                        .Select(i => new DefaultHttpStatusCodeForHttpMethod(i.HttpMethod, i.StatusCode))
                        .ToList();

        return result;
    }

    #endregion
}
