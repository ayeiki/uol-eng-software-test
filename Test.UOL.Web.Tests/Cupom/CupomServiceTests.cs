using NUnit.Framework;
using Moq;
using Test.UOL.Web.Entities;
using Test.UOL.Web.Interfaces;
using Test.UOL.Web.Services;
using Test.UOL.Web.Helpers;
using System;
using System.Collections.Generic;

namespace Test.UOL.Web.Tests.Cupom
{
    [TestFixture]
    public class CupomServiceTests
    {
        private Mock<ICartStore> _cartStoreMock;
        private Mock<ICupomProvider> _cupomProviderMock;
        private CupomService _cupomService;
        private Cart _cart;
        private Guid _cartId;
        private Mock<ICartTotalCalculator> _calculatorMock;

        [SetUp]
        public void Setup()
        {
            // Criamos os (Mocks)
            _cartStoreMock = new Mock<ICartStore>();
            _cupomProviderMock = new Mock<ICupomProvider>();
            _calculatorMock = new Mock<ICartTotalCalculator>();

            // Instanciamos nosso serviço, injetando os mocks
            _cupomService = new CupomService(_cartStoreMock.Object, _cupomProviderMock.Object, _calculatorMock.Object);

            _cartId = Guid.NewGuid();
            _cart = new Cart { Id = _cartId, CartItems = new List<CartItem>() };
            // Quando GetCartById for chamado com _cartId, retorne o _cart
            _cartStoreMock.Setup(s => s.GetCartById(_cartId)).Returns(_cart);
        }

        /// <summary>
        /// Testa a aplicação de um cupom válido ao carrinho.
        /// </summary>
       [Test]
        public void ApplyCupomToCart_ShouldApplyValidCupom()
        {
            // --- ARRANGE ---
            
            // 1. Adicione itens ao carrinho para que haja um total base
            decimal baseTotal = 200m;
            decimal totalComDesconto = 180m; // 10% de 200 = 20 de desconto
            _cart.CartItems.Add(new CartItem(new Product(Guid.NewGuid(), "Produto", baseTotal), 1));            

            string cupomCode = "PROMO10";
            var cupom = new CupomItem { key = cupomCode, type = "Percentage", value = "10" };

            _cupomProviderMock.Setup(p => p.GetCupom(cupomCode)).Returns(cupom);

            // 2. Faça o mock do calculador retornar o NOVO total com desconto
            _calculatorMock.Setup(c => c.CalculateTotal(_cart)).Returns(totalComDesconto);

            // --- ACT ---
            _cupomService.ApplyCupomToCart(_cartId, cupomCode);

            // --- ASSERT ---
            // 1. Verifica se o código foi salvo
            Assert.That(_cart.CupomCode, Is.Not.Null);
            Assert.That(_cart.CupomCode, Is.EqualTo(cupomCode));
            
            // 2. CORREÇÃO: Verifica se o TotalAmount foi recalculado
            Assert.That(_cart.TotalAmount, Is.EqualTo(totalComDesconto));
        }

        /// <summary>
        /// Testa a aplicação de um cupom quando o carrinho já possui um cupom aplicado.
        /// </summary>
        [Test]
        public void ApplyCupomToCart_ShouldThrow_WhenCupomAlreadyExists()
        {
            // Arrange
            // Pré-configura o carrinho para JÁ TER um cupom
            string existingCode = "FLAT50";
            var existingCupom = new CupomItem { key = existingCode, type = "fixed", value = "50" };
            _cupomProviderMock.Setup(p => p.GetCupom(existingCode)).Returns(existingCupom);

            // Aplica o primeiro cupom para setar o estado do carrinho
            _cupomService.ApplyCupomToCart(_cartId, existingCode);

            // tenta aplicar o segundo
            string newCupomCode = "PROMO10";
            var newCupom = new CupomItem { key = newCupomCode, type = "Percentage", value = "10" };
            _cupomProviderMock.Setup(p => p.GetCupom(newCupomCode)).Returns(newCupom);

            // Act & Assert
            // Verificamos se uma exceção do tipo CupomException é lançada
            var ex = Assert.Throws<CupomException>(() => _cupomService.ApplyCupomToCart(_cartId, newCupomCode));
            // Verificamos se a mensagem de erro está correta
            Assert.That(ex.Message, Is.EqualTo(ErrorMessages.CupomAlreadyApplied));
        }

        /// <summary>
        /// Testa a aplicação de um cupom inválido ao carrinho.
        /// </summary>
        [Test]
        public void ApplyCupomToCart_ShouldThrow_WhenCupomIsInvalid()
        {
            // Arrange
            string cupomCode = "PROMO50";

            // O serviço chama GetCupom e espera um CupomItem
            _cupomProviderMock.Setup(p => p.GetCupom(cupomCode)).Returns((CupomItem)null);
            // Act & Assert
            var ex = Assert.Throws<CupomException>(() => _cupomService.ApplyCupomToCart(_cartId, cupomCode));
            // Verificamos se a mensagem de erro está correta
            Assert.That(ex.Message, Is.EqualTo(ErrorMessages.CupomInvalid));
        }

        /// <summary>
        /// Testa a remoção de um cupom aplicado do carrinho.
        /// </summary>
        [Test]
        public void RemoveCupomFromCart_ShouldRemoveExistingCupom()
        {
            // Arrange
            string existingCode = "FLAT50";
            var existingCupom = new CupomItem { key = existingCode, type = "fixed", value = "50" };
            _cupomProviderMock.Setup(p => p.GetCupom(existingCode)).Returns(existingCupom);
            _cupomService.ApplyCupomToCart(_cartId, existingCode);

            // Verifica se foi aplicado
            Assert.That(_cart.CupomCode, Is.EqualTo(existingCode));

            // Act
            _cupomService.RemoveCupomFromCart(_cartId);

            // Assert
            // Verifica se foi removido do carrinho a própria propriedade CupomCode
            Assert.That(_cart.CupomCode, Is.Null);
        }
    }
}