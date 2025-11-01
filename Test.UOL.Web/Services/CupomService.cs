using Test.UOL.Web.Entities;
using Test.UOL.Web.Helpers;
using Test.UOL.Web.Interfaces;

namespace Test.UOL.Web.Services;

public sealed class CupomService : ICupomService
{
    private readonly ICartStore _cartStore;
    private readonly ICupomProvider _provider;
    private readonly ICartTotalCalculator _calculator;

    public CupomService(ICartStore cartStore, ICupomProvider provider, ICartTotalCalculator calculator)
    {
        _cartStore = cartStore;
        _provider = provider;
        _calculator = calculator;
    }


    public void ApplyCupomToCart(Guid cartId, string cupomCode)
    {
        #region Validações
        if (string.IsNullOrWhiteSpace(cupomCode))
            throw new CupomException(ErrorMessages.CupomInvalid);

        var cart = _cartStore.GetCartById(cartId) ?? throw new ArgumentException("Carrinho não encontrado", nameof(cartId));
        if (!string.IsNullOrWhiteSpace(cart.CupomCode))
            throw new CupomException(ErrorMessages.CupomAlreadyApplied);

        var cupom = _provider.GetCupom(cupomCode.Trim()) ?? throw new CupomException(ErrorMessages.CupomInvalid);
        #endregion

        var (type, value) = NormalizeAndValidateHelper.NormalizeAndValidate(cupom);
        cart.ApplyCouponCode(cupom.key!);
        cart.TotalAmount = _calculator.CalculateTotal(cart); // recalcula já com desconto
    }
    

    public void RemoveCupomFromCart(Guid cartId)
    {
        var cart = _cartStore.GetCartById(cartId) ?? throw new ArgumentException("Carrinho não encontrado");
        cart.ClearCupom();
        cart.TotalAmount = _calculator.CalculateTotal(cart);
    }
   
}
