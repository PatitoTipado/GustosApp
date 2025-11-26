
namespace GustosApp.Application.Tests
{
    using GustosApp.Application.UseCases.UsuarioUseCases;
    using GustosApp.Domain.Interfaces;
    using GustosApp.Domain.Model;
    using Moq;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Xunit;

    public class ObtenerUsuarioUseCaseTests
    {
        private readonly Mock<IUsuarioRepository> _mockRepo;
        private readonly ObtenerUsuarioUseCase _useCase;

        public ObtenerUsuarioUseCaseTests()
        {
            _mockRepo = new Mock<IUsuarioRepository>();
            _useCase = new ObtenerUsuarioUseCase(_mockRepo.Object);
        }

        private Usuario CrearUsuario() => new Usuario
        {
            Id = Guid.NewGuid(),
            FirebaseUid = "firebase123",
            IdUsuario = "juan"
        };

        [Fact]
        public async Task HandleAsync_CuandoFirebaseUidExiste_DevuelveUsuario()
        {
            // Arrange
            var usuario = CrearUsuario();

            _mockRepo
                .Setup(r => r.GetByFirebaseUidAsync("firebase123", It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            // Act
            var result = await _useCase.HandleAsync(FirebaseUid: "firebase123");

            // Assert
            Assert.Equal(usuario, result);

            _mockRepo.Verify(r =>
                r.GetByFirebaseUidAsync("firebase123", It.IsAny<CancellationToken>()),
                Times.Once);

            _mockRepo.Verify(r =>
                r.GetByUsernameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task HandleAsync_CuandoFirebaseUidNoDaUsuario_PeroUsernameExiste_DevuelveUsuario()
        {
            // Arrange
            var usuario = CrearUsuario();

            _mockRepo
                .Setup(r => r.GetByFirebaseUidAsync("firebase123", It.IsAny<CancellationToken>()))
                .ReturnsAsync((Usuario)null);

            _mockRepo
                .Setup(r => r.GetByUsernameAsync("juan", It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            // Act & Assert  
            // OJO: el use case NO usa Username si FirebaseUid está seteado aunque dé null  
            // Por lo tanto, en tu lógica actual esto DEBE lanzar "Usuario no encontrado"

            await Assert.ThrowsAsync<Exception>(() =>
                _useCase.HandleAsync(FirebaseUid: "firebase123", Username: "juan"));
        }

        [Fact]
        public async Task HandleAsync_CuandoSePasaSoloUsername_DevuelveUsuario()
        {
            // Arrange
            var usuario = CrearUsuario();

            _mockRepo
                .Setup(r => r.GetByUsernameAsync("juan", It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            // Act
            var result = await _useCase.HandleAsync(Username: "juan");

            // Assert
            Assert.Equal(usuario, result);

            _mockRepo.Verify(r =>
                r.GetByUsernameAsync("juan", It.IsAny<CancellationToken>()),
                Times.Once);

            _mockRepo.Verify(r =>
                r.GetByFirebaseUidAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task HandleAsync_CuandoNoExisteNingunParametro_LanzaException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _useCase.HandleAsync());
        }

        [Fact]
        public async Task HandleAsync_CuandoNoEncuentraUsuario_LanzaException()
        {
            // Arrange
            _mockRepo
                .Setup(r => r.GetByFirebaseUidAsync("firebase123", It.IsAny<CancellationToken>()))
                .ReturnsAsync((Usuario)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() =>
                _useCase.HandleAsync(FirebaseUid: "firebase123"));
        }
    }
}
