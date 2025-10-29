namespace Test.UOL.Web.Services;

public class ProductDto
{
    public ProductDto(int productId, string name, decimal price)
    {
        ProductId = productId;
        Name = name;
        Price = price;
    }
    public int ProductId { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
}