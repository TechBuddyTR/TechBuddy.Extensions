using Microsoft.AspNetCore.Mvc;

namespace ValidationExtension.Tests.Tests;
public sealed class ValidationErrorResponseFactoryTests
{
    [Test]
    public void CreateModel_WithMessage_ShouldContainMessage()
    {
        // Arrange
        var message = "TestMessage";

        // Action
        var model = ValidationErrorResponseFactory.CreateModel(message);

        // Assert
        model.Errors.Should().Contain(message);
        model.Errors.Should().HaveCount(1);
    }


    [Test]
    public void CreateModel_WithListOfMessage_ShouldContainMessages()
    {
        // Arrange
        var messages = new List<string>()
        {
            "TestMessage1",
            "TestMessage2"
        };

        // Action
        var model = ValidationErrorResponseFactory.CreateModel(messages);

        // Assert
        model.Errors.Should().BeEquivalentTo(messages);
        model.Errors.Should().HaveCount(messages.Count);
    }


    [Test]
    public void CreateActionResult_WithMessage_ShouldReturnBadRequestObject()
    {
        // Arrange
        var message = "TestMessage";

        // Action
        var actionResult = ValidationErrorResponseFactory.CreateActionResult(message);

        // Assert
        actionResult.Should().BeOfType<BadRequestObjectResult>();
    }

    [Test]
    public void CreateActionResult_WithMessage_ShouldContainMessageAsObject()
    {
        // Arrange
        var message = "TestMessage";

        // Action
        var actionResult = ValidationErrorResponseFactory.CreateActionResult(message);

        // Assert
        var value = (actionResult as BadRequestObjectResult).Value;

        value.Should().BeOfType<DefaultValidationErrorResponseModel>();
        value.As<DefaultValidationErrorResponseModel>().Errors.Contains(message);
    }
}
