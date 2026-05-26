using Ecommerce.API.DTOs.Auth;
using FluentValidation;

namespace Ecommerce.API.Validators
{
    public class RegisterDtoValidator : AbstractValidator<RegisterDto>
    {
        public RegisterDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is required")
                
                .MinimumLength(3)
                .WithMessage("Name must be atleast 3 characters");


            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required")

                .EmailAddress()
                .WithMessage("Invalid Email format");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password is required")

                .MinimumLength(6)
                .WithMessage("Password must be atleast 6 characters")

                .Matches("[A-Z]")
                .WithMessage("Password must contain at least one uppercase letter")

                .Matches("[a-z]")
                .WithMessage("Password must contain at least one lowercase letter")

                .Matches("[0-9]")
                .WithMessage("Password must contain at least one digit")

                .Matches("[^a-zA-Z0-9]")
                .WithMessage("Password must contain at least one special character");
        }
    }
}
