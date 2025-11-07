using Test.UOL.Web.Entities;

namespace Test.UOL.Web.Interfaces
{
    public interface ICouponStore
    {
        /// <summary>
        /// Gets all coupons.
        /// </summary>
        /// <returns>
        /// A collection of all <seealso cref="Coupon"/>s.
        /// </returns>
        IEnumerable<Coupon> GetCoupons();
        /// <summary>
        /// Gets a <seealso cref="Coupon"/> by its identifier.
        /// </summary>
        /// <param name="key">The identifier of the coupon.</param>
        /// <returns>
        /// The <seealso cref="Coupon"/> if found; otherwise, null.
        /// </returns>
        Coupon? GetCouponByKey(string key);
    }
}
