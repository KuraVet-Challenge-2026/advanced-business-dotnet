using KuraVet.Api.Controllers;
using KuraVet.Api.Models;
using KuraVet.Api.Tests.Unit.Fixtures;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace KuraVet.Api.Tests.Unit.Application
{
    public class EventoConsultaControllerTests
    {
        private static EventoConsultaController CriarController(out KuraVet.Api.Data.KuraVetDbContext context)
        {
            context = InMemoryDbContextFactory.CreateContext();
            var loggerMock = new Mock<ILogger<EventoConsultaController>>();
            return new EventoConsultaController(context, loggerMock.Object);
        }

        [Fact]
        public async Task Post_EventoValido_DeveRetornarCreatedAtAction()
        {
            // Arrange
            var controller = CriarController(out _);
            var evento = new EventoConsulta
            {
                TipoEvento = "Vacina",
                DataEvento = DateTime.Now,
                Descricao = "Vacina V10",
                VeterinarioResponsavel = "Dra. Camila",
                PetId = 1
            };

            // Act
            var resultado = await controller.Post(evento);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(resultado);
            Assert.Equal(nameof(EventoConsultaController.Get), createdResult.ActionName);
        }

        [Fact]
        public async Task GetPorPet_PetComEventos_DeveRetornarOkOrdenadoPorDataDecrescente()
        {
            // Arrange
            var controller = CriarController(out var context);
            var maisAntigo = new EventoConsulta
            {
                TipoEvento = "Consulta",
                DataEvento = DateTime.Now.AddDays(-10),
                VeterinarioResponsavel = "Dr. Pedro",
                PetId = 5
            };
            var maisRecente = new EventoConsulta
            {
                TipoEvento = "Exame",
                DataEvento = DateTime.Now,
                VeterinarioResponsavel = "Dr. Pedro",
                PetId = 5
            };
            context.EventosConsultas.AddRange(maisAntigo, maisRecente);
            await context.SaveChangesAsync();

            // Act
            var resultado = await controller.GetPorPet(5);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            var eventos = Assert.IsAssignableFrom<IEnumerable<EventoConsulta>>(okResult.Value).ToList();
            Assert.Equal(2, eventos.Count);
            Assert.Equal("Exame", eventos.First().TipoEvento);
        }

        [Fact]
        public async Task GetPorPet_PetSemEventos_DeveRetornarNoContent()
        {
            // Arrange
            var controller = CriarController(out _);

            // Act
            var resultado = await controller.GetPorPet(9999);

            // Assert
            Assert.IsType<NoContentResult>(resultado);
        }

        [Fact]
        public async Task Put_EventoInexistente_DeveRetornarNotFound()
        {
            // Arrange
            var controller = CriarController(out _);
            var atualizacao = new EventoConsulta
            {
                TipoEvento = "Retorno",
                DataEvento = DateTime.Now,
                VeterinarioResponsavel = "Dr. Pedro",
                PetId = 1
            };

            // Act
            var resultado = await controller.Put(9999, atualizacao);

            // Assert
            Assert.IsType<NotFoundResult>(resultado);
        }

        [Fact]
        public async Task Delete_EventoExistente_DeveRetornarOkComEventoRemovido()
        {
            // Arrange
            var controller = CriarController(out var context);
            var evento = new EventoConsulta
            {
                TipoEvento = "Cirurgia",
                DataEvento = DateTime.Now,
                VeterinarioResponsavel = "Dra. Beatriz",
                PetId = 2
            };
            context.EventosConsultas.Add(evento);
            await context.SaveChangesAsync();

            // Act
            var resultado = await controller.Delete(evento.Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            var removido = Assert.IsType<EventoConsulta>(okResult.Value);
            Assert.Equal(evento.Id, removido.Id);
            Assert.Empty(context.EventosConsultas);
        }
    }
}
