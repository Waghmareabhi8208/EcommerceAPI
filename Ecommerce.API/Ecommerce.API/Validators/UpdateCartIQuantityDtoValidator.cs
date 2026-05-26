using Ecommerce.API.DTOs.Cart;
using FluentValidation;

namespace Ecommerce.API.Validators
{
    public class UpdateCartIQuantityDtoValidator : AbstractValidator<UpdateCartQuantityDto>
    {
        public UpdateCartIQuantityDtoValidator()
        {
            RuleFor(x => x.Quantity)
               .GreaterThan(0)
               .WithMessage("Quantity must be greater than 0");
        }
    }
}
