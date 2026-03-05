using FluentValidation;

namespace EduManager.Application.DTOs.Feature.TenantFeature.Validator;

public class TenantCreateValidator : AbstractValidator<CreateTenantDto>
{
    public TenantCreateValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(200);

        RuleFor(x => x.Slug)
            .NotEmpty().WithMessage("Slug is required")
            .Matches("^[a-z0-9-]+$")
            .WithMessage("Slug must contain only lowercase letters, numbers, and hyphens");

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress().WithMessage("Valid email required");

        RuleFor(x => x.Mobile)
            .NotEmpty()
            .Matches(@"^01[3-9][09]{8}$").WithMessage("Valid Bangladesh mobile number required"); ;

    }
}
