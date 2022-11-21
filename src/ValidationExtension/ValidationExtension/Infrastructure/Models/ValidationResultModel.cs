using TechBuddy.Extension.Validation.Infrastructure.Models.ResponseModels;

namespace TechBuddy.Extension.Validation.Infrastructure.Models;

/// <summary>
/// The default <see cref="ValidationResultModel"/>
/// </summary>
/// <typeparam name="T">The generic type for <see cref="Model"/></typeparam>
public class ValidationResultModel<T> : ValidationResultModel where T : new()
{
    /// <summary>
    /// The instance of validated and casted model
    /// </summary>
    public T Model { get; set; }
}

/// <summary>
/// The <see cref="ValidationResultModel"/>
/// </summary>
public class ValidationResultModel : BaseValidationErrorResponseModel
{
    /// <summary>
    /// The parameterless constructor that sets Errors to null/>
    /// </summary>
    public ValidationResultModel()
        : this(null)
    {

    }

    /// <summary>
    /// The constructor that sets Errors/>
    /// </summary>
    /// <param name="errors">The list of error</param>
    public ValidationResultModel(List<string> errors) : base(errors)
    {
    }

    /// <summary>
    /// Shows if the validation is successful
    /// </summary>
    public bool IsValid { get; set; }
}
