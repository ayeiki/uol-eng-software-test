using Test.UOL.Web.Entities;

namespace Test.UOL.Web.Interfaces;

public interface ICartStore
{
    /// <summary>
    /// Gets all carts.
    /// </summary>
    /// <returns>
    /// A collection of all <seealso cref="Cart"/>s.
    /// </returns>
    IEnumerable<Cart> GetCarts();
    /// <summary>
    /// Adds a new cart.
    /// </summary>
    /// <param name="cart">The cart to add.</param>
    /// <returns>
    /// The added <seealso cref="Cart"/>.
    /// </returns>
    Cart AddCart(Cart cart);
    /// <summary>
    /// Gets a <seealso cref="Cart"/> by its identifier.
    /// </summary>
    /// <param name="id">The identifier of the cart.</param>
    /// <returns>
    /// The <seealso cref="Cart"/> if found; otherwise, null.
    /// </returns>
    Cart? GetCartById(Guid id);
    /// <summary>
    /// Deletes a <seealso cref="Cart"/> by its identifier.
    /// </summary>
    /// <param name="id">The identifier of the cart to delete.</param>
    /// <returns>
    /// True if the cart was deleted; otherwise, false.
    /// </returns>
    bool DeleteCart(Guid id);
}
