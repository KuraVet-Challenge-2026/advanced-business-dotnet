using KuraVet.Api.Controllers;
using KuraVet.Api.Domain;
using KuraVet.Api.Models;
using KuraVet.Api.Tests.Unit.Fixtures;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace KuraVet.Api.Tests.Unit.Application
{
    public class CheckinControllerTests
    {
        private readonly Mock<ICheckinRiscoService> _riscoServiceMock = new();

        private CheckinController CriarController(out KuraVet.Api.Data.KuraVetDbContext context)
        {
            context = InMemoryDbContextFactory.CreateContext();
            var loggerMock = new Mock<ILogger<CheckinController>>();
            return new CheckinController(context, _riscoServiceMock.Object, loggerMock.Object);
        }

        [Fact]
        public async Task Post_CheckinValido_DeveUsarServicoDeRiscoEDelegarClassificacao()
        {
            // Arrange
            _riscoServiceMock
                .Setup(s => s.ClassificarRisco(It.IsAny<int>()))
                .Returns("Moderado");

            var controller = CriarController(out _);
            var model = new CheckinHistorico
            {
                FrequenciaRespiratoria = 22,
                TempoPreenchimentoCapilar = 3,
                CorMucosa = "Pálida",
                NivelHidratacao = "Reduzida",
                PetId = 1
            };

            // Act
            var resultado = await controller.Post(model);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(resultado);
            var checkinCriado = Assert.IsType<CheckinHistorico>(createdResult.Value);
            Assert.Equal("Moderado", checkinCriado.NivelRiscoIA);
            _riscoServiceMock.Verify(s => s.ClassificarRisco(3), Times.Once);
        }

        [Fact]
        public async Task Post_ServicoDeRiscoRetornaBaixo_DeveGravarNivelRiscoBaixo()
        {
            // Arrange
            _riscoServiceMock
                .Setup(s => s.ClassificarRisco(It.IsAny<int>()))
                .Returns("Baixo");

            var controller = CriarController(out var context);
            var model = new CheckinHistorico
            {
                FrequenciaRespiratoria = 18,
                TempoPreenchimentoCapilar = 1,
                CorMucosa = "Rosada",
                NivelHidratacao = "Normal",
                PetId = 1
            };

            // Act
            await controller.Post(model);

            // Assert
            var salvo = Assert.Single(context.CheckinsHistoricos);
            Assert.Equal("Baixo", salvo.NivelRiscoIA);
        }

        [Fact]
        public async Task Get_ListaVazia_DeveRetornarNoContent()
        {
            // Arrange
            var controller = CriarController(out _);

            // Act
            var resultado = await controller.Get();

            // Assert
            Assert.IsType<NoContentResult>(resultado);
        }

        [Fact]
        public async Task GetPorId_CheckinInexistente_DeveRetornarNotFound()
        {
            // Arrange
            var controller = CriarController(out _);

            // Act
            var resultado = await controller.Get(9999);

            // Assert
            Assert.IsType<NotFoundResult>(resultado);
        }

        [Fact]
        public async Task GetPorRisco_NenhumAlertaEncontrado_DeveRetornarNoContent()
        {
            // Arrange
            var controller = CriarController(out _);

            // Act
            var resultado = await controller.GetPorRisco("Grave");

            // Assert
            Assert.IsType<NoContentResult>(resultado);
        }

        [Fact]
        public async Task Put_CheckinExistente_DeveRecalcularRiscoUsandoServico()
        {
            // Arrange
            _riscoServiceMock
                .Setup(s => s.ClassificarRisco(It.IsAny<int>()))
                .Returns("Moderado"); ;

            var controller = CriarController(out var context);
            var checkin = new CheckinHistorico
            {
                FrequenciaRespiratoria = 20,
                TempoPreenchimentoCapilar = 1,
                CorMucosa = "Rosada",
                NivelHidratacao = "Normal",
                PetId = 1
            };
            context.CheckinsHistoricos.Add(checkin);
            await context.SaveChangesAsync();

            var atualizacao = new CheckinHistorico
            {
                FrequenciaRespiratoria = 25,
                TempoPreenchimentoCapilar = 4,
                CorMucosa = "Pálida",
                NivelHidratacao = "Reduzida",
                PetId = 1
            };

            // Act
            var resultado = await controller.Put(checkin.Id, atualizacao);

            // Assert
            Assert.IsType<OkObjectResult>(resultado);
            var atualizado = await context.CheckinsHistoricos.FindAsync(checkin.Id);
            Assert.Equal("Moderado", atualizado!.NivelRiscoIA);
        }

        [Fact]
        public async Task Delete_CheckinInexistente_DeveRetornarNotFound()
        {
            // Arrange
            var controller = CriarController(out _);

            // Act
            var resultado = await controller.Delete(9999);

            // Assert
            Assert.IsType<NotFoundResult>(resultado);
        }
    }
}
