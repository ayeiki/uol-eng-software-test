using Test.UOL.Web.Entities;

namespace Test.UOL.Web.Interfaces;

public interface ICartTotalCalculator
{
    /// <summary>
    /// Calculates the total price of the specified <seealso cref="Cart"/>.
    /// </summary>
    /// <param name="cart">The cart to calculate the total price for.</param>
    /// <returns>
    /// The total price of the <seealso cref="Cart"/>.
    /// </returns>
    decimal CalculateTotal(Cart cart);
}
