using Test.UOL.Web.Entities;

namespace Test.UOL.Web.Interfaces;

public interface ICartService
{
    /// <summary>
    /// Creates a new <seealso cref="Cart"/>
    /// </summary>
    /// <returns>
    /// The created <seealso cref="Cart"/>.
    /// </returns>
    Cart CreateCart();
    /// <summary>
    /// Gets a <seealso cref="Cart"/> by its identifier.
    /// </summary>
    /// <param name="id">The identifier of the <seealso cref="Cart"/>.</param>
    /// <returns>
    /// The <seealso cref="Cart"/> if found; otherwise, null.
    /// </returns>
    Cart? GetCartById(Guid id);
}
