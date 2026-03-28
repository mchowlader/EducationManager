using FluentValidation;

namespace EduManager.Application.DTOs.Feature.ClassesFeature.Validators;

public class ClassesUpdateValidator : AbstractValidator<UpdateClassesDto>
{
    public ClassesUpdateValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(100).WithMessage("Class name must not exceed 100 characters.")
            .When(x => x.Name is not null);

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters.")
            .When(x => x.Description is not null);
    }
}
