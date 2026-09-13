using KuraVet.Api.Controllers;
using KuraVet.Api.Models;
using KuraVet.Api.Tests.Unit.Fixtures;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace KuraVet.Api.Tests.Unit.Application
{
    public class PetControllerTests : IClassFixture<SeedDataFixture>
    {
        private readonly SeedDataFixture _fixture;

        public PetControllerTests(SeedDataFixture fixture)
        {
            _fixture = fixture;
        }

        private PetController CriarController(out KuraVet.Api.Data.KuraVetDbContext context)
        {
            context = _fixture.CreateContext();
            var loggerMock = new Mock<ILogger<PetController>>();
            return new PetController(context, loggerMock.Object);
        }

        [Fact]
        public async Task GetPorTutor_TutorComPets_DeveRetornarOkComListaDePets()
        {
            // Arrange
            var controller = CriarController(out _);

            // Act
            var resultado = await controller.GetPorTutor(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            var pets = Assert.IsAssignableFrom<IEnumerable<Pet>>(okResult.Value);
            Assert.Single(pets);
        }

        [Fact]
        public async Task GetPorTutor_TutorSemPets_DeveRetornarNoContent()
        {
            // Arrange
            var controller = CriarController(out _);

            // Act
            var resultado = await controller.GetPorTutor(9999);

            // Assert
            Assert.IsType<NoContentResult>(resultado);
        }

        [Fact]
        public async Task GetPorId_PetExistente_DeveRetornarOk()
        {
            // Arrange
            var controller = CriarController(out _);

            // Act
            var resultado = await controller.Get(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            var pet = Assert.IsType<Pet>(okResult.Value);
            Assert.Equal(1, pet.Id);
        }

        [Fact]
        public async Task GetPorId_PetInexistente_DeveRetornarNotFound()
        {
            // Arrange
            var controller = CriarController(out _);

            // Act
            var resultado = await controller.Get(9999);

            // Assert
            Assert.IsType<NotFoundResult>(resultado);
        }

        [Fact]
        public async Task Post_PetValido_DeveRetornarCreatedAtAction()
        {
            // Arrange
            var controller = CriarController(out _);
            var novoPet = new Pet { Nome = "Bidu", TutorId = 1 };

            // Act
            var resultado = await controller.Post(novoPet);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(resultado);
            Assert.Equal(nameof(PetController.Get), createdResult.ActionName);
        }

        [Fact]
        public async Task Delete_PetInexistente_DeveRetornarNotFound()
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
