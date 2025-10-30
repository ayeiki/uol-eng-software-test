using Test.UOL.Web.Entities;
using NUnit.Framework;
using System;
using Test.UOL.Web.Services;
using Moq;
using Microsoft.Extensions.Logging;

namespace Test.UOL.Web.Tests.NAO_ALTERAR;

[TestFixture]
public class PriceCalculatorServiceTests
{
    [Test]
    public void CalculateTotal_ShouldReturnCorrectSum()
    {
        // Arrange
        var calculator = new CartTotalCalculator();
        var cart = new Cart();
        var product1 = new Product(Guid.NewGuid(), "Product 1", 10.0m);
        var product2 = new Product(Guid.NewGuid(), "Product 2", 20.0m);
        cart.CartItems.Add(new CartItem(product1, 2)); // 20.0
        cart.CartItems.Add(new CartItem(product2, 1)); // 20.0

        // Act
        var total = calculator.CalculateTotal(cart);

        // Assert
        Assert.That(total, Is.EqualTo(40.0m));
    }

    [Test]
    public void CalculateTotal_ShouldReturnZeroForEmptyCart()
    {
        // Arrange
        var calculator = new CartTotalCalculator();
        var cart = new Cart();
        // Act
        var total = calculator.CalculateTotal(cart);
        // Assert
        Assert.That(total, Is.EqualTo(0.0m));
    }
}
