
namespace Test.UOL.Web.Interfaces
{
    public interface ICupomService
    {
        /// <summary>
        /// Valida e aplica um cupom ao carrinho informado, recalculando o total.        
        /// </summary>
        /// <param name="cartId">Id do carrinho</param>
        /// <param name="cupomCode">Código do cupom</param>
        void ApplyCupomToCart(Guid cartId, string cupomCode);

        /// <summary>
        /// Remove o cupom do carrinho e recalcula o total.
        /// </summary>
        /// <param name="cartId">Id do carrinho</param>
        void RemoveCupomFromCart(Guid cartId);

    }
        
}
