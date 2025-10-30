using Moq;
using Test.UOL.Web.Entities;
using Test.UOL.Web.Interfaces;
using Test.UOL.Web.Services;
using NUnit.Framework;

namespace Test.UOL.Web.Tests
{
    [TestFixture]
    public class CartServiceTests
    {

        private Mock<ICartStore> _cartStoreMock;
        private CartService _cartService;

        [SetUp] 
        public void Setup()
        {
            _cartStoreMock = new Mock<ICartStore>();

            _cartService = new CartService(_cartStoreMock.Object, new CartTotalCalculator());
        }


        [Test]
        public void CreateCart_ShouldReturnNewCartId()
        {
            // Act
            var result = _cartService.CreateCart();

            // Assert
            _cartStoreMock.Verify(s => s.AddCart(It.IsAny<Cart>()), Times.Once);
        }

    }
}
