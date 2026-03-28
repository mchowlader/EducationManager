using FluentValidation;

namespace EduManager.Application.DTOs.Feature.TeacherFeature.Validator;

public class TeacherUpdateValidator : AbstractValidator<UpdateTeacherDto>
{
    public TeacherUpdateValidator()
    {
        RuleFor(x => x.FullName)
            .MaximumLength(200)
            .When(x => x.FullName is not null);

        RuleFor(x => x.Mobile)
            .MaximumLength(20)
            .When(x => x.Mobile is not null);

        RuleFor(x => x.Designation)
            .MaximumLength(100)
            .When(x => x.Designation is not null);
    }
}