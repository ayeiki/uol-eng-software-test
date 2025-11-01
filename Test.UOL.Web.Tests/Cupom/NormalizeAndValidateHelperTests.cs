using NUnit.Framework;
using Test.UOL.Web.Entities;
using Test.UOL.Web.Helpers;
using System;

namespace Test.UOL.Web.Tests.Cupom
{
    [TestFixture]
    public class NormalizeAndValidateHelperTests
    {
        [Test]
        public void NormalizeAndValidate_ShouldReturnCorrectTuple_WhenValidPercentage()
        {
            // Arrange
            // Usamos os tipos exatos do seu JSON ("Percentage" com 'P')
            var cupom = new CupomItem { type = "Percentage", value = "10.5" };

            // Act
            var (type, value) = NormalizeAndValidateHelper.NormalizeAndValidate(cupom);

            // Assert
            Assert.That(type, Is.EqualTo(CupomType.Percentage));
            Assert.That(value, Is.EqualTo(10.5m));
        }

        [Test]
        public void NormalizeAndValidate_ShouldReturnCorrectTuple_WhenValidFixed()
        {
            // Arrange
            var cupom = new CupomItem { type = "Fixed", value = "50" };

            // Act
            var (type, value) = NormalizeAndValidateHelper.NormalizeAndValidate(cupom);

            // Assert
            Assert.That(type, Is.EqualTo(CupomType.Fixed));
            Assert.That(value, Is.EqualTo(50m));
        }

        [Test]
        public void NormalizeAndValidate_ShouldThrow_WhenTypeIsInvalid()
        {
            // Arrange
            var cupom = new CupomItem { type = "invalid_type", value = "10" };

            // Act & Assert
            var ex = Assert.Throws<CupomException>(() => NormalizeAndValidateHelper.NormalizeAndValidate(cupom));
            Assert.That(ex.Message, Is.EqualTo(ErrorMessages.CupomInvalid));
        }

        [Test]
        public void NormalizeAndValidate_ShouldThrow_WhenValueIsNonNumeric()
        {
            // Arrange
            var cupom = new CupomItem { type = "Fixed", value = "abc" };

            // Act & Assert
            var ex = Assert.Throws<CupomException>(() => NormalizeAndValidateHelper.NormalizeAndValidate(cupom));
            Assert.That(ex.Message, Is.EqualTo(ErrorMessages.CupomInvalid));
        }

        [Test]
        public void NormalizeAndValidate_ShouldThrow_WhenValueIsNegative()
        {
            // Arrange
            var cupom = new CupomItem { type = "Percentage", value = "-20" };

            // Act & Assert
            var ex = Assert.Throws<CupomException>(() => NormalizeAndValidateHelper.NormalizeAndValidate(cupom));
            Assert.That(ex.Message, Is.EqualTo(ErrorMessages.CupomInvalid));
        }
    }
}