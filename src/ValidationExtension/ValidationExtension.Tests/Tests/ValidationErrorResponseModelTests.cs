namespace ValidationExtension.Tests.Tests;
public sealed class ValidationErrorResponseModelTests
{
    [Test]
    public void WithMessage_ShouldContainMessage()
    {
        // Arrange
        var message = "TestMessage";

        // Action
        var model = new DefaultValidationErrorResponseModel(message);

        // Assert
        model.Errors.Should().Contain(message);
        model.Errors.Should().HaveCount(1);
    }


    [Test]
    public void WithListOfMessage_ShouldContainMessages()
    {
        // Arrange
        var messages = new List<string>()
        {
            "TestMessage1",
            "TestMessage2"
        };

        // Action
        var model = new DefaultValidationErrorResponseModel(messages);

        // Assert
        model.Errors.Should().BeEquivalentTo(messages);
        model.Errors.Should().HaveCount(messages.Count);
    }
}
