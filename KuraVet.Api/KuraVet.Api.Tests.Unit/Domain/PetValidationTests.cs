using System.ComponentModel.DataAnnotations;
using KuraVet.Api.Models;
using Xunit;

namespace KuraVet.Api.Tests.Unit.Domain
{
    public class PetValidationTests
    {
        private static IList<ValidationResult> Validar(Pet pet)
        {
            var context = new ValidationContext(pet);
            var resultados = new List<ValidationResult>();
            Validator.TryValidateObject(pet, context, resultados, validateAllProperties: true);
            return resultados;
        }

        [Fact]
        public void Validate_PetComDadosValidos_DeveRetornarValido()
        {
            // Arrange
            var pet = new Pet { Nome = "Rex", TutorId = 1 };

            // Act
            var resultados = Validar(pet);

            // Assert
            Assert.Empty(resultados);
        }

        [Fact]
        public void Validate_PetComNomeVazio_DeveRetornarInvalido()
        {
            // Arrange
            var pet = new Pet { Nome = string.Empty, TutorId = 1 };

            // Act
            var resultados = Validar(pet);

            // Assert
            Assert.Contains(resultados, r => r.MemberNames.Contains(nameof(Pet.Nome)));
        }

        [Fact]
        public void Validate_PetComNomeAcimaDoLimite_DeveRetornarInvalido()
        {
            // Arrange
            var pet = new Pet { Nome = new string('B', 101), TutorId = 1 };

            // Act
            var resultados = Validar(pet);

            // Assert
            Assert.Contains(resultados, r => r.MemberNames.Contains(nameof(Pet.Nome)));
        }
    }
}
