using FluentValidation;

namespace EduManager.Application.DTOs.Feature.TeacherFeature.Validator;

public class TeacherCreateValidator : AbstractValidator<CreateTeacherDto>
{
    public TeacherCreateValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Valid email is required.")
            .MaximumLength(100);

       RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters.");

        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required.")
            .MaximumLength(200);

        RuleFor(x => x.Mobile)
            .NotEmpty().WithMessage("Mobile number is required.")
            .MaximumLength(11)
            .Matches(@"^01\d{9}$")
            .WithMessage("Mobile number must start with 01 and be 11 digits.");

        RuleFor(x => x.Designation)
            .NotEmpty().WithMessage("Designation is required.")
            .MaximumLength(100);

        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage("Date of birth is required.")
            .LessThan(DateTime.UtcNow).WithMessage("Date of birth must be in the past.");

    }
}
