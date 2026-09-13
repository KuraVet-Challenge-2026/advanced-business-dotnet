using System.ComponentModel.DataAnnotations;
using KuraVet.Api.Models;
using Xunit;

namespace KuraVet.Api.Tests.Unit.Domain
{
    public class EventoConsultaValidationTests
    {
        private static EventoConsulta CriarEventoValido() => new()
        {
            TipoEvento = "Vacina",
            DataEvento = DateTime.Now,
            Descricao = "Vacina antirrábica anual",
            VeterinarioResponsavel = "Dr. João",
            PetId = 1
        };

        private static IList<ValidationResult> Validar(EventoConsulta evento)
        {
            var context = new ValidationContext(evento);
            var resultados = new List<ValidationResult>();
            Validator.TryValidateObject(evento, context, resultados, validateAllProperties: true);
            return resultados;
        }

        [Fact]
        public void Validate_EventoComDadosValidos_DeveRetornarValido()
        {
            // Arrange
            var evento = CriarEventoValido();

            // Act
            var resultados = Validar(evento);

            // Assert
            Assert.Empty(resultados);
        }

        [Fact]
        public void Validate_EventoComTipoEventoVazio_DeveRetornarInvalido()
        {
            // Arrange
            var evento = CriarEventoValido();
            evento.TipoEvento = string.Empty;

            // Act
            var resultados = Validar(evento);

            // Assert
            Assert.Contains(resultados, r => r.MemberNames.Contains(nameof(EventoConsulta.TipoEvento)));
        }

        [Fact]
        public void Validate_EventoComVeterinarioResponsavelVazio_DeveRetornarInvalido()
        {
            // Arrange
            var evento = CriarEventoValido();
            evento.VeterinarioResponsavel = string.Empty;

            // Act
            var resultados = Validar(evento);

            // Assert
            Assert.Contains(resultados, r => r.MemberNames.Contains(nameof(EventoConsulta.VeterinarioResponsavel)));
        }
    }
}
