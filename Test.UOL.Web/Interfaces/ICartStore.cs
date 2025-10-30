using Test.UOL.Web.Entities;

namespace Test.UOL.Web.Interfaces
{
    public interface ICartStore
    {
        Cart NewCart();
        IEnumerable<CartItem> GetCartItems(Guid IdCart);
        void AddItem(Guid idCart, CartItem cartItem);
        void ChangeItem(Guid IdCart, Guid IdCartItem, int quantity);
        void DeleteItem(Guid IdCart, Guid IdCartItem);

    }
}