using System;
using System.Collections.Generic;
using Moq;
using Test.UOL.Web.Entities;
using Test.UOL.Web.Interfaces;
using Test.UOL.Web.Services;
using Bogus;
using NUnit.Framework;
using System.Linq;

[TestFixture]

public class CartItemServiceTests
{
    private Faker<Product> _productFaker;
    private Faker<CartItem> _cartItemFaker;
    private Mock<ICartStore> _cartStoreMock;
    private CartItemService _cartItemService;


    [SetUp]
    public void Setup()
    {
        _cartStoreMock = new Mock<ICartStore>();

        _cartItemService = new CartItemService(_cartStoreMock.Object, new CartTotalCalculator());

        _productFaker = new Faker<Product>()
            .CustomInstantiator(f => new Product(
                f.Random.Guid(),
                f.Commerce.ProductName(),
                f.Random.Decimal(1, 1000)));

        _cartItemFaker = new Faker<CartItem>()
            .CustomInstantiator(f => new CartItem(
                _productFaker.Generate(),
                f.Random.Int(1, 10)));
    }

    [Test]
    public void PutItemInCart_ShouldAddMultipleRandomItems()
    {
        // Arrange
        var cartId = Guid.NewGuid();
        var items = _cartItemFaker.Generate(5);
        var cart = new Cart { Id = cartId, CartItems = new List<CartItem>() };
        _cartStoreMock.Setup(s => s.GetCartById(cartId)).Returns(cart);

        // Act
        foreach (var dto in items)
        {
            _cartItemService.PutItemInCart(cartId, dto);
        }

        // Assert
        Assert.That(cart.CartItems.Count, Is.EqualTo(5));
        Assert.That(cart.TotalAmount, Is.EqualTo(items.Sum(i => i.Product.Price * i.Quantity)));
    }

