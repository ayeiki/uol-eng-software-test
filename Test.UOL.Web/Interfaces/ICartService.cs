using Test.UOL.Web.Entities;
using Test.UOL.Web.Services;

namespace Test.UOL.Web.Interfaces;

public interface ICartService
{
    Guid NewCart();
    decimal CalculateTotal(Guid IdCart);
    void AddItem(Guid IdCart, CartItemDto cartItem);
    void ChangeItem(Guid IdCart, Guid IdCartItem, int quantity);
    void DeleteItem(Guid IdCart, Guid IdCartItem, int quantity);
    IEnumerable<CartItem> GetCartItems(Guid IdCart);
}
