namespace Test.UOL.Web.Entities;

public class Product
{
    public Guid Id { get; }
    public string Name { get; }
    public decimal Price { get; }

    public Product(Guid id, string name, decimal price)
    {
        Id = id;
        Name = name;
        Price = price;
    }
}
