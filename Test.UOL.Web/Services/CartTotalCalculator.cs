using Test.UOL.Web.Interfaces;
using Test.UOL.Web.Entities;

namespace Test.UOL.Web.Services;

public class CartTotalCalculator() : ICartTotalCalculator
{
    public decimal CalculateTotal(Cart cart)
    {
        var total = cart.CartItems.Sum(item => item.Product.Price * item.Quantity);

        if (cart.Coupon?.Key != null)
        {
            var discount = 0m;

            if (cart.Coupon.Type.Equals("Percentage", StringComparison.OrdinalIgnoreCase))
            {
                discount = decimal.Parse(cart.Coupon.Value) / 100;
                total = Math.Max(0, total * (1 - discount));
                return total;
            }

            discount = decimal.Parse(cart.Coupon.Value);
            total = Math.Max(0, total - discount);
            return total;
        }

        return total;
    }
}