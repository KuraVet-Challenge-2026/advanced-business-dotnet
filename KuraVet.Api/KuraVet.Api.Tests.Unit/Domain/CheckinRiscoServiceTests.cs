using KuraVet.Api.Domain;
using Xunit;

namespace KuraVet.Api.Tests.Unit.Domain
{
    public class CheckinRiscoServiceTests
    {
        private readonly ICheckinRiscoService _sut = new CheckinRiscoService();

        [Fact]
        public void ClassificarRisco_TpcMenorQueDois_DeveRetornarBaixo()
        {
            // Arrange
            var tpc = 1;

            // Act
            var resultado = _sut.ClassificarRisco(tpc);

            // Assert
            Assert.Equal("Baixo", resultado);
        }

        [Fact]
        public void ClassificarRisco_TpcIgualADois_DeveRetornarBaixo()
        {
            // Arrange
            var tpc = 2;

            // Act
            var resultado = _sut.ClassificarRisco(tpc);

            // Assert
            Assert.Equal("Baixo", resultado);
        }

        [Fact]
        public void ClassificarRisco_TpcMaiorQueDois_DeveRetornarModerado()
        {
            // Arrange
            var tpc = 3;

            // Act
            var resultado = _sut.ClassificarRisco(tpc);

            // Assert
            Assert.Equal("Moderado", resultado);
        }

        [Theory]
        [InlineData(0, "Baixo")]
        [InlineData(1, "Baixo")]
        [InlineData(2, "Baixo")]
        [InlineData(3, "Moderado")]
        [InlineData(10, "Moderado")]
        public void ClassificarRisco_DiversosValoresDeTpc_DeveRetornarNivelEsperado(int tpc, string nivelEsperado)
        {

            // Act
            var resultado = _sut.ClassificarRisco(tpc);

            // Assert
            Assert.Equal(nivelEsperado, resultado);
        }
    }
}
