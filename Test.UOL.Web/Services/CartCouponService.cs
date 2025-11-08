using System.Text.Json;
using System.Text.Json.Serialization;
using Test.UOL.Web.Entities;
using Test.UOL.Web.Interfaces;

namespace Test.UOL.Web.Services
{
    public class CartCouponService : ICartCouponService
    {
        private readonly string _jsonPath;
        private readonly Dictionary<string, (string Value, CouponType Type)> _coupons;
        private readonly ICartService _cartService;
        private readonly ICartTotalCalculator _cartTotalCalculator;

        public CartCouponService(ICartTotalCalculator cartTotalCalculator, ICartService cartService)
        {
            _jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "files", "cupom.json");
            _cartTotalCalculator = cartTotalCalculator;
            _cartService = cartService;

            _coupons = LoadCoupons();
        }

        //Load cupons from JSON file
        private Dictionary<string, (string Value, CouponType Type)> LoadCoupons()
        {
            if (!File.Exists(_jsonPath))
                throw new FileNotFoundException("Coupon file not found.", _jsonPath);

            var jsonString = File.ReadAllText(_jsonPath);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
            };

            using var jsonCoupons = JsonDocument.Parse(jsonString);
            var root = jsonCoupons.RootElement;

            if (root.TryGetProperty("coupons", out var couponsElement))
            {
                var coupons = new Dictionary<string, (string Value, CouponType Type)>(StringComparer.OrdinalIgnoreCase);

                foreach (var element in couponsElement.EnumerateArray())
                {
                    var key = element.GetProperty("key").GetString();
                    var value = element.GetProperty("value").GetString();

                    if (Enum.TryParse<CouponType>(
                            element.GetProperty("type").GetString(),
                            ignoreCase: true,
                            out var type))
                    {
                        if (!string.IsNullOrWhiteSpace(key))
                            coupons[key] = (value ?? "0", type);
                    }
                }

                return coupons;
            }

            return new Dictionary<string, (string Value, CouponType Type)>();
        }

        //Applies coupon to cart and recalculates total
        public Cart AddCouponToCart(Guid cartId, string couponKey)
        {
            var cart = _cartService.GetCartById(cartId);

            if (cart.Coupon != null)
                throw new ArgumentException("A coupon was already applied to this cart.");

            if (!_coupons.TryGetValue(couponKey, out var couponTuple))
                throw new ArgumentException("Coupon not found.");

            var coupon = new Coupon
            {
                Key = couponKey,
                Value = couponTuple.Value,
                Type = couponTuple.Type
            };

            ValidateCoupon(coupon);

            cart.Coupon = coupon;
            cart.TotalAmount = _cartTotalCalculator.CalculateTotal(cart);
            return cart;
        }

    //Checks if the coupon could be applied on cart
        private void ValidateCoupon(Coupon coupon)
        {
            if (coupon.Type != CouponType.Percentage && coupon.Type != CouponType.Fixed)
                throw new ArgumentException("Coupon type not supported.");

            if (!decimal.TryParse(coupon.Value, out decimal value) || value <= 0)
                throw new ArgumentException("Coupon value must be greater than zero.");
        }
    }
}