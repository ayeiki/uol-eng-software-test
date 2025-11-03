namespace Test.UOL.Web.Dtos;

public class CouponDiscountInCartRequest
{
    public Guid CartId {  get; set; }
    public string CouponDiscountKey { get; set; }
}
