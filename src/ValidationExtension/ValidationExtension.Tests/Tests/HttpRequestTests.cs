using TechBuddy.Extension.Validation.Extensions;
using TechBuddy.Extensions.Tests.Common.TestCommon.Builders;
using TechBuddy.Extensions.Tests.Common.TestCommon.Constants;

namespace ValidationExtension.Tests.Tests;
public sealed class HttpRequestTests
{
    [Test]
    public async Task Validate_WithValidModel_ShouldValidate()
    {
        // Arrange
        var model = new TestModel()
        {
            Id = 1,
            Name = "Test"
        };

        var request = HttpRequestBuilder
                        .CreateRequest()
                        .SetBody(JsonSerializer.Serialize(model, GeneralConstants.JsonOptions))
                        .ToRequest();

        // Act
        var result = await request.ValidateAsync<TestModel, TestValidator>();


        // Assert
        result.IsValid.Should().BeTrue();
        result.Model.Should().BeEquivalentTo(model);
        result.Errors.Should().BeNullOrEmpty();
    }

    [Test]
    public async Task Validate_WithInvalidPropertyV1_ShouldNotValidate()
    {
        // Arrange
        var model = new TestModel()
        {
            Id = 0,
            Name = "Test"
        };

        var request = HttpRequestBuilder
                        .CreateRequest()
                        .SetBody(JsonSerializer.Serialize(model, GeneralConstants.JsonOptions))
                        .ToRequest();

        // Act
        var result = await request.ValidateAsync<TestModel, TestValidator>();


        // Assert
        result.IsValid.Should().BeFalse();
        result.Model.Should().BeEquivalentTo(model);
        result.Errors.Should().NotBeNullOrEmpty().And.HaveCount(1);
    }

    [Test]
    public async Task Validate_WithInvalidPropertyV2_ShouldNotValidate()
    {
        // Arrange
        var model = new TestModel()
        {
            Id = 0,
            Name = ""
        };

        var request = HttpRequestBuilder
                        .CreateRequest()
                        .SetBody(JsonSerializer.Serialize(model, GeneralConstants.JsonOptions))
                        .ToRequest();

        // Act
        var result = await request.ValidateAsync<TestModel, TestValidator>();


        // Assert
        result.IsValid.Should().BeFalse();
        result.Model.Should().BeEquivalentTo(model);
        result.Errors.Should().NotBeNullOrEmpty().And.HaveCount(2);
    }

    [Test]
    public async Task Validate_WhenHttpRequestNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        HttpRequest request = null;

        // Act
        var result = async () => await request.ValidateAsync<TestModel, TestValidator>();

        // Assert
        await result.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    public async Task Validate_WhenHttpRequestBodyEmpty_ShouldThrowArgumentNullException()
    {
        // Arrange
        var request = HttpRequestBuilder
                        .CreateRequest()
                        .ToRequest();

        // Act
        var result = async () => await request.ValidateAsync<TestModel, TestValidator>();

        // Assert
        await result.Should().ThrowAsync<ArgumentException>();
    }
}
