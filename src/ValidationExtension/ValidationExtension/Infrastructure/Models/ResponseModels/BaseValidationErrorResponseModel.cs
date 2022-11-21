namespace TechBuddy.Extension.Validation.Infrastructure.Models.ResponseModels;

/// <summary>
/// BaseInvalidResponse Model
/// </summary>
public abstract class BaseValidationErrorResponseModel
{
    /// <summary>
    /// The parameterless contsturctor
    /// </summary>
    public BaseValidationErrorResponseModel()
    {

    }


    /// <summary>
    /// The constructor that sets the <see cref="Errors"/>
    /// </summary>
    /// <param name="errors"></param>
    protected BaseValidationErrorResponseModel(List<string> errors)
    {
        Errors = errors;
    }

    /// <summary>
    /// List of exceptions occured
    /// </summary>
    public List<string> Errors { get; set; }
}