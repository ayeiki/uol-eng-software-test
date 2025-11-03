using Test.UOL.Web.Interfaces;
using Test.UOL.Web.Entities;

namespace Test.UOL.Web.Services;

public class CartTotalCalculator() : ICartTotalCalculator
{
    public decimal CalculateTotal(Cart cart)
    {
        var discountAmountInCart = cart.DiscountAmountInCart ?? 0;
        var total = (cart.CartItems.Sum(item => item.Product.Price * item.Quantity)) - discountAmountInCart;
        return total;
    }
}