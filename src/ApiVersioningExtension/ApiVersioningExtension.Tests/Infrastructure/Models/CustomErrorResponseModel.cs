namespace ApiVersioningExtension.Tests.Infrastructure.Models;
internal class CustomErrorResponseModel
{
    public string ErrorCode { get; set; }

    public string ErrorMessage { get; set; }

    public string ErrorDetail { get; set; }
}
