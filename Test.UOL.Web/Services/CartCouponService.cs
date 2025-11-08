using System.Text.Json;
using Test.UOL.Web.Entities;
using Test.UOL.Web.Interfaces;
using Test.UOL.Web.Stores;

namespace Test.UOL.Web.Services;

public class CartCouponService : ICartCouponService
{
    private readonly string _jsonPath;
    private readonly List<Coupon> _coupons;
    private readonly ICartTotalCalculator _cartTotalCalculator;

    public CartCouponService(ICartTotalCalculator cartTotalCalculator)
    {
        _jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "files", "cupom.json");
        _coupons = LoadCoupons();
        _cartTotalCalculator = cartTotalCalculator;
    }

    private class CouponWrapper
    {
        public List<Coupon> Coupons { get; set; } = new();
    }

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

    public Cart ApplyCouponDiscount(Cart cart, string couponKey)
    {
        if (cart.Coupon.Key != null)
            throw new ArgumentException("A coupon was already applied on this cart");

        var coupon = _coupons?.FirstOrDefault(c => c.Key.Equals(couponKey, StringComparison.OrdinalIgnoreCase));

        ValidateCoupon(coupon);

        cart.Coupon = coupon;
        cart.TotalAmount = _cartTotalCalculator.CalculateTotal(cart);
        return cart;
    }

    private void ValidateCoupon(Coupon coupon)
    {
        if (coupon == null)
            throw new ArgumentException("Coupon not found");

        if (!coupon.Type.Equals("Fixed") && !coupon.Type.Equals("Percentage"))
            throw new ArgumentException("Coupon type not supported");

        if (!(decimal.Parse(coupon.Value ?? "0") > 0))
            throw new ArgumentException("Coupon value must be greater than zero");
    }
}