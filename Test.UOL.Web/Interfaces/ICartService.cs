using Test.UOL.Web.Entities;
using Test.UOL.Web.Services;

namespace Test.UOL.Web.Interfaces;

public interface ICartService
{
    decimal CalculateTotal(Guid Id);
    void AddItem(CartItemDto cartItem);
    void ChangeItem(Guid Id, int quantity);
    void DeleteItem(Guid Id, int quantity);
    IEnumerable<CartItem> GetCartItems();
}
