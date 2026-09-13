using System.ComponentModel.DataAnnotations;
using KuraVet.Api.Models;
using Xunit;

namespace KuraVet.Api.Tests.Unit.Domain
{
    public class TutorValidationTests
    {
        private static IList<ValidationResult> Validar(Tutor tutor)
        {
            var context = new ValidationContext(tutor);
            var resultados = new List<ValidationResult>();
            Validator.TryValidateObject(tutor, context, resultados, validateAllProperties: true);
            return resultados;
        }

        [Fact]
        public void Validate_TutorComNomeValido_DeveRetornarValido()
        {
            // Arrange
            var tutor = new Tutor { Nome = "Maria Oliveira" };

            // Act
            var resultados = Validar(tutor);

            // Assert
            Assert.Empty(resultados);
        }

        [Fact]
        public void Validate_TutorComNomeVazio_DeveRetornarInvalido()
        {
            // Arrange
            var tutor = new Tutor { Nome = string.Empty };

            // Act
            var resultados = Validar(tutor);

            // Assert
            Assert.Contains(resultados, r => r.MemberNames.Contains(nameof(Tutor.Nome)));
        }

        [Fact]
        public void Validate_TutorComNomeAcimaDoLimite_DeveRetornarInvalido()
        {
            // Arrange
            var tutor = new Tutor { Nome = new string('A', 101) };

            // Act
            var resultados = Validar(tutor);

            // Assert
            Assert.Contains(resultados, r => r.MemberNames.Contains(nameof(Tutor.Nome)));
        }
    }
}
