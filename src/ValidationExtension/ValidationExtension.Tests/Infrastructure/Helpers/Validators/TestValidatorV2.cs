namespace ValidationExtension.Tests.Helpers.Validators;

public sealed class TestValidatorV2 : AbstractValidator<TestModelV2>
{
    public TestValidatorV2()
    {
        RuleFor(i => i.Id).GreaterThan(0).WithMessage("{PropertyName} cannot be zero!");
        RuleFor(i => i.FullName).MinimumLength(3).WithMessage("{PropertyName} must be at least {MinLenght} character");
    }
}
