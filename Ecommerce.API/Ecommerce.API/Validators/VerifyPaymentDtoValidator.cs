using Ecommerce.API.DTOs.Payment;
using FluentValidation;

namespace Ecommerce.API.Validators
{
    public class VerifyPaymentDtoValidator : AbstractValidator<VerifyPaymentDto>
    {
        public VerifyPaymentDtoValidator()
        {
            RuleFor(x => x.OrderId)
                .GreaterThan(0)
                .WithMessage("Valid order id is required");

            RuleFor(x => x.RazorpayPaymentId)
                .NotEmpty()
                .WithMessage("Payment id is required");

            RuleFor(x => x.RazorpayOrderId)
                .NotEmpty()
                .WithMessage("Razorpay order id is required");

            RuleFor(x => x.RazorpaySignature)
                .NotEmpty()
                .WithMessage("Payment signature is required");
        }
    }
}
