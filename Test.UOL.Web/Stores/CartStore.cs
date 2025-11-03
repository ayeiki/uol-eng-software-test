using System.Text.Json;
using Test.UOL.Web.Entities;
using Test.UOL.Web.Entities.Helpers;
using Test.UOL.Web.Interfaces;

namespace Test.UOL.Web.Stores;

public class CartStore : ICartStore
{
    public static readonly Dictionary<Guid, Cart> Carts = new Dictionary<Guid, Cart>();
    private static IEnumerable<CouponDiscount> CouponDiscounts;
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _env;

    public CartStore(IConfiguration configuration, IWebHostEnvironment env)
    {
        _configuration = configuration;
        _env = env;
        SetCouponDiscounts();
    }

    private void SetCouponDiscounts()
    {
        string relativePath = _configuration["CuponsFilePath"];
        string fullPath = Path.Combine(_env.ContentRootPath, relativePath);

        string json = File.ReadAllText(fullPath);

        // Extrai o array "coupons" de dentro do JSON
        using var doc = JsonDocument.Parse(json);
        var couponsJson = doc.RootElement.GetProperty("coupons").GetRawText();

        // Desserializa somente o array de cupons
        var data = JsonSerializer.Deserialize<List<CouponDiscount>>(couponsJson);

        CouponDiscounts = data;
    }

    public IEnumerable<CouponDiscount> GetCouponDiscounts()
    {
        return CouponDiscounts;
    }

    public CouponDiscount? GetCouponDiscountByKey(string key)
    {
        return CouponDiscounts.FirstOrDefault(c=> c.Key.Equals(key));
    }

    public IEnumerable<Cart> GetCarts()
    {
        return [.. Carts.Values];
    }

    public Cart AddCart(Cart cart)
    {
        Carts.Add(cart.Id, cart);
        return cart;
    }
    public Cart? GetCartById(Guid id)
    {
        return Carts.TryGetValue(id, out var cart) ? cart : null;
    }
    public bool DeleteCart(Guid id)
    {
        return Carts.Remove(id);
    }
}

