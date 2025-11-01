using Test.UOL.Web.Helpers;

namespace Test.UOL.Web.Entities;

public class Cart
{
    public Guid Id { get; set; }
    public List<CartItem> CartItems { get; set; }
    public decimal TotalAmount { get; set; }
    public string? Customer { get; set; }
    public string? CustomerAddress { get; set; }

    public string? CupomCode { get; set; }
    public Cart()
    {
        CartItems = new List<CartItem>();
    }

    /// <summary> 
    /// Aplica o código do cupom ao carrinho. 
    /// Lança exceções com mensagens específicas se já houver um cupom aplicado ou se o código for inválido. 
    /// </summary>
    public void ApplyCouponCode(string code)
    {
        if (!string.IsNullOrWhiteSpace(CupomCode))
            throw new CupomException(ErrorMessages.CupomAlreadyApplied);

        if (string.IsNullOrWhiteSpace(code))
            throw new CupomException(ErrorMessages.CupomInvalid);

        CupomCode = code.Trim();
    }

    /// <summary> 
    /// Remove o cupom do carrinho. 
    /// </summary>
    public void ClearCupom() => CupomCode = null;
}