    [Test]
    public void PutItemInCart_ShouldThrowForNegativeQuantity()
    {
        // Arrange
        var cartId = Guid.NewGuid();
        var dto = _cartItemFaker.Clone().RuleFor(x => x.Quantity, -5).Generate();
        _cartStoreMock.Setup(s => s.GetCartById(cartId)).Returns(new Cart { Id = cartId, CartItems = new List<CartItem>() });
        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => _cartItemService.PutItemInCart(cartId, dto));
        Assert.That(ex.Message, Does.Contain("Quantity must be greater than zero"));
    }

    [Test]
    public void PutItemInCart_ShouldHandleLargeQuantities()
    {
        // Arrange
        var cartId = Guid.NewGuid();
        var dto = _cartItemFaker.Clone().RuleFor(x => x.Quantity, 1000).Generate();
        var cart = new Cart { Id = cartId, CartItems = new List<CartItem>() };
        _cartStoreMock.Setup(s => s.GetCartById(cartId)).Returns(cart);

        // Act
        _cartItemService.PutItemInCart(cartId, dto);

        // Assert
        Assert.That(cart.CartItems.Count, Is.EqualTo(1));
        Assert.That(cart.CartItems[0].Quantity, Is.EqualTo(1000));
        Assert.That(cart.TotalAmount, Is.EqualTo(dto.Product.Price * 1000));
    }

    [Test]
    public void PutItemInCart_ShouldAddValidItem()
    {
        // Arrange
        var cartId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var cartItem = new CartItem(new Product(productId, "Valid Product", 20.0m), 2);
        var cart = new Cart { Id = cartId, CartItems = new List<CartItem>() };
        _cartStoreMock.Setup(s => s.GetCartById(cartId)).Returns(cart);

        // Act
        _cartItemService.PutItemInCart(cartId, cartItem);

        // Assert
        Assert.That(cart.CartItems.Count, Is.EqualTo(1));
        Assert.That(cart.CartItems[0].Product.Name, Is.EqualTo("Valid Product"));
        Assert.That(cart.CartItems[0].Quantity, Is.EqualTo(2));
        Assert.That(cart.TotalAmount, Is.EqualTo(40.0m));
    }

    [Test]
    public void PutItemInCart_ShouldUpdateQuantityIfProductExists()
    {
        // Arrange
        var cartId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var existing = new CartItem(new Product(productId, "Produto Existente", 10.0m), 3);
        var cartItem = new CartItem(new Product(productId, "Produto Existente", 10.0m), 2);
        var cart = new Cart { Id = cartId, CartItems = new List<CartItem> { existing } };
        _cartStoreMock.Setup(s => s.GetCartById(cartId)).Returns(cart);

        // Act
        _cartItemService.PutItemInCart(cartId, cartItem);

        // Assert
        Assert.That(cart.CartItems.Count, Is.EqualTo(1));
        Assert.That(cart.CartItems[0].Quantity, Is.EqualTo(5));
        Assert.That(cart.TotalAmount, Is.EqualTo(50.0m));
    }
    [Test]
    public void PutItemInCart_ShouldThrowExceptionForNullProduct()
    {
        // Arrange
        var cartId = Guid.NewGuid();
        var cartItem = new CartItem(null, 1);

        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() => _cartItemService.PutItemInCart(cartId, cartItem));
        Assert.That(ex.ParamName, Is.EqualTo("Product"));
    }


    [Test]
    public void PutItemInCart_ShouldHandleProductWithSpecialCharacters()
    {
        // Arrange
        var cartId = Guid.NewGuid();
        var specialName = "Café & Chá #1!";
        var cartItem = new CartItem(new Product(Guid.NewGuid(), specialName, 15.5m), 3);
        var cart = new Cart { Id = cartId, CartItems = new List<CartItem>() };
        _cartStoreMock.Setup(s => s.GetCartById(cartId)).Returns(cart);

        // Act
        _cartItemService.PutItemInCart(cartId, cartItem);

        // Assert
        Assert.That(cart.CartItems.Count, Is.EqualTo(1));
        Assert.That(cart.CartItems[0].Product.Name, Is.EqualTo(specialName));
        Assert.That(cart.CartItems[0].Quantity, Is.EqualTo(3));
        Assert.That(cart.TotalAmount, Is.EqualTo(46.5m));
    }

    [Test]
    public void PutItemInCart_ShouldHandleZeroPriceProduct()
    {
        // Arrange
        var cartId = Guid.NewGuid();
        var cartItem = new CartItem(new Product(Guid.NewGuid(), "Brinde", 0m), 2);
        var cart = new Cart { Id = cartId, CartItems = new List<CartItem>() };
        _cartStoreMock.Setup(s => s.GetCartById(cartId)).Returns(cart);

        // Act
        _cartItemService.PutItemInCart(cartId, cartItem);

        // Assert
        Assert.That(cart.CartItems.Count, Is.EqualTo(1));
        Assert.That(cart.CartItems[0].Product.Name, Is.EqualTo("Brinde"));
        Assert.That(cart.CartItems[0].Product.Price, Is.EqualTo(0m));
        Assert.That(cart.CartItems[0].Quantity, Is.EqualTo(2));
        Assert.That(cart.TotalAmount, Is.EqualTo(0m));
    }

    [Test]
    public void PutItemInCart_ShouldHandleMaxIntQuantity()
    {
        // Arrange
        var cartId = Guid.NewGuid();
        var cartItem = new CartItem(_productFaker.Generate(), int.MaxValue);
        var cart = new Cart { Id = cartId, CartItems = new List<CartItem>() };
        _cartStoreMock.Setup(s => s.GetCartById(cartId)).Returns(cart);

        // Act
        _cartItemService.PutItemInCart(cartId, cartItem);

        // Assert
        Assert.That(cart.CartItems.Count, Is.EqualTo(1));
        Assert.That(cart.CartItems[0].Quantity, Is.EqualTo(int.MaxValue));
        Assert.That(cart.TotalAmount, Is.EqualTo(cartItem.Product.Price * int.MaxValue));
    }

    [Test]
    public void AddItem_ShouldNotDuplicateProductWithSameId()
    {
        // Arrange
        var cartId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var existing = new CartItem(new Product(productId, "Produto", 10.0m), 1);
        var cartItem = new CartItem(new Product(productId, "Produto", 10.0m), 2);
        var cart = new Cart { Id = cartId, CartItems = new List<CartItem> { existing } };
        _cartStoreMock.Setup(s => s.GetCartById(cartId)).Returns(cart);

        // Act
        _cartItemService.PutItemInCart(cartId, cartItem);

        // Assert
        Assert.That(cart.CartItems.Count, Is.EqualTo(1));
        Assert.That(cart.CartItems[0].Quantity, Is.EqualTo(3));
        Assert.That(cart.TotalAmount, Is.EqualTo(30.0m));
    }

    [Test]
    public void AddItem_ShouldHandleEmptyProductName()
    {
        // Arrange
        var cartId = Guid.NewGuid();
        var product = new Product(Guid.NewGuid(), string.Empty, 0m);
        var cartItem = new CartItem(product, 1);
        var cart = new Cart { Id = cartId, CartItems = new List<CartItem>() };
        _cartStoreMock.Setup(s => s.GetCartById(cartId)).Returns(cart);

        // Act
        _cartItemService.PutItemInCart(cartId, cartItem);

        // Assert
        Assert.That(cart.CartItems.Count, Is.EqualTo(1));
        Assert.That(cart.CartItems[0].Product.Name, Is.EqualTo(string.Empty));
        Assert.That(cart.CartItems[0].Quantity, Is.EqualTo(1));
        Assert.That(cart.TotalAmount, Is.EqualTo(product.Price * 1));
    }

    [Test]
    public void AddItem_ShouldThrowForMinIntQuantity()
    {
        // Arrange
        var cartId = Guid.NewGuid();
        var cartItem = new CartItem(_productFaker.Generate(), int.MinValue);
        _cartStoreMock.Setup(s => s.GetCartById(cartId)).Returns(new Cart { Id = cartId, CartItems = new List<CartItem>() });

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => _cartItemService.PutItemInCart(cartId, cartItem));
        Assert.That(ex.Message, Does.Contain("Quantity must be greater than zero"));
    }

    [Test]
    public void AddItem_ShouldHandleVeryLongProductName()
    {
        // Arrange
        var cartId = Guid.NewGuid();
        var longName = new string('A', 1000);
        var product = new Product(Guid.NewGuid(), longName, 0m);
        var cartItem = new CartItem(product, 1);
        var cart = new Cart { Id = cartId, CartItems = new List<CartItem>() };
        _cartStoreMock.Setup(s => s.GetCartById(cartId)).Returns(cart);

        // Act
        _cartItemService.PutItemInCart(cartId, cartItem);

        // Assert
        Assert.That(cart.CartItems.Count, Is.EqualTo(1));
        Assert.That(cart.CartItems[0].Product.Name, Is.EqualTo(longName));
        Assert.That(cart.CartItems[0].Quantity, Is.EqualTo(1));
        Assert.That(cart.TotalAmount, Is.EqualTo(product.Price * 1));
    }



}