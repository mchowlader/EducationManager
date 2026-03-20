using FluentValidation;

namespace EduManager.Application.DTOs.Feature.RoleFeature.Validator;

internal class RoleCreateValidator : AbstractValidator<RoleCreateDto>
{
    public RoleCreateValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Role name is required.")
            .MaximumLength(100).WithMessage("Role name must not exceed 100 charecters.");
    }
}
