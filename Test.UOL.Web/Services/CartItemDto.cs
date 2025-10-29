namespace Test.UOL.Web.Services;

public class CartItemDto
{

    public CartItemDto(ProductDto product, int quantity)
    {
        Product = product;
        Quantity = quantity;
    }

    public ProductDto Product { get; set; }
    public int Quantity { get; set; }
}
