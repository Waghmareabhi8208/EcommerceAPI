using Ecommerce.API.DTOs.Order;
using FluentValidation;
using System.Xml;

namespace Ecommerce.API.Validators
{
    public class UpdateOrderStatusDtoValidator : AbstractValidator<UpdateOrderStatusDto>
    {
        public UpdateOrderStatusDtoValidator()
        {
            RuleFor(x => x.Status)
                .NotEmpty()
                .WithMessage(
                    "Order status is required")

                .Must(status =>
                    new[]
                    {
                        "Pending",
                        "Processing",
                        "Shipped",
                        "Delivered",
                        "Cancelled"
                    }
                    .Contains(status))
                .WithMessage(
                    "Invalid order status");
        }
    }
}
