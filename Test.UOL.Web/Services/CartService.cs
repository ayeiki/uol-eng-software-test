using Test.UOL.Web.Interfaces;
using Test.UOL.Web.Entities;

namespace Test.UOL.Web.Services;

public class CartService : ICartService
{
    private readonly ICartStore _cartStore;
    private readonly ICouponStore _couponStore;
    private readonly ICartTotalCalculator _cartTotalCalculator;

    public CartService(ICartStore cartStore, ICartTotalCalculator cartTotalCalculator, ICouponStore couponStore)
    {
        _cartStore = cartStore;
        _cartTotalCalculator = cartTotalCalculator;
        _couponStore = couponStore;
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

    public Cart ApplyCoupon(Guid cartId, string couponKey)
    {
        var cart = _cartStore.GetCartById(cartId);
        if (cart == null)
        {
            throw new ArgumentException("Cart not found", nameof(cartId));
        }
        if (cart.Coupon != null)
        {
            throw new ArgumentException("Coupon already applied to cart", nameof(couponKey));
        }

        var coupon = _couponStore.GetCouponByKey(couponKey);
        if(coupon == null)
        {
            throw new ArgumentException("Coupon not found", nameof(couponKey));
        }        
        cart.ApplyCoupon(coupon);

        cart.TotalAmount = _cartTotalCalculator.CalculateTotal(cart);
        return cart;
    }
}
