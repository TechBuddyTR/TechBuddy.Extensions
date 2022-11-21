using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace TechBuddy.Extension.Validation.Infrastructure.Models.ModelProviders;

/// <summary>
/// The default model provider interface that promise to return an object for ResponseBody
/// </summary>
public interface IDefaultModelProvider
{
    /// <summary>
    /// Get the object model to be writting as ResponseBody.
    /// </summary>
    /// <param name="modelStateValues">The <see cref="ModelStateDictionary.ValueEnumerable"/> argument in which you can find validation exception details </param>
    /// <returns>returns ResponseBody object</returns>
    object GetModel(ModelStateDictionary.ValueEnumerable modelStateValues);
}