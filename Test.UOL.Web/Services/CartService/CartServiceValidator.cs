using FluentValidation;

namespace Test.UOL.Web.Services.CartService
{
    public class CartServiceValidator : AbstractValidator<CartItemDto>
    {
        public CartServiceValidator()
        {
            RuleFor(x => x.Quantity)
                .GreaterThan(0)
                .WithMessage("Quantity must be greater than zero.");
        }
    }
}