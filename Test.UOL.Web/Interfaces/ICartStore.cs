using Test.UOL.Web.Entities;

namespace Test.UOL.Web.Interfaces
{
    public interface ICartStore
    {
        void AddItem(CartItem cartItem);
        void ChangeItem(Guid Id, int quantity);
        void DeleteItem(Guid Id, int quantity);
        IEnumerable<CartItem> GetCartItems();
    }
}