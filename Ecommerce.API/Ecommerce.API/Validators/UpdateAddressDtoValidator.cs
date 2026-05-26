using Ecommerce.API.DTOs.Address;
using FluentValidation;

namespace Ecommerce.API.Validators
{
    public class UpdateAddressDtoValidator : AbstractValidator<UpdateAddressDto>
    {
        public UpdateAddressDtoValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty()
                .WithMessage("Full name is required")
                .MinimumLength(3);

            RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .WithMessage("Phone number is required")

                .Matches(@"[0-9]{10}$")
                .WithMessage("Phone number must be 10 digits");

            RuleFor(x => x.City)
                .NotEmpty()
                .WithMessage("City is required");

            RuleFor(x => x.State)
              .NotEmpty()
              .WithMessage("State is required");

            RuleFor(x => x.PostalCode)
                .NotEmpty()
                .WithMessage("Postal code is required")

                .Matches(@"^[0-9]{6}$")
                .WithMessage(
                    "Postal code must be 6 digits");

            RuleFor(x => x.Country)
                .NotEmpty()
                .WithMessage("Country is required");
        }
    }
}
