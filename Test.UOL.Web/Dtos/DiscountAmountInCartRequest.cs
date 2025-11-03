namespace Test.UOL.Web.Dtos;

public class DiscountAmountInCartRequest
{
    public Guid CartId {  get; set; }
    public string CouponDiscountKey { get; set; }
}
