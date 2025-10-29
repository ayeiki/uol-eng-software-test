using System;
using System.Collections.Generic;
using Moq;
using Test.UOL.Web.Entities;
using Test.UOL.Web.Interfaces;
using Test.UOL.Web.Services;
using Test.UOL.Web.Services.CartService;
using FluentValidation;
using Bogus;
using NUnit.Framework;

namespace Test.UOL.Web.Tests
{
    [TestFixture]
    public class CartTests
    {
        private Faker<ProductDto> _productFaker;
        private Faker<CartItemDto> _cartItemFaker;
        private Mock<ICartStore> _cartStoreMock;
        private IValidator<CartItemDto> _validator;
        private CartService _cartService;


        [SetUp]
        public void Setup()
        {
            _cartStoreMock = new Mock<ICartStore>();
            _validator = new CartServiceValidator();
            _cartService = new CartService(_cartStoreMock.Object, _validator);

            _productFaker = new Faker<ProductDto>()
                .CustomInstantiator(f => new ProductDto(
                    f.Random.Int(1, 1000),
                    f.Commerce.ProductName(),
                    f.Random.Decimal(1, 1000)));

            _cartItemFaker = new Faker<CartItemDto>()
                .CustomInstantiator(f => new CartItemDto(
                    _productFaker.Generate(),
                    f.Random.Int(1, 10)));
        }
        [Test]
        public void AddItem_ShouldAddMultipleRandomItems()
        {
            // Arrange
            var items = _cartItemFaker.Generate(5);
            _cartStoreMock.Setup(s => s.GetCartItems()).Returns(new List<CartItem>());

            // Act
            foreach (var dto in items)
            {
                _cartService.AddItem(dto);
            }

            // Assert
            _cartStoreMock.Verify(s => s.AddItem(It.IsAny<CartItem>()), Times.Exactly(5));
        }

        [Test]
        public void AddItem_ShouldThrowForNegativeQuantity()
        {
            // Arrange
            var dto = _cartItemFaker.Clone().RuleFor(x => x.Quantity, -5).Generate();

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => _cartService.AddItem(dto));
            Assert.That(ex.Message, Does.Contain("Quantity must be greater than zero"));
        }

        [Test]
        public void AddItem_ShouldHandleLargeQuantities()
        {
            // Arrange
            var dto = _cartItemFaker.Clone().RuleFor(x => x.Quantity, 1000).Generate();
            _cartStoreMock.Setup(s => s.GetCartItems()).Returns(new List<CartItem>());

            // Act
            _cartService.AddItem(dto);

            // Assert
            _cartStoreMock.Verify(s => s.AddItem(It.Is<CartItem>(c => c.Quantity == 1000)), Times.Once);
        }


