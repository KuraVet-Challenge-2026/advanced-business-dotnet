using KuraVet.Api.Controllers;
using KuraVet.Api.Models;
using KuraVet.Api.Tests.Unit.Fixtures;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace KuraVet.Api.Tests.Unit.Application
{
    public class TutorControllerTests
    {
        private static TutorController CriarController(out KuraVet.Api.Data.KuraVetDbContext context)
        {
            context = InMemoryDbContextFactory.CreateContext();
            var loggerMock = new Mock<ILogger<TutorController>>();
            return new TutorController(context, loggerMock.Object);
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
        public async Task Get_TutoresCadastrados_DeveRetornarOkComLista()
        {
            // Arrange
            var controller = CriarController(out var context);
            context.Tutores.Add(new Tutor { Nome = "Carlos Souza" });
            await context.SaveChangesAsync();

            // Act
            var resultado = await controller.Get();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            var tutores = Assert.IsAssignableFrom<IEnumerable<Tutor>>(okResult.Value);
            Assert.Single(tutores);
        }

        [Fact]
        public async Task GetPorId_TutorExistente_DeveRetornarOk()
        {
            // Arrange
            var controller = CriarController(out var context);
            var tutor = new Tutor { Nome = "Ana Paula" };
            context.Tutores.Add(tutor);
            await context.SaveChangesAsync();

            // Act
            var resultado = await controller.Get(tutor.Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            var tutorRetornado = Assert.IsType<Tutor>(okResult.Value);
            Assert.Equal(tutor.Id, tutorRetornado.Id);
        }

        [Fact]
        public async Task GetPorId_TutorInexistente_DeveRetornarNotFound()
        {
            // Arrange
            var controller = CriarController(out _);

            // Act
            var resultado = await controller.Get(999);

            // Assert
            Assert.IsType<NotFoundResult>(resultado);
        }

        [Fact]
        public async Task Post_TutorValido_DeveRetornarCreatedAtAction()
        {
            // Arrange
            var controller = CriarController(out _);
            var tutor = new Tutor { Nome = "Fernanda Lima" };

            // Act
            var resultado = await controller.Post(tutor);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(resultado);
            Assert.Equal(nameof(TutorController.Get), createdResult.ActionName);
        }

        [Fact]
        public async Task Put_TutorExistente_DeveRetornarOkComDadosAtualizados()
        {
            // Arrange
            var controller = CriarController(out var context);
            var tutor = new Tutor { Nome = "Nome Antigo" };
            context.Tutores.Add(tutor);
            await context.SaveChangesAsync();

            var tutorAtualizado = new Tutor { Nome = "Nome Novo" };

            // Act
            var resultado = await controller.Put(tutor.Id, tutorAtualizado);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            var retornado = Assert.IsType<Tutor>(okResult.Value);
            Assert.Equal("Nome Novo", retornado.Nome);
        }

        [Fact]
        public async Task Put_TutorInexistente_DeveRetornarNotFound()
        {
            // Arrange
            var controller = CriarController(out _);
            var tutorAtualizado = new Tutor { Nome = "Não Importa" };

            // Act
            var resultado = await controller.Put(999, tutorAtualizado);

            // Assert
            Assert.IsType<NotFoundResult>(resultado);
        }

        [Fact]
        public async Task Delete_TutorExistente_DeveRetornarOkComTutorRemovido()
        {
            // Arrange
            var controller = CriarController(out var context);
            var tutor = new Tutor { Nome = "Roberto Alves" };
            context.Tutores.Add(tutor);
            await context.SaveChangesAsync();

            // Act
            var resultado = await controller.Delete(tutor.Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            var removido = Assert.IsType<Tutor>(okResult.Value);
            Assert.Equal(tutor.Id, removido.Id);
            Assert.Empty(context.Tutores);
        }

        [Fact]
        public async Task Delete_TutorInexistente_DeveRetornarNotFound()
        {
            // Arrange
            var controller = CriarController(out _);

            // Act
            var resultado = await controller.Delete(999);

            // Assert
            Assert.IsType<NotFoundResult>(resultado);
        }
    }
}
