using Test.UOL.Web.Interfaces;
using Test.UOL.Web.Entities;

namespace Test.UOL.Web.Services;

public class CartTotalCalculator() : ICartTotalCalculator
{
    public decimal CalculateTotal(Cart cart)
    {
        var total = cart.CartItems.Sum(item => item.Product.Price * item.Quantity);

        // Apply coupon discount if exists
        // Implemented on CalculateTotal method to ensure correct total calculation on cart updates
        if (cart.Coupon != null)
            total = ApplyCouponDiscount(total, cart.Coupon);

        return total;
    }

    //Applies the coupon discount based on its type
    private decimal ApplyCouponDiscount(decimal total, Coupon coupon)
    {
        var discount = 0m;

        if (coupon.Type.Equals("Percentage", StringComparison.OrdinalIgnoreCase))
        {
            discount = decimal.Parse(coupon.Value) / 100;
            total = Math.Max(0, total * (1 - discount));
            return total;
        }

        discount = decimal.Parse(coupon.Value);
        total = Math.Max(0, total - discount);
        return total;
    }
}