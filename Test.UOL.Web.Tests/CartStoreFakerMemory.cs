using System;
using System.Collections.Generic;
using System.Linq;
using Test.UOL.Web.Entities;
using Test.UOL.Web.Interfaces;

namespace Test.UOL.Web.Tests;

public class CartStoreFakerMemory : ICartStore
{
    private readonly List<Cart> _carts = new();
    private readonly List<CouponDiscount> _coupons = new();

    public Cart AddCart(Cart cart)
    {
        _carts.Add(cart);
        return cart; // ✅ Isso precisa existir!
    }

    public Cart GetCartById(Guid id) => _carts.FirstOrDefault(c => c.Id == id);

    public CouponDiscount GetCouponDiscountByKey(string key) =>
        _coupons.FirstOrDefault(c => string.Equals(c.Key, key, StringComparison.OrdinalIgnoreCase));

    public void AddCoupon(CouponDiscount coupon) => _coupons.Add(coupon);

    #region Not Implemented
    public IEnumerable<CouponDiscount> GetCouponDiscounts()
    {
        throw new NotImplementedException();
    }

    public bool DeleteCart(Guid id)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Cart> GetCarts()
    {
        throw new NotImplementedException();
    }
    #endregion
}
