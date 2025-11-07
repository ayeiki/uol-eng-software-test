using Test.UOL.Web.Interfaces;
using Test.UOL.Web.Entities;

namespace Test.UOL.Web.Services;

public class CartTotalCalculator() : ICartTotalCalculator
{
    public decimal CalculateTotal(Cart cart)
    {
        var total = cart.CartItems.Sum(item => item.Product.Price * item.Quantity);
        total = CalculateTotalWithCoupon(total, cart.Coupon);
        return total;
    }

    private static decimal CalculateTotalWithCoupon(decimal total, Coupon? coupon)
    {
        if (coupon == null) return total;
        
        decimal totalAmount = total;
        decimal discount;

        if (coupon.Type == Enums.ECouponType.Percentage)
        {
            if (coupon.Value.HasValue)
            {
                discount = (total * coupon.Value.Value) / 100;
                totalAmount -= discount;
            }
        }
        else
        {
            if (coupon.Value.HasValue)
            {
                discount = coupon.Value.Value;
                totalAmount -= discount;
            }
        }
        return totalAmount < 0 ? 0 : totalAmount;        
    }
}