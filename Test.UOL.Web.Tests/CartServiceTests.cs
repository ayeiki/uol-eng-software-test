using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Test.UOL.Web.Entities;
using Test.UOL.Web.Interfaces;
using Test.UOL.Web.Services;
using Test.UOL.Web.Stores;

namespace Test.UOL.Web.Tests
{
    [TestFixture]
    public class CartServiceTests
    {
        private Mock<ICouponStore> _couponStoreMock;
        private Mock<ICartStore> _cartStoreMock;
        private CartService _cartService;

        [SetUp] 
        public void Setup()
        {
            _cartStoreMock = new Mock<ICartStore>();
            _couponStoreMock = new Mock<ICouponStore>();

            _cartService = new CartService(_cartStoreMock.Object, new CartTotalCalculator(), _couponStoreMock.Object);
        }


        [Test]
        public void CreateCart_ShouldReturnNewCartId()
        {
            // Act
            var result = _cartService.CreateCart();

            // Assert
            _cartStoreMock.Verify(s => s.AddCart(It.IsAny<Cart>()), Times.Once);
        }

        [Test]
        public void ApplyCoupon_PercentageType_TotalShouldBeValid()
        {
            // Arrange
            var calculator = new CartTotalCalculator();

            var cartId = Guid.NewGuid();
            var couponKey = "PERC10";
            var product = new Product(Guid.NewGuid(), string.Empty, 100m);
            var cartItem = new CartItem(product, 1);
            var cart = new Cart { Id = cartId, CartItems = new List<CartItem>() { cartItem } };
            var coupon = new Coupon { Key = "PERC10", Type = Enums.ECouponType.Percentage, Value = 10 };

            _cartStoreMock.Setup(s => s.GetCartById(cartId)).Returns(cart);
            _couponStoreMock.Setup(c => c.GetCouponByKey(couponKey)).Returns(coupon); ;

            // Act
            _cartService.ApplyCoupon(cartId, couponKey);
            var total = calculator.CalculateTotal(cart);

            // Assert
            Assert.That(total, Is.EqualTo(90.0m));
        }


        [Test]
        public void ApplyCoupon_FixedType_TotalShouldBeValid()
        {
            // Arrange
            var calculator = new CartTotalCalculator();

            var cartId = Guid.NewGuid();
            var couponKey = "PERC10";
            var product = new Product(Guid.NewGuid(), string.Empty, 100m);
            var cartItem = new CartItem(product, 1);
            var cart = new Cart { Id = cartId, CartItems = new List<CartItem>() { cartItem } };
            var coupon = new Coupon { Key = "FIXED35", Type = Enums.ECouponType.Fixed, Value = 35 };

            _cartStoreMock.Setup(s => s.GetCartById(cartId)).Returns(cart);
            _couponStoreMock.Setup(c => c.GetCouponByKey(couponKey)).Returns(coupon); ;

            // Act
            _cartService.ApplyCoupon(cartId, couponKey);
            var total = calculator.CalculateTotal(cart);

            // Assert
            Assert.That(total, Is.EqualTo(65.0m));
        }

        [Test]
        public void ApplyCoupon_ShouldAlreadyUsed()
        {
            // Arrange
            var calculator = new CartTotalCalculator();

            var cartId = Guid.NewGuid();
            var couponKey = "PERC10";
            var product = new Product(Guid.NewGuid(), string.Empty, 100m);
            var cartItem = new CartItem(product, 1);
            var cart = new Cart { Id = cartId, CartItems = new List<CartItem>() { cartItem } };
            var coupon = new Coupon { Key = "PERC10", Type = Enums.ECouponType.Percentage, Value = 10 };

            _cartStoreMock.Setup(s => s.GetCartById(cartId)).Returns(cart);
            _couponStoreMock.Setup(c => c.GetCouponByKey(couponKey)).Returns(coupon); ;

            // Act
            _cartService.ApplyCoupon(cartId, couponKey);            

            // Assert
            var ex = Assert.Throws<ArgumentException>(() => _cartService.ApplyCoupon(cartId, couponKey));
            Assert.That(ex.Message, Does.Contain("Coupon already applied to cart"));
        }
    }
}
