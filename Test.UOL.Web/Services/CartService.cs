using Test.UOL.Web.Dtos;
using Test.UOL.Web.Entities;
using Test.UOL.Web.Interfaces;

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
        Cart? cart = CheckExistingCart(id);
        return cart;
    }

    public Cart UpdateCart(Cart cart)
    {
        var existingCart = CheckExistingCart(cart.Id);
        existingCart.Customer = cart.Customer;
        existingCart.CustomerAddress = cart.CustomerAddress;

        cart.TotalAmount = _cartTotalCalculator.CalculateTotal(cart);
        return cart;
    }

    #region Coupon Discount
    public Cart ApplyCouponDiscount(CouponDiscountInCartRequest request)
    {
        var cart = ValidateCart(request.CartId);
        if (cart.TotalAmount <= 0)
            return cart;

        var result = ValidateCouponDiscount(request, cart);

        cart.CouponDiscountApplied = result.coupon;
        cart.DiscountAmountInCart = result.discount;
        cart.TotalAmount = _cartTotalCalculator.CalculateTotal(cart);

        return cart;
    }

    public void RemoveCouponDiscount(Guid cartId)
    {
        var cart = ValidateCart(cartId);
        cart.CouponDiscountApplied = null;
        cart.DiscountAmountInCart = null;
        cart.TotalAmount = _cartTotalCalculator.CalculateTotal(cart);
    }

    private (CouponDiscount coupon, decimal discount) ValidateCouponDiscount(CouponDiscountInCartRequest request, Cart cart)
    {
        if (cart.CouponDiscountApplied is not null)
            throw new ArgumentException("Já existe um cupom aplicado.");

        var coupon = _cartStore.GetCouponDiscountByKey(request.CouponDiscountKey)
            ?? throw new ArgumentException("Cupom inválido.");

        var discount = coupon.Type switch
        {
            Entities.Enums.CouponDiscountType.Fixed => coupon.Value,
            Entities.Enums.CouponDiscountType.Percentage => cart.TotalAmount * (coupon.Value / 100m),
            _ => 0
        };

        return(coupon, discount);
    }

    private Cart ValidateCart(Guid cartId)
    {
        var cart = CheckExistingCart(cartId);
        if (cart.CartItems is null || cart.CartItems.Count == 0)
            throw new ArgumentException("O seu carrinho está vazio.");

        return cart;
    }

    private Cart CheckExistingCart(Guid id)
    {
        var cart = _cartStore.GetCartById(id);
        if (cart == null)
        {
            throw new ArgumentException("Cart not found", nameof(id));
        }

        return cart;
    }
    #endregion
}

