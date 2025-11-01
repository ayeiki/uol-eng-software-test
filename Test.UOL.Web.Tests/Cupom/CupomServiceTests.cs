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
            // Arrange
            string cupomCode = "VALIDO10";
            var cupom = new CupomItem { key = cupomCode, type = "percent", value = "10" };
            // Quando GetCupom for chamado com "VALIDO10", retorne o cupom acima
            _cupomProviderMock.Setup(p => p.GetCupom(cupomCode)).Returns(cupom);
            _calculatorMock.Setup(c => c.CalculateTotal(_cart)).Returns(0m); // Retorna qualquer valor, apenas para o mock funcionar

            // Act
            _cupomService.ApplyCupomToCart(_cartId, cupomCode);
            // Assert
            Assert.That(_cart.CupomCode, Is.Not.Null);
            Assert.That(_cart.CupomCode, Is.EqualTo(cupomCode));
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