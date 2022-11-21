using Microsoft.AspNetCore.Mvc;
using TechBuddy.Extension.Validation.Infrastructure.Models.ResponseModels;

namespace TechBuddy.Extension.Validation.Infrastructure.Factories;

/// <summary>
/// The ValidationErrorReponseFactory class
/// </summary>
public sealed class ValidationErrorResponseFactory
{
    /// <summary>
    /// Gets BadRequestObjectResults that has the body as <see cref="DefaultValidationErrorResponseModel"/> with the provided <paramref name="message"/>
    /// </summary>
    /// <param name="message">The validation message</param>
    /// <returns>returns <see cref="BadRequestObjectResult"/></returns>
    public static IActionResult CreateActionResult(string message)
    {
        var model = new DefaultValidationErrorResponseModel(message);

        return new BadRequestObjectResult(model);
    }

    /// <summary>
    /// Gets <see cref="DefaultValidationErrorResponseModel"/> with <paramref name="message"/>
    /// </summary>
    /// <param name="message">The validation message</param>
    /// <returns>returns <see cref="DefaultValidationErrorResponseModel"/> </returns>
    public static DefaultValidationErrorResponseModel CreateModel(string message)
    {
        return new DefaultValidationErrorResponseModel(message);
    }

    /// <summary>
    /// Gets <see cref="DefaultValidationErrorResponseModel"/> with <paramref name="messages"/> as validation error messages
    /// </summary>
    /// <param name="messages">The list of validation message</param>
    /// <returns>returns <see cref="DefaultValidationErrorResponseModel"/> </returns>
    public static DefaultValidationErrorResponseModel CreateModel(IEnumerable<string> messages)
    {
        return new DefaultValidationErrorResponseModel(messages?.ToList());
    }
}
