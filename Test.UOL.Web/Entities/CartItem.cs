namespace Test.UOL.Web.Entities;

public class CartItem
{
    public Guid Id { get; set; }
    public Product Product { get; }
    public int Quantity { get; private set; }

    public CartItem(Product product, int quantity)
    {
        Id = Guid.NewGuid();
        Product = product;
        Quantity = quantity;
    }

    public void ChangeQuantity(int quantity) => Quantity += quantity;
}
