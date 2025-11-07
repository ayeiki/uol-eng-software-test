using Test.UOL.Web.Entities;

namespace Test.UOL.Web.Interfaces;

public interface ICartService
{
    /// <summary>
    /// Creates a new <seealso cref="Cart"/>
    /// </summary>
    /// <returns>
    /// The created <seealso cref="Cart"/>.
    /// </returns>
    Cart CreateCart();
    /// <summary>
    /// Gets a <seealso cref="Cart"/> by its identifier.
    /// </summary>
    /// <param name="id">The identifier of the <seealso cref="Cart"/>.</param>
    /// <returns>
    /// The <seealso cref="Cart"/> if found; otherwise, null.
    /// </returns>
    Cart? GetCartById(Guid id);
    /// <summary>
    /// Updates the specified <seealso cref="Cart"/>.
    /// Does not update cart items.
    /// </summary>
    /// <param name="cart">The cart to update.</param>
    /// <returns>
    /// The updated <seealso cref="Cart"/>.
    /// </returns>
    Cart UpdateCart(Cart cart);

    /// <summary>
    /// Updates the specified <seealso cref="Cart"/> total value with discount coupon.
    /// Does not update cart items.
    /// </summary>
    /// <param name="cartId">The cart ID to update.</param>
    /// <param name="coupon">The coupon to apply to the cart.</param>
    /// <returns>
    /// The updated <seealso cref="Cart"/>.
    /// </returns>
    Cart ApplyCoupon(Guid cartId, string couponKey);

}
