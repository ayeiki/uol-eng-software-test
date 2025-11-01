using NUnit.Framework;
using Moq;
using Test.UOL.Web.Entities;
using Test.UOL.Web.Interfaces;
using Test.UOL.Web.Services;
using System;

namespace Test.UOL.Web.Tests.Cupom
{
    [TestFixture]
    public class CartTotalWithCupomCalculatorTests
    {
        private Mock<ICartTotalCalculator> _baseCalculatorMock;
        private Mock<ICupomProvider> _cupomProviderMock;
        private Mock<IDiscountCalculator> _discountCalculatorMock;
        private CartTotalWithCupomCalculator _calculator;

        [SetUp]
        public void Setup()
        {
            _baseCalculatorMock = new Mock<ICartTotalCalculator>();
            _cupomProviderMock = new Mock<ICupomProvider>();
            _discountCalculatorMock = new Mock<IDiscountCalculator>();

            _calculator = new CartTotalWithCupomCalculator(
                _baseCalculatorMock.Object,
                _cupomProviderMock.Object,
                _discountCalculatorMock.Object
            );
        }

        /// <summary>
        /// Testa o retorno do total do carrinho quando nenhum cupom está aplicado.
        /// </summary>
        [Test]
        public void CalculateTotal_ShouldReturnBaseTotal_WhenNoCupomIsApplied()
        {
            // Arrange
            var cart = new Cart();
            decimal baseTotal = 150m;

            // Instanciamos o Cart. CupomCode já é null por padrão.
            // Quando CalculateTotal for chamado, finja que o total dos itens é 150
            _baseCalculatorMock.Setup(c => c.CalculateTotal(cart)).Returns(baseTotal);

            // Act
            var finalTotal = _calculator.CalculateTotal(cart);

            // Assert
            Assert.That(finalTotal, Is.EqualTo(baseTotal));
        }

        /// <summary>
        /// Testa o retorno do total do carrinho quando um cupom está aplicado.
        /// </summary>
        [Test]
        public void CalculateTotal_ShouldReturnDiscountedTotal_WhenCupomIsApplied()
        {
            // Arrange
            string cupomCode = "PROMO10";
            var cupom = new CupomItem { key = cupomCode, type = "Percentage", value = "10" };
            var cart = new Cart { CupomCode = cupomCode };
            decimal baseTotal = 200m; // Total dos itens
            decimal discount = 20m;   // Valor do desconto

            _baseCalculatorMock.Setup(c => c.CalculateTotal(cart)).Returns(baseTotal);
            _cupomProviderMock.Setup(p => p.GetCupom(cupomCode)).Returns(cupom);
            _discountCalculatorMock.Setup(d => d.ComputeDiscount(baseTotal, CupomType.Percentage, 10m)).Returns(discount);

            // Act
            var finalTotal = _calculator.CalculateTotal(cart);

            // Assert
            Assert.That(finalTotal, Is.EqualTo(180m)); // 200 (base) - 20 (desconto) = 180
        }

        /// <summary>
        /// Testa o retorno do total do carrinho quando o desconto excede o total.
        /// </summary>
        [Test]
        public void CalculateTotal_ShouldReturnZero_WhenDiscountExceedsTotal()
        {
            // Arrange
            string cupomCode = "FLAT50";
            var cupom = new CupomItem { key = cupomCode, type = "fixed", value = "50" };
            var cart = new Cart { CupomCode = cupomCode };

            decimal baseTotal = 30m; // Total dos itens é 30
            decimal discount = 30m;  // O IDiscountCalculator deve retornar 30, não 50

            _baseCalculatorMock.Setup(c => c.CalculateTotal(cart)).Returns(baseTotal);
            _cupomProviderMock.Setup(p => p.GetCupom(cupomCode)).Returns(cupom);
            _discountCalculatorMock.Setup(d => d.ComputeDiscount(baseTotal, CupomType.Fixed, 50m)).Returns(discount); // O ComputeDiscount real já limita em 30

            // Act
            var finalTotal = _calculator.CalculateTotal(cart);

            // Assert
            Assert.That(finalTotal, Is.EqualTo(0m)); // 30 (base) - 30 (desconto) = 0
        }

        
    }
}