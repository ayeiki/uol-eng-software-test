using Bogus;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Test.UOL.Web.Dtos;
using Test.UOL.Web.Entities;
using Test.UOL.Web.Interfaces;
using Test.UOL.Web.Services;
using Test.UOL.Web.Stores;

namespace Test.UOL.Web.Tests
{
    [TestFixture]
    public class CartServiceTests
    {
        private Faker<Product> _productFaker;
        private Faker<CartItem> _cartItemFaker;
        private Faker<List<CouponDiscount>> _couponDiscountsFaker;

        private CartStoreFakerMemory _cartStoreMock;
        private CartService _cartService;

        [SetUp] 
        public void Setup()
        {
            _cartStoreMock = new CartStoreFakerMemory();
            _cartService = new CartService(_cartStoreMock, new CartTotalCalculator());

            _productFaker = new Faker<Product>()
            .CustomInstantiator(f => new Product(
                f.Random.Guid(),
                f.Commerce.ProductName(),
                f.Random.Decimal(1, 1000)));

            _cartItemFaker = new Faker<CartItem>()
                .CustomInstantiator(f => new CartItem(
                    _productFaker.Generate(),
                    f.Random.Int(1, 10)));

            _cartStoreMock.AddCoupon(new CouponDiscount
            {
                Key = "PROMO20",
                Type = Entities.Enums.CouponDiscountType.Percentage,
                Value = 20
            });

            _cartStoreMock.AddCoupon(new CouponDiscount
            {
                Key = "FLAT50",
                Type = Entities.Enums.CouponDiscountType.Fixed,
                Value = 50
            });
        }


        [Test]
        public void CreateCart_ShouldReturnNewCartId()
        {
            // Act
            var cart = _cartService.CreateCart();

            // Assert
            Assert.That(cart, Is.Not.Null);
            Assert.That(cart.Id, Is.Not.EqualTo(Guid.Empty));
            Assert.That(_cartStoreMock.GetCartById(cart.Id), Is.Not.Null);
        }


        [Test]
        public void FullFlow_ShouldCreateCart_AndApplyFixedCoupon()
        {
            // === 1. Criar carrinho ===
            var cart = _cartService.CreateCart();
            Assert.That(cart, Is.Not.Null);

            // === 2. Adicionar itens com Bogus ===
            cart.CartItems.AddRange(_cartItemFaker.Generate(3));
            cart.TotalAmount = new CartTotalCalculator().CalculateTotal(cart);
            Assert.That(cart.TotalAmount, Is.GreaterThan(0));

            var originalTotal = cart.TotalAmount;

            // === 3. Aplicar cupom fixo ===
            var request = new CouponDiscountInCartRequest
            {
                CartId = cart.Id,
                CouponDiscountKey = "FLAT50"
            };

            var updatedCart = _cartService.ApplyCouponDiscount(request);
            var expectedDiscount = 50m;
            var expectedTotal = originalTotal - 50m;

            // === 4. Validar resultado ===
            Assert.That(updatedCart.CouponDiscountApplied.Key, Is.EqualTo("FLAT50"));
            Assert.That(updatedCart.DiscountAmountInCart, Is.EqualTo(expectedDiscount));
            Assert.That(updatedCart.TotalAmount, Is.EqualTo(expectedTotal));
            Assert.That(updatedCart.CartItems.Count, Is.EqualTo(3));
        }

        [Test]
        public void FullFlow_ShouldCreateCart_AndApplyPercentageCoupon()
        {
            var cart = _cartService.CreateCart();
            Assert.That(cart, Is.Not.Null);

            cart.CartItems.AddRange(_cartItemFaker.Generate(2));
            cart.TotalAmount = new CartTotalCalculator().CalculateTotal(cart);
            Assert.That(cart.TotalAmount, Is.GreaterThan(0));

            var originalTotal = cart.TotalAmount;

            var request = new CouponDiscountInCartRequest
            {
                CartId = cart.Id,
                CouponDiscountKey = "PROMO20"
            };

            var updatedCart = _cartService.ApplyCouponDiscount(request);
            var expectedDiscount = originalTotal * 0.20m;
            var expectedTotal = originalTotal - expectedDiscount;

            // === 4. Validar resultado ===
            Assert.That(updatedCart.CouponDiscountApplied.Key, Is.EqualTo("PROMO20"));
            Assert.That(updatedCart.DiscountAmountInCart, Is.EqualTo(expectedDiscount).Within(0.01m));
            Assert.That(updatedCart.TotalAmount, Is.EqualTo(expectedTotal).Within(0.01m));
            Assert.That(updatedCart.CartItems.Count, Is.EqualTo(2));
        }

        [Test]
        public void FullFlow_ShouldThrow_WhenCouponIsInvalid()
        {
            var cart = _cartService.CreateCart();
            Assert.That(cart, Is.Not.Null);

            cart.CartItems.AddRange(_cartItemFaker.Generate(6));
            cart.TotalAmount = new CartTotalCalculator().CalculateTotal(cart);
            Assert.That(cart.TotalAmount, Is.GreaterThan(0));

            var originalTotal = cart.TotalAmount;

            var request = new CouponDiscountInCartRequest
            {
                CartId = cart.Id,
                CouponDiscountKey = "INVALIDO"
            };

            var ex = Assert.Throws<ArgumentException>(() => _cartService.ApplyCouponDiscount(request));
            Assert.That(ex.Message, Is.EqualTo("Cupom inválido."));
        }

        [Test]
        public void FullFlow_ShouldThrow_WhenApplyingCouponToEmptyCart()
        {
            var cart = _cartService.CreateCart();
            Assert.That(cart, Is.Not.Null);
            Assert.That(cart.CartItems.Count, Is.EqualTo(0));

            var request = new CouponDiscountInCartRequest
            {
                CartId = cart.Id,
                CouponDiscountKey = "FLAT50"
            };

            var ex = Assert.Throws<ArgumentException>(() => _cartService.ApplyCouponDiscount(request));
            Assert.That(ex.Message, Is.EqualTo("O seu carrinho está vazio."));
        }

        [Test]
        public void FullFlow_ShouldThrow_WhenCouponAlreadyApplied()
        {
            var cart = _cartService.CreateCart();
            Assert.That(cart, Is.Not.Null);

            cart.CartItems.AddRange(_cartItemFaker.Generate(3));
            cart.TotalAmount = new CartTotalCalculator().CalculateTotal(cart);
            Assert.That(cart.TotalAmount, Is.GreaterThan(0));

            var firstRequest = new CouponDiscountInCartRequest
            {
                CartId = cart.Id,
                CouponDiscountKey = "FLAT50"
            };
            var updatedCart = _cartService.ApplyCouponDiscount(firstRequest);

            var secondRequest = new CouponDiscountInCartRequest
            {
                CartId = cart.Id,
                CouponDiscountKey = "PERCENT20"
            };

            var ex = Assert.Throws<ArgumentException>(() => _cartService.ApplyCouponDiscount(secondRequest));
            Assert.That(ex.Message, Is.EqualTo("Já existe um cupom aplicado."));
        }
    }
}
