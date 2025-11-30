using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GustosApp.Application.UseCases.VotacionUseCases;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using Moq;
using Xunit;

namespace GustosApp.Application.Tests
{
    public class CerrarVotacionUseCaseTests
    {
        private readonly Mock<IVotacionRepository> _mockVotacionRepository;
        private readonly Mock<IUsuarioRepository> _mockUsuarioRepository;
        private readonly Mock<IGrupoRepository> _mockGrupoRepository;
        private readonly CerrarVotacionUseCase _useCase;

        public CerrarVotacionUseCaseTests()
        {
            _mockVotacionRepository = new Mock<IVotacionRepository>();
            _mockUsuarioRepository = new Mock<IUsuarioRepository>();
            _mockGrupoRepository = new Mock<IGrupoRepository>();

            _useCase = new CerrarVotacionUseCase(
                _mockVotacionRepository.Object,
                _mockUsuarioRepository.Object,
                _mockGrupoRepository.Object);
        }

        [Fact]
        public async Task HandleAsync_UsuarioNoEncontrado_LanzaUnauthorizedAccessException()
        {
            // Arrange
            var firebaseUid = "firebase123";
            var votacionId = Guid.NewGuid();

            _mockUsuarioRepository
                .Setup(x => x.GetByFirebaseUidAsync(firebaseUid, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Usuario?)null);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _useCase.HandleAsync(firebaseUid, votacionId));
        }

