using FluentValidation;
using Microsoft.AspNetCore.Http;
using System.Text;
using System.Text.Json;
using TechBuddy.Extension.Validation.Infrastructure.Models;

namespace TechBuddy.Extension.Validation.Extensions;

/// <summary>
/// The HttpRequest Extensions
/// </summary>
public static class HttpRequestExtensions
{
    /// <summary>
    /// Validates the object from http request body if it is <typeparamref name="TModel"/>, with provided <typeparamref name="TValidator"/>
    /// </summary>
    /// <typeparam name="TModel">The model which the body object will be compared to</typeparam>
    /// <typeparam name="TValidator">The validator (FluentValidator used)</typeparam>
    /// <param name="req">The HttpRequest to get the body object</param>
    /// <returns></returns>
    public static async Task<ValidationResultModel<TModel>> ValidateAsync<TModel, TValidator>(this HttpRequest req)
        where TModel : new()
        where TValidator : IValidator<TModel>
    {
        ArgumentNullException.ThrowIfNull(req);

        if (req.Body.Length <= 0)
            throw new ArgumentException("Request body cannot be empty!");

        var validator = Activator.CreateInstance<TValidator>();

        var result = new ValidationResultModel<TModel>
        {
            Model = await req.ReadBodyAsync<TModel>()
        };

        var validatorResult = await validator.ValidateAsync(result.Model);

        result.IsValid = validatorResult.IsValid;

        if (!result.IsValid)
            result.Errors = validatorResult.Errors.Select(i => i.ErrorMessage).ToList();

        return result;
    }


    /// <summary>
    /// Reads and returns the generic body object from httprequest by serializing to provided generic type
    /// </summary>
    /// <typeparam name="T">The type for object to be serialized</typeparam>
    /// <param name="req">The http request to get the body object</param>
    /// <returns>The generic object</returns>
    private static async Task<T> ReadBodyAsync<T>(this HttpRequest req)
    {
        req.EnableBuffering();
        req.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(req.Body, Encoding.UTF8, leaveOpen: true);
        var jsonBody = await reader.ReadToEndAsync();
        req.Body.Position = 0;

        return JsonSerializer.Deserialize<T>(jsonBody);
    }


}
