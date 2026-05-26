using Ecommerce.API.DTOs.Auth;
using FluentValidation;

namespace Ecommerce.API.Validators
{
    public class RefreshTokenRequestDtoValidator : AbstractValidator<RefreshTokenRequestDto>
    {
        public RefreshTokenRequestDtoValidator()
        {
            RuleFor(x => x.RefreshToken)
                .NotEmpty()
                .WithMessage(
                    "Refresh token is required");
        }
    }
}