        [Fact]
        public async Task HandleAsync_VotacionNoEncontrada_LanzaArgumentException()
        {
            // Arrange
            var firebaseUid = "firebase123";
            var votacionId = Guid.NewGuid();
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

            _mockVotacionRepository
                .Setup(x => x.ObtenerPorIdAsync(votacionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((VotacionGrupo?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _useCase.HandleAsync(firebaseUid, votacionId));

            Assert.Equal("Votación no encontrada", exception.Message);
        }

        [Fact]
        public async Task HandleAsync_UsuarioNoEsAdministrador_LanzaUnauthorizedAccessException()
        {
            // Arrange
            var firebaseUid = "firebase123";
            var votacionId = Guid.NewGuid();
            var grupoId = Guid.NewGuid();
            var adminId = Guid.NewGuid();
            var usuario = new Usuario
            {
                Id = Guid.NewGuid(),
                FirebaseUid = "testFirebaseUid",
                Email = "test@test.com",
                Nombre = "Test",
                Apellido = "User"
            };
            var grupo = new Grupo("Test Grupo", adminId) { Id = grupoId };
            var votacion = new VotacionGrupo(grupoId) { Grupo = grupo };

            _mockUsuarioRepository
                .Setup(x => x.GetByFirebaseUidAsync(firebaseUid, It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            _mockVotacionRepository
                .Setup(x => x.ObtenerPorIdAsync(votacionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(votacion);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _useCase.HandleAsync(firebaseUid, votacionId));

            Assert.Equal("Solo el administrador puede cerrar la votación", exception.Message);
        }

        [Fact]
        public async Task HandleAsync_VotacionYaCerrada_LanzaInvalidOperationException()
        {
            // Arrange
            var firebaseUid = "firebase123";
            var votacionId = Guid.NewGuid();
            var grupoId = Guid.NewGuid();
            var usuario = new Usuario
            {
                Id = Guid.NewGuid(),
                FirebaseUid = "testFirebaseUid",
                Email = "test@test.com",
                Nombre = "Test",
                Apellido = "User"
            };
            var grupo = new Grupo("Test Grupo", usuario.Id) { Id = grupoId };
            var votacion = new VotacionGrupo(grupoId) { Grupo = grupo };
            votacion.CerrarVotacion();

            _mockUsuarioRepository
                .Setup(x => x.GetByFirebaseUidAsync(firebaseUid, It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            _mockVotacionRepository
                .Setup(x => x.ObtenerPorIdAsync(votacionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(votacion);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _useCase.HandleAsync(firebaseUid, votacionId));

            Assert.Equal("Solo se pueden cerrar votaciones activas", exception.Message);
        }

        [Fact]
        public async Task HandleAsync_TodosVotaronConGanadorClaro_CierraYEstableceGanador()
        {
            // Arrange
            var firebaseUid = "firebase123";
            var votacionId = Guid.NewGuid();
            var grupoId = Guid.NewGuid();
            var restaurante1Id = Guid.NewGuid();
            var restaurante2Id = Guid.NewGuid();
            var usuario1Id = Guid.NewGuid();
            var usuario2Id = Guid.NewGuid();
            var usuario3Id = Guid.NewGuid();
            
            var admin = new Usuario
            {
                Id = usuario1Id,
                FirebaseUid = "firebase123",
                Email = "admin@test.com",
                Nombre = "Admin",
                Apellido = "User"
            };
            var usuario2 = new Usuario
            {
                Id = usuario2Id,
                FirebaseUid = "user2FirebaseUid",
                Email = "user2@test.com",
                Nombre = "User",
                Apellido = "Two"
            };
            var usuario3 = new Usuario
            {
                Id = usuario3Id,
                FirebaseUid = "user3FirebaseUid",
                Email = "user3@test.com",
                Nombre = "User",
                Apellido = "Three"
            };
            
            var grupo = new Grupo("Test Grupo", usuario1Id) { Id = grupoId };
            var miembro1 = new MiembroGrupo(grupoId, usuario1Id, true);
            var miembro2 = new MiembroGrupo(grupoId, usuario2Id, false);
            var miembro3 = new MiembroGrupo(grupoId, usuario3Id, false);
            grupo.Miembros.Add(miembro1);
            grupo.Miembros.Add(miembro2);
            grupo.Miembros.Add(miembro3);
            
            var votacion = new VotacionGrupo(grupoId) { Grupo = grupo };
            
            // Todos votan, restaurante1 gana con 2 votos
            votacion.Votos.Add(new VotoRestaurante(votacion.Id, usuario1Id, restaurante1Id, null));
            votacion.Votos.Add(new VotoRestaurante(votacion.Id, usuario2Id, restaurante1Id, null));
            votacion.Votos.Add(new VotoRestaurante(votacion.Id, usuario3Id, restaurante2Id, null));

            _mockUsuarioRepository
                .Setup(x => x.GetByFirebaseUidAsync(firebaseUid, It.IsAny<CancellationToken>()))
                .ReturnsAsync(admin);

            _mockVotacionRepository
                .Setup(x => x.ObtenerPorIdAsync(votacionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(votacion);

            _mockVotacionRepository
                .Setup(x => x.ActualizarVotacionAsync(It.IsAny<VotacionGrupo>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var resultado = await _useCase.HandleAsync(firebaseUid, votacionId);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(EstadoVotacion.Cerrada, resultado.Estado);
            Assert.Equal(restaurante1Id, resultado.RestauranteGanadorId);
            var restaurantesEmpatados = resultado.ObtenerRestaurantesEmpatados();
            Assert.Single(restaurantesEmpatados); // No hay empate
            _mockVotacionRepository.Verify(
                x => x.ActualizarVotacionAsync(It.IsAny<VotacionGrupo>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task HandleAsync_EmpateEnVotacion_CierraSinEstablecerGanadorAutomatico()
        {
            // Arrange
            var firebaseUid = "firebase123";
            var votacionId = Guid.NewGuid();
            var grupoId = Guid.NewGuid();
            var restaurante1Id = Guid.NewGuid();
            var restaurante2Id = Guid.NewGuid();
            var usuario1Id = Guid.NewGuid();
            var usuario2Id = Guid.NewGuid();
            
            var admin = new Usuario
            {
                Id = usuario1Id,
                FirebaseUid = "firebase123",
                Email = "admin@test.com",
                Nombre = "Admin",
                Apellido = "User"
            };
            var usuario2 = new Usuario
            {
                Id = usuario2Id,
                FirebaseUid = "user2FirebaseUid",
                Email = "user2@test.com",
                Nombre = "User",
                Apellido = "Two"
            };
            
            var grupo = new Grupo("Test Grupo", usuario1Id) { Id = grupoId };
            var miembro1 = new MiembroGrupo(grupoId, usuario1Id, true);
            var miembro2 = new MiembroGrupo(grupoId, usuario2Id, false);
            grupo.Miembros.Add(miembro1);
            grupo.Miembros.Add(miembro2);
            
            var votacion = new VotacionGrupo(grupoId) { Grupo = grupo };
            
            // Empate: cada uno vota por un restaurante diferente
            votacion.Votos.Add(new VotoRestaurante(votacion.Id, usuario1Id, restaurante1Id, null));
            votacion.Votos.Add(new VotoRestaurante(votacion.Id, usuario2Id, restaurante2Id, null));

            _mockUsuarioRepository
                .Setup(x => x.GetByFirebaseUidAsync(firebaseUid, It.IsAny<CancellationToken>()))
                .ReturnsAsync(admin);

            _mockVotacionRepository
                .Setup(x => x.ObtenerPorIdAsync(votacionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(votacion);

            _mockVotacionRepository
                .Setup(x => x.ActualizarVotacionAsync(It.IsAny<VotacionGrupo>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var resultado = await _useCase.HandleAsync(firebaseUid, votacionId);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(EstadoVotacion.Cerrada, resultado.Estado);
            Assert.Null(resultado.RestauranteGanadorId);
            var restaurantesEmpatados = resultado.ObtenerRestaurantesEmpatados();
            Assert.Equal(2, restaurantesEmpatados.Count); // Hay empate
            _mockVotacionRepository.Verify(
                x => x.ActualizarVotacionAsync(It.IsAny<VotacionGrupo>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task HandleAsync_CierreExitoso_RetornaResultadoCompleto()
        {
            // Arrange
            var firebaseUid = "firebase123";
            var votacionId = Guid.NewGuid();
            var grupoId = Guid.NewGuid();
            var restaurante1Id = Guid.NewGuid();
            var usuario1Id = Guid.NewGuid();
            
            var admin = new Usuario
            {
                Id = usuario1Id,
                FirebaseUid = "firebase123",
                Email = "admin@test.com",
                Nombre = "Admin",
                Apellido = "User"
            };
            
            var grupo = new Grupo("Test Grupo", usuario1Id) { Id = grupoId };
            var miembro = new MiembroGrupo(grupoId, usuario1Id, true);
            grupo.Miembros.Add(miembro);
            
            var votacion = new VotacionGrupo(grupoId) { Grupo = grupo };
            votacion.Votos.Add(new VotoRestaurante(votacion.Id, usuario1Id, restaurante1Id, null));

            _mockUsuarioRepository
                .Setup(x => x.GetByFirebaseUidAsync(firebaseUid, It.IsAny<CancellationToken>()))
                .ReturnsAsync(admin);

            _mockVotacionRepository
                .Setup(x => x.ObtenerPorIdAsync(votacionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(votacion);

            _mockVotacionRepository
                .Setup(x => x.ActualizarVotacionAsync(It.IsAny<VotacionGrupo>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var resultado = await _useCase.HandleAsync(firebaseUid, votacionId);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(votacion.Id, resultado.Id);
            Assert.Equal(grupoId, resultado.GrupoId);
            Assert.Equal(EstadoVotacion.Cerrada, resultado.Estado);
            Assert.Equal(restaurante1Id, resultado.RestauranteGanadorId);
            var restaurantesEmpatados = resultado.ObtenerRestaurantesEmpatados();
            Assert.Single(restaurantesEmpatados); // Un solo ganador
            Assert.Equal(restaurante1Id, restaurantesEmpatados.First());
        }

        [Fact]
        public async Task HandleAsync_SinVotos_CierraSinGanador()
        {
            // Arrange
            var firebaseUid = "firebase123";
            var votacionId = Guid.NewGuid();
            var grupoId = Guid.NewGuid();
            var adminId = Guid.NewGuid();
            
            var admin = new Usuario
            {
                Id = adminId,
                FirebaseUid = "firebase123",
                Email = "admin@test.com",
                Nombre = "Admin",
                Apellido = "User"
            };
            
            var grupo = new Grupo("Test Grupo", adminId) { Id = grupoId };
            var votacion = new VotacionGrupo(grupoId) { Grupo = grupo };

            _mockUsuarioRepository
                .Setup(x => x.GetByFirebaseUidAsync(firebaseUid, It.IsAny<CancellationToken>()))
                .ReturnsAsync(admin);

            _mockVotacionRepository
                .Setup(x => x.ObtenerPorIdAsync(votacionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(votacion);

            _mockVotacionRepository
                .Setup(x => x.ActualizarVotacionAsync(It.IsAny<VotacionGrupo>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var resultado = await _useCase.HandleAsync(firebaseUid, votacionId);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(EstadoVotacion.Cerrada, resultado.Estado);
            Assert.Null(resultado.RestauranteGanadorId);
            _mockVotacionRepository.Verify(
                x => x.ActualizarVotacionAsync(votacion, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task HandleAsync_ConGanadorManual_EstableceGanadorEspecificado()
        {
            // Arrange
            var firebaseUid = "firebase123";
            var votacionId = Guid.NewGuid();
            var grupoId = Guid.NewGuid();
            var adminId = Guid.NewGuid();
            var restauranteManualId = Guid.NewGuid();
            
            var admin = new Usuario
            {
                Id = adminId,
                FirebaseUid = "firebase123",
                Email = "admin@test.com",
                Nombre = "Admin",
                Apellido = "User"
            };
            
            var grupo = new Grupo("Test Grupo", adminId) { Id = grupoId };
            var votacion = new VotacionGrupo(grupoId) { Grupo = grupo };

            _mockUsuarioRepository
                .Setup(x => x.GetByFirebaseUidAsync(firebaseUid, It.IsAny<CancellationToken>()))
                .ReturnsAsync(admin);

            _mockVotacionRepository
                .Setup(x => x.ObtenerPorIdAsync(votacionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(votacion);

            _mockVotacionRepository
                .Setup(x => x.ActualizarVotacionAsync(It.IsAny<VotacionGrupo>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var resultado = await _useCase.HandleAsync(firebaseUid, votacionId, restauranteManualId);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(EstadoVotacion.Cerrada, resultado.Estado);
            Assert.Equal(restauranteManualId, resultado.RestauranteGanadorId);
            _mockVotacionRepository.Verify(
                x => x.ActualizarVotacionAsync(votacion, It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
