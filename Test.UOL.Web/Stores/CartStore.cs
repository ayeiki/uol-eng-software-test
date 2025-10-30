using Test.UOL.Web.Entities;
using Test.UOL.Web.Interfaces;

namespace Test.UOL.Web.Stores;

public class CartStore : ICartStore
{
    public CartStore()
    {

    }

    public IEnumerable<CartItem> GetCartItems(Guid idCart)
    {
        var cart = Store.Carts.SingleOrDefault(_ => _.Id == idCart);
        if (cart != null)
            return cart.CartItems;
        return Enumerable.Empty<CartItem>();
    }

    public Cart NewCart()
    {
        var cart = new Cart();
        cart.Id = Guid.NewGuid();
        cart.CartItems = new List<CartItem>();
        Store.Carts.Add(cart);
        return cart;
    }

    public void AddItem(Guid idCart, CartItem cartItem)
    {
        var cart = Store.Carts.SingleOrDefault(_ => _.Id == idCart);
        if (cart != null)
        {
            cart.CartItems.Add(cartItem);
        }
    }

    public void ChangeItem(Guid idCart, Guid idCartItem, int quantity)
    {
        var cart = Store.Carts.SingleOrDefault(_ => _.Id == idCart);
        if (cart != null)
        {
            var item = cart.CartItems.SingleOrDefault(_ => _.Id == idCartItem);
            if (item != null)
            {
                item.ChangeQuantity(quantity);
            }
        }
    }

    public void DeleteItem(Guid idCart, Guid idCartItem)
    {
        var cart = Store.Carts.SingleOrDefault(_ => _.Id == idCart);
        if (cart != null)
        {
            var item = cart.CartItems.SingleOrDefault(_ => _.Id == idCartItem);
            if (item != null)
            {
                cart.CartItems.Remove(item);
            }
        }
    }
}

