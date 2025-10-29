using Test.UOL.Web.Entities;
using Test.UOL.Web.Interfaces;

namespace Test.UOL.Web.Stores;

public class CartStore : ICartStore
{
    public CartStore()
    {

    }

    public IEnumerable<CartItem> GetCartItems()
    {
        return Store.CartItems;
    }

    public void AddItem(CartItem cartItem)
    {
        Store.CartItems.Add(cartItem);
    }

    public void ChangeItem(Guid Id, int quantity)
    {
        var item = Store.CartItems.SingleOrDefault(_ => _.Id == Id);
        if (item != null)
        {
            item.ChangeQuantity(quantity);
        }
    }

    public void DeleteItem(Guid Id, int quantity)
    {
        var item = Store.CartItems.SingleOrDefault(_ => _.Id == Id);
        if (item != null)
        {
            Store.CartItems.Remove(item);
        }
    }
}

