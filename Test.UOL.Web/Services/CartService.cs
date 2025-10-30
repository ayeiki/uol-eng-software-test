using Test.UOL.Web.Interfaces;
using Test.UOL.Web.Entities;

namespace Test.UOL.Web.Services;

public class CartService : ICartService
{
    private readonly ICartStore _cartStore;
    private readonly ICartTotalCalculator _cartTotalCalculator;

    public CartService(ICartStore cartStore, ICartTotalCalculator cartTotalCalculator)
    {
        _cartStore = cartStore;
        _cartTotalCalculator = cartTotalCalculator;
    }

    public Cart CreateCart()
    {
        var cart = new Cart
        {
            Id = Guid.NewGuid(),
        };
        cart.TotalAmount = _cartTotalCalculator.CalculateTotal(cart);
        return _cartStore.AddCart(cart);
    }

    public Cart GetCartById(Guid id)
    {
        var cart = _cartStore.GetCartById(id);
        if (cart == null)
        {
            throw new ArgumentException("Cart not found", nameof(id));
        }
        return cart;
    }

    public Cart UpdateCart(Cart cart)
    {
        var existingCart = _cartStore.GetCartById(cart.Id);
        if (existingCart == null)
        {
            throw new ArgumentException("Cart not found", nameof(cart.Id));
        }

        existingCart.Customer = cart.Customer;
        existingCart.CustomerAddress = cart.CustomerAddress;

        cart.TotalAmount = _cartTotalCalculator.CalculateTotal(cart);
        return cart;
    }
}

