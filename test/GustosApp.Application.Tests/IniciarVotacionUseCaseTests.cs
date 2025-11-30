using System;
using System.Threading;
using System.Threading.Tasks;
using GustosApp.Application.UseCases.VotacionUseCases;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using Moq;
using Xunit;

namespace GustosApp.Application.Tests
{
    public class IniciarVotacionUseCaseTests
    {
        private readonly Mock<IVotacionRepository> _mockVotacionRepository;
        private readonly Mock<IGrupoRepository> _mockGrupoRepository;
        private readonly Mock<IUsuarioRepository> _mockUsuarioRepository;
        private readonly IniciarVotacionUseCase _useCase;

        public IniciarVotacionUseCaseTests()
        {
            _mockVotacionRepository = new Mock<IVotacionRepository>();
            _mockGrupoRepository = new Mock<IGrupoRepository>();
            _mockUsuarioRepository = new Mock<IUsuarioRepository>();

            _useCase = new IniciarVotacionUseCase(
                _mockVotacionRepository.Object,
                _mockGrupoRepository.Object,
                _mockUsuarioRepository.Object);
        }

        [Fact]
        public async Task HandleAsync_UsuarioNoEncontrado_LanzaUnauthorizedAccessException()
        {
            // Arrange
            var firebaseUid = "firebase123";
            var grupoId = Guid.NewGuid();

            _mockUsuarioRepository
                .Setup(x => x.GetByFirebaseUidAsync(firebaseUid, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Usuario?)null);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _useCase.HandleAsync(firebaseUid, grupoId));
        }

        [Fact]
        public async Task HandleAsync_UsuarioNoEsMiembro_LanzaUnauthorizedAccessException()
        {
            // Arrange
            var firebaseUid = "firebase123";
            var grupoId = Guid.NewGuid();
            var usuario = new Usuario
            {
                Id = Guid.NewGuid(),
                FirebaseUid = "testFirebaseUid",
                Email = "test@test.com",
                Nombre = "Test",
                Apellido = "User"
            };

            _mockUsuarioRepository
                .Setup(x => x.GetByFirebaseUidAsync(firebaseUid, It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            _mockGrupoRepository
                .Setup(x => x.UsuarioEsMiembroAsync(grupoId, firebaseUid, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _useCase.HandleAsync(firebaseUid, grupoId));
        }

        [Fact]
        public async Task HandleAsync_VotacionActivaExistente_LanzaInvalidOperationException()
        {
            // Arrange
            var firebaseUid = "firebase123";
            var grupoId = Guid.NewGuid();
            var usuario = new Usuario
            {
                Id = Guid.NewGuid(),
                FirebaseUid = "testFirebaseUid",
                Email = "test@test.com",
                Nombre = "Test",
                Apellido = "User"
            };
            var votacionActiva = new VotacionGrupo(grupoId);

            _mockUsuarioRepository
                .Setup(x => x.GetByFirebaseUidAsync(firebaseUid, It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            _mockGrupoRepository
                .Setup(x => x.UsuarioEsMiembroAsync(grupoId, firebaseUid, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _mockVotacionRepository
                .Setup(x => x.ObtenerVotacionActivaAsync(grupoId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(votacionActiva);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _useCase.HandleAsync(firebaseUid, grupoId));

            Assert.Equal("Ya existe una votación activa en este grupo", exception.Message);
        }

        [Fact]
        public async Task HandleAsync_TodoValido_CreaVotacion()
        {
            // Arrange
            var firebaseUid = "firebase123";
            var grupoId = Guid.NewGuid();
            var descripcion = "Votación del viernes";
            var usuario = new Usuario
            {
                Id = Guid.NewGuid(),
                FirebaseUid = "testFirebaseUid",
                Email = "test@test.com",
                Nombre = "Test",
                Apellido = "User"
            };
            var nuevaVotacion = new VotacionGrupo(grupoId, descripcion);

            _mockUsuarioRepository
                .Setup(x => x.GetByFirebaseUidAsync(firebaseUid, It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            _mockGrupoRepository
                .Setup(x => x.UsuarioEsMiembroAsync(grupoId, firebaseUid, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _mockVotacionRepository
                .Setup(x => x.ObtenerVotacionActivaAsync(grupoId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((VotacionGrupo?)null);

            _mockVotacionRepository
                .Setup(x => x.CrearVotacionAsync(It.IsAny<VotacionGrupo>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(nuevaVotacion);

            // Act
            var resultado = await _useCase.HandleAsync(firebaseUid, grupoId, descripcion);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(grupoId, resultado.GrupoId);
            Assert.Equal(EstadoVotacion.Activa, resultado.Estado);
            _mockVotacionRepository.Verify(
                x => x.CrearVotacionAsync(It.IsAny<VotacionGrupo>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task HandleAsync_SinDescripcion_CreaVotacionSinDescripcion()
        {
            // Arrange
            var firebaseUid = "firebase123";
            var grupoId = Guid.NewGuid();
            var usuario = new Usuario
            {
                Id = Guid.NewGuid(),
                FirebaseUid = "testFirebaseUid",
                Email = "test@test.com",
                Nombre = "Test",
                Apellido = "User"
            };
            var nuevaVotacion = new VotacionGrupo(grupoId);

            _mockUsuarioRepository
                .Setup(x => x.GetByFirebaseUidAsync(firebaseUid, It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            _mockGrupoRepository
                .Setup(x => x.UsuarioEsMiembroAsync(grupoId, firebaseUid, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _mockVotacionRepository
                .Setup(x => x.ObtenerVotacionActivaAsync(grupoId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((VotacionGrupo?)null);

            _mockVotacionRepository
                .Setup(x => x.CrearVotacionAsync(It.IsAny<VotacionGrupo>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(nuevaVotacion);

            // Act
            var resultado = await _useCase.HandleAsync(firebaseUid, grupoId);

            // Assert
            Assert.NotNull(resultado);
            _mockVotacionRepository.Verify(
                x => x.CrearVotacionAsync(It.IsAny<VotacionGrupo>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
