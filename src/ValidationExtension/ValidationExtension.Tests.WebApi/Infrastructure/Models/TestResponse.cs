using TechBuddy.Extension.Validation.Infrastructure.Models.ResponseModels;

public class TestResponse : BaseValidationErrorResponseModel
{
    public int HttpStatusCode { get; set; }
}
