using Test.UOL.Web.Entities;

namespace Test.UOL.Web.Interfaces;

public interface ICartCouponService
{
    /// <summary>
    /// Add a coupon to a cart and calculates the new TotalAmount.
    /// </summary>
    /// <param name="cartId">The Cart Id wich coupon will be applied</param>
    /// <param name="couponKey">The discount coupon to be applied on the cart.</param>
    /// <returns>
    /// The <seealso cref="Cart"/> with updated cost.
    /// </returns>
    Cart AddCouponToCart(Guid cartId, string couponKey);
}