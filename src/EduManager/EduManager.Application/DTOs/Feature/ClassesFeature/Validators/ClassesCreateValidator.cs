using FluentValidation;

namespace EduManager.Application.DTOs.Feature.ClassesFeature.Validators;

public class ClassesCreateValidator : AbstractValidator<CreateClassesDTO>
{
    public ClassesCreateValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Class name is required.")
            .MaximumLength(100).WithMessage("Class name must not exceed 100 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters.")
            .When(x => x.Description is not null);
    }
}
