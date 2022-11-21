namespace TechBuddy.Extension.Validation.Infrastructure.Models.ResponseModels;

/// <summary>
/// The default ValidationErrorResponseModel
/// </summary>
public class DefaultValidationErrorResponseModel : BaseValidationErrorResponseModel
{
    /// <summary>
    /// The constructor
    /// </summary>
    /// <param name="errorMessage">The exception message</param>
    public DefaultValidationErrorResponseModel(string errorMessage)
        : base(new List<string> { errorMessage })
    {
    }

    /// <summary>
    /// The constructor with list of exception
    /// </summary>
    /// <param name="errorMessage">The List of exception</param>
    public DefaultValidationErrorResponseModel(IEnumerable<string> errorMessage)
        : base(errorMessage.ToList())
    {
    }
}
