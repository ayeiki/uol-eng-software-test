using NUnit.Framework;
using Moq;
using Test.UOL.Web.Entities;
using Test.UOL.Web.Interfaces;
using Test.UOL.Web.Services;
using System;
using System.Collections.Generic;

namespace Test.UOL.Web.Tests.Cupom
{
    [TestFixture]
    public class CartItemServiceWithCupomTests
    {
        private Mock<ICartStore> _cartStoreMock;
        private CartItemService _cartItemService;
        private CartTotalCalculator _baseCalculator;
        private Mock<ICupomProvider> _cupomProviderMock;
        private Mock<IDiscountCalculator> _discountCalculatorMock;
        private CartTotalWithCupomCalculator _cupomTotalCalculator;

        [SetUp]
        public void Setup()
        {
            _baseCalculator = new CartTotalCalculator();
            _cupomProviderMock = new Mock<ICupomProvider>();
            _discountCalculatorMock = new Mock<IDiscountCalculator>();

            _cupomTotalCalculator = new CartTotalWithCupomCalculator(
                _baseCalculator,
                _cupomProviderMock.Object,
                _discountCalculatorMock.Object
            );

            _cartStoreMock = new Mock<ICartStore>();

            _cartItemService = new CartItemService(
                _cartStoreMock.Object,
                _cupomTotalCalculator
            );
        }

        [Test]
        public void PutItemInCart_ShouldUpdateTotalWithDiscount_WhenCupomIsApplied()
        {
            // --- ARRANGE (Preparar) ---

            // 1. Criar o carrinho com um cupom JÁ aplicado
            var cartId = Guid.NewGuid();
            string cupomCode = "PROMO10";
            var cart = new Cart
            {
                Id = cartId,
                CartItems = new List<CartItem>(),
                CupomCode = cupomCode
            };

            // 2. Adicionar o item ao carrinho
            var product = new Product(Guid.NewGuid(), "Produto Caro", 200m);
            var itemToAdd = new CartItem(product, 1); // Total base será 200

            // 3. Configurar os mocks
            _cartStoreMock.Setup(s => s.GetCartById(cartId)).Returns(cart);
            var cupom = new CupomItem { key = cupomCode, type = "Percentage", value = "10" };
            _cupomProviderMock.Setup(p => p.GetCupom(cupomCode)).Returns(cupom);

            //Calcular o desconto de 10% sobre 200 = 20
            _discountCalculatorMock.Setup(d => d.ComputeDiscount(200m, CupomType.Percentage, 10m)).Returns(20m);

            // --- ACT ---
            // Adicionar o item ao carrinho
            _cartItemService.PutItemInCart(cartId, itemToAdd);

            // --- ASSERT ---
            // O total deve ser o total base menos o desconto do cupom
            Assert.That(cart.TotalAmount, Is.EqualTo(180m)); // 200 (base) - 20 (desconto) = 180
        }

        [Test]
        public void PutItemInCart_ShouldUpdateTotalWithoutDiscount_WhenNoCupomExists()
        {
            // --- ARRANGE ---

            // 1. Criar o carrinho SEM cupom
            var cartId = Guid.NewGuid();
            var cart = new Cart
            {
                Id = cartId,
                CartItems = new List<CartItem>()
            };

            // 2. Criar o item
            var product = new Product(Guid.NewGuid(), "Produto", 150m);
            var itemToAdd = new CartItem(product, 1); // Total base será 150

            // 3. Configurar mocks
            _cartStoreMock.Setup(s => s.GetCartById(cartId)).Returns(cart);

            // --- ACT ---
            _cartItemService.PutItemInCart(cartId, itemToAdd);

            // --- ASSERT  ---

            // O total deve ser apenas o total base, calculado pelo _baseCalculator
            Assert.That(cart.TotalAmount, Is.EqualTo(150m));
        }

        [Test]
        public void DeleteItem_ShouldRecalculateTotalWithDiscount_WhenCupomIsApplied()
        {
            // --- ARRANGE---

            // 1. Definir produtos e itens
            var productA_Id = Guid.NewGuid();
            var productA = new Product(productA_Id, "Produto A", 100m);
            var itemA = new CartItem(productA, 1); // R$ 100

            var productB_Id = Guid.NewGuid();
            var productB = new Product(productB_Id, "Produto B", 50m);
            var itemB = new CartItem(productB, 2); // R$ 100

            // 2. Criar o carrinho com cupom e os DOIS itens
            var cartId = Guid.NewGuid();
            string cupomCode = "PROMO10";
            var cart = new Cart
            {
                Id = cartId,
                CartItems = new List<CartItem> { itemA, itemB }, // Total base inicial = 200
                CupomCode = cupomCode
            };

            // 3. Configurar os mocks
            _cartStoreMock.Setup(s => s.GetCartById(cartId)).Returns(cart);
            var cupom = new CupomItem { key = cupomCode, type = "Percentage", value = "10" };
            _cupomProviderMock.Setup(p => p.GetCupom(cupomCode)).Returns(cupom);

            // Desconto inicial de 10% sobre 200 = 20
            _discountCalculatorMock.Setup(d => d.ComputeDiscount(200m, CupomType.Percentage, 10m)).Returns(20m);
                        
            _discountCalculatorMock.Setup(d => d.ComputeDiscount(100m, CupomType.Percentage, 10m)).Returns(10m);

            // --- ACT ---            
            _cartItemService.DeleteItem(cartId, itemA.Id);

            // --- ASSERT ---
            Assert.That(cart.CartItems.Count, Is.EqualTo(1)); // só deve restar o item B
            Assert.That(cart.CartItems[0].Product.Id, Is.EqualTo(productB_Id));

            // Novo total base = 100 (itemB)
            // Novo desconto = 10 (10% de 100)
            // Total final = 90
            Assert.That(cart.TotalAmount, Is.EqualTo(90m));
        }
    }
}