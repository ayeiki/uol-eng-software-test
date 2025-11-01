using Test.UOL.Web.Entities;
using Test.UOL.Web.Helpers;
using Test.UOL.Web.Interfaces;

namespace Test.UOL.Web.Services;

public sealed class CartTotalWithCupomCalculator : ICartTotalCalculator
{
    private readonly ICartTotalCalculator _inner;
    private readonly ICupomProvider _provider;
    private readonly IDiscountCalculator _discountCalculator;

    public CartTotalWithCupomCalculator(
        ICartTotalCalculator inner,
        ICupomProvider provider,
        IDiscountCalculator discountCalculator)
    {
        _inner = inner;
        _provider = provider;
        _discountCalculator = discountCalculator;
    }

    public decimal CalculateTotal(Cart cart)
    {
        var baseTotal = _inner.CalculateTotal(cart);

        if (string.IsNullOrWhiteSpace(cart.CupomCode)) // sem cupom aplicado, retorna o total base
            return baseTotal;

        var cupom = _provider.GetCupom(cart.CupomCode);
        if (cupom == null)
        {
            cart.ClearCupom();
            return baseTotal;
        }

        try
        {
            var (type, value) = NormalizeAndValidateHelper.NormalizeAndValidate(cupom);
            var discount = _discountCalculator.ComputeDiscount(baseTotal, type, value);
            return Math.Max(0, baseTotal - discount);  // verifica se o desconto não excede o total do carrinho, se sim retorna zero
        }
        catch
        {
            cart.ClearCupom(); // se por algum motivo deu erro, ignora e limpa
            return baseTotal;
        }
    }
}