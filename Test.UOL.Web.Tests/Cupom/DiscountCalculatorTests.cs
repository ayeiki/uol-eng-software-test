using NUnit.Framework;
using Test.UOL.Web.Entities;
using Test.UOL.Web.Services;

namespace Test.UOL.Web.Tests.Cupom
{
    [TestFixture]
    public class DiscountCalculatorTests
    {
        private DiscountCalculator _calculator;

        [SetUp]
        public void Setup()
        {
            _calculator = new DiscountCalculator();
        }

        /// <summary>
        /// Testa o cálculo de desconto percentual.
        /// </summary>
        [Test]
        public void ComputeDiscount_ShouldCalculatePercentCorrectly()
        {
            // Arrange
            var type = CupomType.Percentage;
            var value = 10m;
            decimal total = 200m;

            // Act
            var discount = _calculator.ComputeDiscount(total, type, value);

            // Assert
            Assert.That(discount, Is.EqualTo(20m)); // 10% de 200 = 20
        }

        /// <summary>
        /// Testa o cálculo de desconto fixo.
        /// </summary>
        [Test]
        public void CalculateDiscount_ShouldCalculateFixedCorrectly()
        {
            // Arrange
            var type = CupomType.Fixed;
            var value = 50m;
            decimal total = 200m;

            // Act
            var discount = _calculator.ComputeDiscount(total, type, value);
            // Assert
            Assert.That(discount, Is.EqualTo(50m)); // Desconto fixo de R$ 50
        }

        /// <summary>
        /// Testa se o desconto fixo não ultrapassa o total do carrinho.
        /// </summary>
        [Test]
        public void CalculateDiscount_ShouldCapDiscountAtTotal_WhenFixedIsGreater()
        {
            // Arrange
            var type = CupomType.Fixed;
            var value = 50m;  // R$ 50 de desconto
            decimal total = 30m; // Carrinho só tem R$ 30

            // Act
            var discount = _calculator.ComputeDiscount(total, type, value);

            // Assert
            Assert.That(discount, Is.EqualTo(30m)); // O desconto não pode ser 50, tem que ser no máximo 30
        }

        /// <summary>
        /// Testa o comportamento com um tipo de cupom desconhecido.
        /// </summary>
        [Test]
        public void CalculateDiscount_ShouldReturnZero_ForUnknownType()
        {
            // Arrange
            var type = (CupomType)99;
            var value = 100m;
            decimal total = 200m;

            // Act
            var discount = _calculator.ComputeDiscount(total, type, value);
            // Assert
            Assert.That(discount, Is.EqualTo(0m)); // Tipo desconhecido retorna 0 de desconto
        }
    }
}