        [Test]
        public void AddItem_ShouldThrowExceptionForInvalidQuantity()
        {
            // Arrange
            var dto = new CartItemDto(new ProductDto(1, "Test Product", 10.0m), 0);

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => _cartService.AddItem(dto));
            Assert.That(ex.Message, Does.Contain("Quantity must be greater than zero"));
        }

        [Test]
        public void AddItem_ShouldAddValidItem()
        {
            // Arrange
            var dto = new CartItemDto(new ProductDto(1, "Valid Product", 20.0m), 2);
            _cartStoreMock.Setup(s => s.GetCartItems()).Returns(new List<CartItem>());

            // Act
            _cartService.AddItem(dto);

            // Assert
            _cartStoreMock.Verify(s => s.AddItem(It.Is<CartItem>(c => c.Product.Id == 1 && c.Quantity == 2)), Times.Once);
        }

        [Test]
        public void AddItem_ShouldUpdateQuantityIfProductExists()
        {
            // Arrange
            var existing = new CartItem(new Product(1, "Produto Existente", 10.0m), 3);
            var dto = new CartItemDto(new ProductDto(1, "Produto Existente", 10.0m), 2);
            _cartStoreMock.Setup(s => s.GetCartItems()).Returns(new List<CartItem> { existing });

            // Act
            _cartService.AddItem(dto);

            // Assert
            _cartStoreMock.Verify(s => s.ChangeItem(existing.Id, 5), Times.Once);
        }

        [Test]
        public void GetCartItems_ShouldReturnItemsFromStore()
        {
            // Arrange
            var items = new List<CartItem> { new CartItem(new Product(1, "Produto", 10.0m), 1) };
            _cartStoreMock.Setup(s => s.GetCartItems()).Returns(items);

            // Act
            var result = _cartService.GetCartItems();

            // Assert
            Assert.That(result, Is.EqualTo(items));
        }

        [Test]
        public void AddItem_ShouldThrowExceptionForNullProduct()
        {
            // Arrange
            var dto = new CartItemDto(null, 1);

            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => _cartService.AddItem(dto));
            Assert.That(ex.ParamName, Is.EqualTo("Product"));
        }

                [Test]
        public void AddItem_ShouldHandleProductWithSpecialCharacters()
        {
            // Arrange
            var specialName = "Café & Chá #1!";
            var dto = new CartItemDto(new ProductDto(99, specialName, 15.5m), 3);
            _cartStoreMock.Setup(s => s.GetCartItems()).Returns(new List<CartItem>());

            // Act
            _cartService.AddItem(dto);

            // Assert
            _cartStoreMock.Verify(s => s.AddItem(It.Is<CartItem>(c => c.Product.Name == specialName)), Times.Once);
        }

        [Test]
        public void AddItem_ShouldHandleZeroPriceProduct()
        {
            // Arrange
            var dto = new CartItemDto(new ProductDto(100, "Brinde", 0m), 2);
            _cartStoreMock.Setup(s => s.GetCartItems()).Returns(new List<CartItem>());

            // Act
            _cartService.AddItem(dto);

            // Assert
            _cartStoreMock.Verify(s => s.AddItem(It.Is<CartItem>(c => c.Product.Price == 0m)), Times.Once);
        }

        [Test]
        public void AddItem_ShouldHandleMaxIntQuantity()
        {
            // Arrange
            var dto = new CartItemDto(_productFaker.Generate(), int.MaxValue);
            _cartStoreMock.Setup(s => s.GetCartItems()).Returns(new List<CartItem>());

            // Act
            _cartService.AddItem(dto);

            // Assert
            _cartStoreMock.Verify(s => s.AddItem(It.Is<CartItem>(c => c.Quantity == int.MaxValue)), Times.Once);
        }

        [Test]
        public void AddItem_ShouldNotDuplicateProductWithSameId()
        {
            // Arrange
            var productId = 123;
            var existing = new CartItem(new Product(productId, "Produto", 10.0m), 1);
            var dto = new CartItemDto(new ProductDto(productId, "Produto", 10.0m), 2);
            _cartStoreMock.Setup(s => s.GetCartItems()).Returns(new List<CartItem> { existing });

            // Act
            _cartService.AddItem(dto);

            // Assert
            _cartStoreMock.Verify(s => s.AddItem(It.IsAny<CartItem>()), Times.Never);
            _cartStoreMock.Verify(s => s.ChangeItem(existing.Id, 3), Times.Once);
        }

        [Test]
        public void AddItem_ShouldHandleEmptyProductName()
        {
            // Arrange
            var product = _productFaker.Clone().RuleFor(x => x.Name, string.Empty).Generate();
            var dto = new CartItemDto(product, 1);
            _cartStoreMock.Setup(s => s.GetCartItems()).Returns(new List<CartItem>());

            // Act
            _cartService.AddItem(dto);

            // Assert
            _cartStoreMock.Verify(s => s.AddItem(It.Is<CartItem>(c => c.Product.Name == string.Empty)), Times.Once);
        }

        [Test]
        public void AddItem_ShouldAllowNegativePrice()
        {
            // Arrange
            var product = _productFaker.Clone().RuleFor(x => x.Price, -99.99m).Generate();
            var dto = new CartItemDto(product, 1);
            _cartStoreMock.Setup(s => s.GetCartItems()).Returns(new List<CartItem>());

            // Act
            _cartService.AddItem(dto);

            // Assert
            _cartStoreMock.Verify(s => s.AddItem(It.Is<CartItem>(c => c.Product.Price == -99.99m)), Times.Once);
        }

        [Test]
        public void AddItem_ShouldThrowForMinIntQuantity()
        {
            // Arrange
            var dto = new CartItemDto(_productFaker.Generate(), int.MinValue);

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => _cartService.AddItem(dto));
            Assert.That(ex.Message, Does.Contain("Quantity must be greater than zero"));
        }

        [Test]
        public void AddItem_ShouldHandleVeryLongProductName()
        {
            // Arrange
            var longName = new string('A', 1000);
            var product = _productFaker.Clone().RuleFor(x => x.Name, longName).Generate();
            var dto = new CartItemDto(product, 1);
            _cartStoreMock.Setup(s => s.GetCartItems()).Returns(new List<CartItem>());

            // Act
            _cartService.AddItem(dto);

            // Assert
            _cartStoreMock.Verify(s => s.AddItem(It.Is<CartItem>(c => c.Product.Name == longName)), Times.Once);
        }
    }
}
