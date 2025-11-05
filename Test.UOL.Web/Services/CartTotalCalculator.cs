using Test.UOL.Web.Interfaces;
using Test.UOL.Web.Entities;

namespace Test.UOL.Web.Services;

public class CartTotalCalculator() : ICartTotalCalculator
{
    public decimal CalculateTotal(Cart cart)
    {
        var subtotal = cart.CartItems.Sum(item => item.Product.Price * item.Quantity);

        decimal discount = 0;
        if (cart.CouponDiscountApplied is not null)
        {
            var coupon = cart.CouponDiscountApplied;
            discount = coupon.Type switch
            {
                Entities.Enums.CouponDiscountType.Fixed => coupon.Value,
                Entities.Enums.CouponDiscountType.Percentage => subtotal * (coupon.Value / 100m),
                _ => 0
            };
        }

        var total = subtotal - discount;
        cart.DiscountAmountInCart = Math.Round(discount, 2);

        return Math.Round(Math.Max(total, 0), 2);
    }
}