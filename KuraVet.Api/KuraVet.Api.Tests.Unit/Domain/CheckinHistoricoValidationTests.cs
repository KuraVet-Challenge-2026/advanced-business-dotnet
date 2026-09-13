using System.ComponentModel.DataAnnotations;
using KuraVet.Api.Models;
using Xunit;

namespace KuraVet.Api.Tests.Unit.Domain
{
    public class CheckinHistoricoValidationTests
    {
        private static CheckinHistorico CriarCheckinValido() => new()
        {
            DataCheckin = DateTime.Now,
            FrequenciaRespiratoria = 20,
            TempoPreenchimentoCapilar = 1,
            CorMucosa = "Rosada",
            NivelHidratacao = "Normal",
            PetId = 1
        };

        private static IList<ValidationResult> Validar(CheckinHistorico checkin)
        {
            var context = new ValidationContext(checkin);
            var resultados = new List<ValidationResult>();
            Validator.TryValidateObject(checkin, context, resultados, validateAllProperties: true);
            return resultados;
        }

        [Fact]
        public void Validate_CheckinComDadosValidos_DeveRetornarValido()
        {
            // Arrange
            var checkin = CriarCheckinValido();

            // Act
            var resultados = Validar(checkin);

            // Assert
            Assert.Empty(resultados);
        }

        [Fact]
        public void Validate_CheckinComCorMucosaVazia_DeveRetornarInvalido()
        {
            // Arrange
            var checkin = CriarCheckinValido();
            checkin.CorMucosa = string.Empty;

            // Act
            var resultados = Validar(checkin);

            // Assert
            Assert.Contains(resultados, r => r.MemberNames.Contains(nameof(CheckinHistorico.CorMucosa)));
        }

        [Fact]
        public void Validate_CheckinComNivelHidratacaoVazio_DeveRetornarInvalido()
        {
            // Arrange
            var checkin = CriarCheckinValido();
            checkin.NivelHidratacao = string.Empty;

            // Act
            var resultados = Validar(checkin);

            // Assert
            Assert.Contains(resultados, r => r.MemberNames.Contains(nameof(CheckinHistorico.NivelHidratacao)));
        }
    }
}
