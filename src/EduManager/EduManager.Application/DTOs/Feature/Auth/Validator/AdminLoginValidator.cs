using FluentValidation;

namespace EduManager.Application.DTOs.Feature.Auth.Validator;

public class AdminLoginValidator : AbstractValidator<AdminLoginDto>
{
    public AdminLoginValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(10);
    }
}
