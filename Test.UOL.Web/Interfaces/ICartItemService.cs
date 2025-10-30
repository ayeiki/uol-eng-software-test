using Test.UOL.Web.Entities;

namespace Test.UOL.Web.Interfaces;

public interface ICartItemService
{
    /// <summary>
    ///  Adds an item to the specified cart.
    /// </summary>
    /// <param name="IdCart">The identifier of the cart.</param>
    /// <param name="cartItem">The item to add to the cart.</param>
    /// <returns>The updated cart.</returns>
    Cart PutItemInCart(Guid IdCart, CartItem cartItem);
    /// <summary>
    /// Deletes an item from the specified cart.
    /// </summary>
    /// <param name="IdCart">The identifier of the cart.</param>
    /// <param name="IdCartItem">The identifier of the item to delete from the cart.</param>
    /// <returns>The updated cart.</returns>
    Cart DeleteItem(Guid IdCart, Guid IdCartItem);
    /// <summary>
    /// Gets all items from the specified cart.
    /// </summary>
    /// <param name="IdCart">The identifier of the cart.</param>
    /// <returns>A collection of all <seealso cref="CartItem"/>s in the cart.</returns>
    IEnumerable<CartItem> GetCartItems(Guid IdCart);
}