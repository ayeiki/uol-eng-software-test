namespace Test.UOL.Web.Entities;

public class Cart
{
    public Guid Id { get; set; }
    public List<CartItem> CartItems { get; set; }
    public decimal TotalAmount { get; set; }
    public string? Customer { get; set; }
    public string? CustomerAddress { get; set; }
    
   
    // Aplicação de cupons de desconto
   
    public string? CouponKey { get; private set; }
    public Coupon? Coupon { get; private set; }


    public Cart()
    {
        CartItems = new List<CartItem>();
    }

    public void ApplyCoupon(Coupon coupon)
    {
        Coupon = coupon;
        CouponKey = coupon.Key;
    }    
}

