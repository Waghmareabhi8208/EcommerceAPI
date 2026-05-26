using Ecommerce.API.DTOs.Payment;
using FluentValidation;

namespace Ecommerce.API.Validators
{
    public class CreatePaymentDtoValidator : AbstractValidator<CreatePaymentDto>
    {
        public CreatePaymentDtoValidator()
        {
            RuleFor(x => x.OrderId)
                .GreaterThan(0)
                .WithMessage("Valid order id is required");
        }
    }
}
