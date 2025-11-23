
namespace GustosApp.Application.Tests
{
    using GustosApp.Application.UseCases.UsuarioUseCases.GustoUseCases;
    using GustosApp.Domain.Interfaces;
    using GustosApp.Domain.Model;
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Xunit;

    public class ObtenerGustosUseCaseTests
    {
        private readonly Mock<IUsuarioRepository> _mockUsuarioRepo;
        private readonly ObtenerGustosUseCase _useCase;

        public ObtenerGustosUseCaseTests()
        {
            _mockUsuarioRepo = new Mock<IUsuarioRepository>();
            _useCase = new ObtenerGustosUseCase(_mockUsuarioRepo.Object);
        }

        private Usuario CrearUsuarioPrueba()
        {
            return new Usuario
            {
                Gustos = new List<Gusto>
            {
                new Gusto { Nombre = "Pizza" },
                new Gusto { Nombre = "Sushi" }
            },
                Restricciones = new List<Restriccion>
            {
                new Restriccion { Nombre = "Sin gluten" }
            },
                CondicionesMedicas = new List<CondicionMedica>
            {
                new CondicionMedica { Nombre = "Hipertensión" }
            }
            };
        }

        [Fact]
        public async Task HandleAsync_UsuarioNoExiste_LanzaUnauthorizedAccessException()
        {
            // Arrange
            _mockUsuarioRepo
                .Setup(r => r.GetByFirebaseUidAsync("uid", It.IsAny<CancellationToken>()))
                .ReturnsAsync((Usuario)null);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _useCase.HandleAsync("uid"));
        }

        [Fact]
        public async Task HandleAsync_GustosEsNull_DevuelveDatosDelUsuario()
        {
            // Arrange
            var usuario = CrearUsuarioPrueba();

            _mockUsuarioRepo
                .Setup(r => r.GetByFirebaseUidAsync("uid", It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            // Act
            var result = await _useCase.HandleAsync("uid", CancellationToken.None, null);

            // Assert
            Assert.Equal(new List<string> { "Pizza", "Sushi" }, result.Gustos);
            Assert.Equal(new List<string> { "Sin gluten" }, result.Restricciones);
            Assert.Equal(new List<string> { "Hipertensión" }, result.CondicionesMedicas);
        }

        [Fact]
        public async Task HandleAsync_GustosEsListaVacia_DevuelveDatosDelUsuario()
        {
            // Arrange
            var usuario = CrearUsuarioPrueba();

            _mockUsuarioRepo
                .Setup(r => r.GetByFirebaseUidAsync("uid", It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            // Act
            var result = await _useCase.HandleAsync("uid", CancellationToken.None, new List<string>());

            // Assert
            Assert.Equal(new List<string> { "Pizza", "Sushi" }, result.Gustos);
            Assert.Equal(new List<string> { "Sin gluten" }, result.Restricciones);
            Assert.Equal(new List<string> { "Hipertensión" }, result.CondicionesMedicas);
        }

        [Fact]
        public async Task HandleAsync_GustosEnviados_DevuelveGustosEnviados()
        {
            // Arrange
            var usuario = CrearUsuarioPrueba();
            var gustosEnviados = new List<string> { "Hamburguesa" };

            _mockUsuarioRepo
                .Setup(r => r.GetByFirebaseUidAsync("uid", It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            // Act
            var result = await _useCase.HandleAsync("uid", CancellationToken.None, gustosEnviados);

            // Assert
            Assert.Equal(gustosEnviados, result.Gustos);
            Assert.Equal(new List<string> { "Sin gluten" }, result.Restricciones);
            Assert.Equal(new List<string> { "Hipertensión" }, result.CondicionesMedicas);
        }
    }
}
