using Microsoft.AspNetCore.Cors.Infrastructure;
using System.Text.Json;
using Test.UOL.Web.Entities;
using Test.UOL.Web.Interfaces;

namespace Test.UOL.Web.Services;

public class CartCouponService : ICartCouponService
{
    private readonly string _jsonPath;
    private readonly List<Coupon> _coupons;
    private readonly ICartService _cartService;
    private readonly ICartTotalCalculator _cartTotalCalculator;

    public CartCouponService(ICartTotalCalculator cartTotalCalculator, ICartService cartService)
    {
        _jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "files", "cupom.json");
        _coupons = LoadCoupons();
        _cartTotalCalculator = cartTotalCalculator;
        _cartService = cartService;
    }

    private class CouponWrapper
    {
        public List<Coupon> Coupons { get; set; } = new();
    }

    //Loads coupons from the JSON file
    private List<Coupon> LoadCoupons()
    {
        if (!File.Exists(_jsonPath))
            throw new FileNotFoundException("Coupon files not found.", _jsonPath);

        var jsonString = File.ReadAllText(_jsonPath);

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var wrapper = JsonSerializer.Deserialize<CouponWrapper>(jsonString, options);
        return wrapper?.Coupons ?? new List<Coupon>();
    }

    //Add coupon to cart and recalculates the total amount if coupon is valid
    public Cart AddCouponToCart(Guid cartId, string couponKey)
    {
        var cart = _cartService.GetCartById(cartId);

        if (cart.Coupon != null)
            throw new ArgumentException("A coupon was already applied to this cart");

        var coupon = _coupons?.FirstOrDefault(c => c.Key.Equals(couponKey, StringComparison.OrdinalIgnoreCase));

        ValidateCoupon(coupon);

        cart.Coupon = coupon;
        cart.TotalAmount = _cartTotalCalculator.CalculateTotal(cart);
        return cart;
    }

    //Checks if the coupon could be applied on cart
    private void ValidateCoupon(Coupon coupon)
    {
        if (coupon == null)
            throw new ArgumentException("Coupon not found");

        if (!coupon.Type.Equals("Fixed") && !coupon.Type.Equals("Percentage"))
            throw new ArgumentException("Coupon type not supported");

        if (!decimal.TryParse(coupon.Value, out decimal value) || value <= 0)
            throw new ArgumentException("Coupon value must be greater than zero");
    }
}