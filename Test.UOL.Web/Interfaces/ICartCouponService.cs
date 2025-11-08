using Test.UOL.Web.Entities;

namespace Test.UOL.Web.Interfaces;

public interface ICartCouponService
{
    /// <summary>
    /// Calculates the total price of the specified <seealso cref="Cart"/> after using a coupon.
    /// </summary>
    /// <param name="cart">The cart to calculate the total price for.</param>
    /// <param name="couponKey">The discount coupon to be applied on the cart.</param>
    /// <returns>
    /// The <seealso cref="Cart"/> with updated cost.
    /// </returns>
    Cart ApplyCouponDiscount(Cart cart, string couponKey);
}