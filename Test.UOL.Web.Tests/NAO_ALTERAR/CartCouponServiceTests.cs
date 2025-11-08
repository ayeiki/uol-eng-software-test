using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using Test.UOL.Web.Entities;
using Test.UOL.Web.Interfaces;
using Test.UOL.Web.Services;

namespace Test.UOL.Web.Tests.NAO_ALTERAR;

[TestFixture]
public class CartCouponServiceTests
{
    private Mock<ICartStore> _cartStoreMock;
    private Mock<ICartTotalCalculator> _cartTotalCalculator = new();
    private CartService _cartService;

    [SetUp]
    public void SetUp()
    {
        _cartStoreMock = new Mock<ICartStore>();
        _cartService = new CartService(_cartStoreMock.Object, _cartTotalCalculator.Object);
    }

    [Test]
    public void AddCouponToCart_ShouldReturnZeroForEmptyCart()
    {
        // Arrange
        var calculator = new CartCouponService(new CartTotalCalculator(), _cartService);
        var cartId = Guid.NewGuid();
        var cart = new Cart { Id = cartId, CartItems = new List<CartItem>() };
        _cartStoreMock.Setup(s => s.GetCartById(cartId)).Returns(cart);

        var couponKey = "Promo20";

        // Act
        var total = calculator.AddCouponToCart(cart.Id, couponKey).TotalAmount;

        // Assert
        Assert.That(total, Is.EqualTo(0.0m));
    }

    [Test]
    public void AddCouponToCart_ShouldReturnZeroForDiscountBiggerThanAmount()
    {
        // Arrange
        var calculator = new CartCouponService(new CartTotalCalculator(), _cartService);
        var cartId = Guid.NewGuid();
        var cart = new Cart { Id = cartId, CartItems = new List<CartItem>() };
        _cartStoreMock.Setup(s => s.GetCartById(cartId)).Returns(cart);

        var couponKey = "FLAT50";
        var product = new Product(Guid.NewGuid(), "Product 1", 10.0m);
        cart.CartItems.Add(new CartItem(product, 2)); // 20.0

        // Act
        var total = calculator.AddCouponToCart(cart.Id, couponKey).TotalAmount;

        // Assert
        Assert.That(total, Is.EqualTo(0.0m));
    }

    [Test]
    public void AddCouponToCart_ShouldReturnCorrectFixedDiscount()
    {
        // Arrange
        var cartId = Guid.NewGuid();
        var cart = new Cart { Id = cartId, CartItems = new List<CartItem>() };
        _cartStoreMock.Setup(s => s.GetCartById(cartId)).Returns(cart);

        var calculator = new CartCouponService(new CartTotalCalculator(), _cartService);
        var couponKey = "FLAT50";
        var product = new Product(Guid.NewGuid(), "Product 1", 50.0m);
        cart.CartItems.Add(new CartItem(product, 2));

        // Act
        var total = calculator.AddCouponToCart(cart.Id, couponKey).TotalAmount;

        // Assert
        Assert.That(total, Is.EqualTo(50.0m));
    }

    [Test]
    public void AddCouponToCart_ShouldReturnCorrectPercentualDiscount()
    {
        // Arrange
        var calculator = new CartCouponService(new CartTotalCalculator(), _cartService);
        var cartId = Guid.NewGuid();
        var cart = new Cart { Id = cartId, CartItems = new List<CartItem>() };
        _cartStoreMock.Setup(s => s.GetCartById(cartId)).Returns(cart);

        var couponKey = "Promo20";
        var product = new Product(Guid.NewGuid(), "Product 1", 10.0m);
        cart.CartItems.Add(new CartItem(product, 2)); // 20.0

        // Act
        var total = calculator.AddCouponToCart(cart.Id, couponKey).TotalAmount;

        // Assert
        Assert.That(total, Is.EqualTo(16.0m));
    }

    [Test]
    public void AddCouponToCart_ShouldThrowExceptionForNotFound()
    {
        // Arrange
        var calculator = new CartCouponService(new CartTotalCalculator(), _cartService);
        var cartId = Guid.NewGuid();
        var cart = new Cart { Id = cartId, CartItems = new List<CartItem>() };
        _cartStoreMock.Setup(s => s.GetCartById(cartId)).Returns(cart);

        var couponKey = "ForFree";
        var product = new Product(Guid.NewGuid(), "Product 1", 10.0m);
        cart.CartItems.Add(new CartItem(product, 2)); // 20.0

        // Act && Assert
        Assert.Throws<ArgumentException>(() => calculator.AddCouponToCart(cart.Id, couponKey));
    }

    [Test]
    public void AddCouponToCart_ShouldThrowExceptionForTypeInvalid()
    {
        // Arrange
        var calculator = new CartCouponService(new CartTotalCalculator(), _cartService);
        var cartId = Guid.NewGuid();
        var cart = new Cart { Id = cartId, CartItems = new List<CartItem>() };
        _cartStoreMock.Setup(s => s.GetCartById(cartId)).Returns(cart);

        var couponKey = "DISCOUNT10";
        var product = new Product(Guid.NewGuid(), "Product 1", 10.0m);
        cart.CartItems.Add(new CartItem(product, 2)); // 20.0

        // Act && Assert
        Assert.Throws<ArgumentException>(() => calculator.AddCouponToCart(cart.Id, couponKey));
    }

    [Test]
    public void AddCouponToCart_ShouldThrowExceptionForApllyCouponTwice()
    {
        // Arrange
        var calculator = new CartCouponService(new CartTotalCalculator(), _cartService);
        var cartId = Guid.NewGuid();
        var cart = new Cart { Id = cartId, CartItems = new List<CartItem>() };
        _cartStoreMock.Setup(s => s.GetCartById(cartId)).Returns(cart);

        var couponKey = "PROMO20";
        var product = new Product(Guid.NewGuid(), "Product 1", 10.0m);
        cart.CartItems.Add(new CartItem(product, 2)); // 20.0

        cart = calculator.AddCouponToCart(cart.Id, couponKey);

        // Act && Assert
        Assert.Throws<ArgumentException>(() => calculator.AddCouponToCart(cart.Id, couponKey));
    }

    [Test]
    public void AddCouponToCart_ShouldThrowExceptionForValueInvalid()
    {
        // Arrange
        var calculator = new CartCouponService(new CartTotalCalculator(), _cartService);
        var cartId = Guid.NewGuid();
        var cart = new Cart { Id = cartId, CartItems = new List<CartItem>() };
        _cartStoreMock.Setup(s => s.GetCartById(cartId)).Returns(cart);

        var couponKey = "DISCOUNT20";
        var product = new Product(Guid.NewGuid(), "Product 1", 10.0m);
        cart.CartItems.Add(new CartItem(product, 2)); // 20.0

        // Act && Assert
        Assert.Throws<ArgumentException>(() => calculator.AddCouponToCart(cart.Id, couponKey));
    }
}