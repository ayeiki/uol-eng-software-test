using Test.UOL.Web.Entities;
using Test.UOL.Web.Interfaces;

namespace Test.UOL.Web.Stores;

public class CartStore : ICartStore
{

    public static readonly Dictionary<Guid, Cart> Carts = new Dictionary<Guid, Cart>();

    public CartStore()
    {

    }

    public IEnumerable<Cart> GetCarts()
    {
        return [.. Carts.Values];
    }

    public Cart AddCart(Cart cart)
    {
        Carts.Add(cart.Id, cart);
        return cart;
    }
    public Cart? GetCartById(Guid id)
    {
        return Carts.TryGetValue(id, out var cart) ? cart : null;
    }
    public bool DeleteCart(Guid id)
    {
        return Carts.Remove(id);
    }
}

