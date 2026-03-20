using FluentValidation;

namespace EduManager.Application.DTOs.Feature.RoleFeature.Validator;

public class RoleUpdateValidator : AbstractValidator<RoleUpdateDto>
{
    public RoleUpdateValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(100).WithMessage("Role name must not exceed 100 characters.")
            .When(x => x.Name is not null);
    }
}