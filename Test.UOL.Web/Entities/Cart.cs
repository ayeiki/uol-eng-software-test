namespace Test.UOL.Web.Entities;

public class Cart
{
    public Guid Id { get; set; }
    public List<CartItem> CartItems { get; set; }
    public decimal TotalAmount { get; set; }
    public string? Customer { get; set; }
    public string? CustomerAddress { get; set; }
    public Coupon? Coupon { get; set; }
    public Cart()
    {
        CartItems = new List<CartItem>();
    }
}