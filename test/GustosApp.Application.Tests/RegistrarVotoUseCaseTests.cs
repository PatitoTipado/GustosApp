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
    public class RegistrarVotoUseCaseTests
    {
        private readonly Mock<IVotacionRepository> _mockVotacionRepository;
        private readonly Mock<IUsuarioRepository> _mockUsuarioRepository;
        private readonly Mock<IGrupoRepository> _mockGrupoRepository;
        private readonly Mock<IRestauranteRepository> _mockRestauranteRepository;
        private readonly RegistrarVotoUseCase _useCase;

        public RegistrarVotoUseCaseTests()
        {
            _mockVotacionRepository = new Mock<IVotacionRepository>();
            _mockUsuarioRepository = new Mock<IUsuarioRepository>();
            _mockGrupoRepository = new Mock<IGrupoRepository>();
            _mockRestauranteRepository = new Mock<IRestauranteRepository>();

            _useCase = new RegistrarVotoUseCase(
                _mockVotacionRepository.Object,
                _mockUsuarioRepository.Object,
                _mockGrupoRepository.Object,
                _mockRestauranteRepository.Object);
        }

        [Fact]
        public async Task HandleAsync_UsuarioNoEncontrado_LanzaUnauthorizedAccessException()
        {
            // Arrange
            var firebaseUid = "firebase123";
            var votacionId = Guid.NewGuid();
            var restauranteId = Guid.NewGuid();

            _mockUsuarioRepository
                .Setup(x => x.GetByFirebaseUidAsync(firebaseUid, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Usuario?)null);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _useCase.HandleAsync(firebaseUid, votacionId, restauranteId));
        }

        [Fact]
        public async Task HandleAsync_VotacionNoEncontrada_LanzaArgumentException()
        {
            // Arrange
            var firebaseUid = "firebase123";
            var votacionId = Guid.NewGuid();
            var restauranteId = Guid.NewGuid();
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
                () => _useCase.HandleAsync(firebaseUid, votacionId, restauranteId));

            Assert.Equal("Votación no encontrada", exception.Message);
        }

        [Fact]
        public async Task HandleAsync_VotacionNoActiva_LanzaInvalidOperationException()
        {
            // Arrange
            var firebaseUid = "firebase123";
            var votacionId = Guid.NewGuid();
            var restauranteId = Guid.NewGuid();
            var grupoId = Guid.NewGuid();
            var usuario = new Usuario
            {
                Id = Guid.NewGuid(),
                FirebaseUid = "testFirebaseUid",
                Email = "test@test.com",
                Nombre = "Test",
                Apellido = "User"
            };
            var votacion = new VotacionGrupo(grupoId);
            votacion.CerrarVotacion();

            _mockUsuarioRepository
                .Setup(x => x.GetByFirebaseUidAsync(firebaseUid, It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            _mockVotacionRepository
                .Setup(x => x.ObtenerPorIdAsync(votacionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(votacion);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _useCase.HandleAsync(firebaseUid, votacionId, restauranteId));

            Assert.Equal("La votación no está activa", exception.Message);
        }

        [Fact]
        public async Task HandleAsync_UsuarioNoEsMiembro_LanzaUnauthorizedAccessException()
        {
            // Arrange
            var firebaseUid = "firebase123";
            var votacionId = Guid.NewGuid();
            var restauranteId = Guid.NewGuid();
            var grupoId = Guid.NewGuid();
            var adminId = Guid.NewGuid();
            var usuario = new Usuario
            {
                Id = Guid.NewGuid(),
                FirebaseUid = "firebase123",
                Email = "test@test.com",
                Nombre = "Test",
                Apellido = "User"
            };
            var admin = new Usuario
            {
                Id = adminId,
                FirebaseUid = "admin123",
                Email = "admin@test.com",
                Nombre = "Admin",
                Apellido = "User"
            };
            var grupo = new Grupo("Test Grupo", adminId);
            
            var votacion = new VotacionGrupo(grupoId) { Grupo = grupo };

            _mockUsuarioRepository
                .Setup(x => x.GetByFirebaseUidAsync(firebaseUid, It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            _mockVotacionRepository
                .Setup(x => x.ObtenerPorIdAsync(votacionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(votacion);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _useCase.HandleAsync(firebaseUid, votacionId, restauranteId));
        }

        [Fact]
        public async Task HandleAsync_MiembroNoAfectaRecomendacion_LanzaInvalidOperationException()
        {
            // Arrange
            var firebaseUid = "firebase123";
            var votacionId = Guid.NewGuid();
            var restauranteId = Guid.NewGuid();
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
            var admin = new Usuario
            {
                Id = adminId,
                FirebaseUid = "adminFirebaseUid",
                Email = "admin@test.com",
                Nombre = "Admin",
                Apellido = "User"
            };
            var grupo = new Grupo("Test Grupo", adminId);
            
            // Agregar miembro que NO afecta recomendación
            var miembro = new MiembroGrupo(grupoId, usuario.Id, false);
            typeof(MiembroGrupo).GetProperty("afectarRecomendacion")!.SetValue(miembro, false);
            grupo.Miembros.Add(miembro);
            
            var votacion = new VotacionGrupo(grupoId) { Grupo = grupo };

            _mockUsuarioRepository
                .Setup(x => x.GetByFirebaseUidAsync(firebaseUid, It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            _mockVotacionRepository
                .Setup(x => x.ObtenerPorIdAsync(votacionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(votacion);

            _mockRestauranteRepository
                .Setup(x => x.GetRestauranteByIdAsync(restauranteId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Restaurante
                {
                    Id = Guid.NewGuid(),
                    PlaceId = "testPlaceId",
                    Nombre = "Test Restaurant"
                });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _useCase.HandleAsync(firebaseUid, votacionId, restauranteId));

            Assert.Equal("No puedes votar porque no estás marcado para asistir a la reunión", exception.Message);
        }

        [Fact]
        public async Task HandleAsync_RestauranteNoEncontrado_LanzaArgumentException()
        {
            // Arrange
            var firebaseUid = "firebase123";
            var votacionId = Guid.NewGuid();
            var restauranteId = Guid.NewGuid();
            var grupoId = Guid.NewGuid();
            var adminId = Guid.NewGuid();
            var usuario = new Usuario
            {
                Id = Guid.NewGuid(),
                FirebaseUid = "firebase123",
                Email = "test@test.com",
                Nombre = "Test",
                Apellido = "User"
            };
            var admin = new Usuario
            {
                Id = adminId,
                FirebaseUid = "admin123",
                Email = "admin@test.com",
                Nombre = "Admin",
                Apellido = "User"
            };
            var grupo = new Grupo("Test Grupo", adminId);
            
            var miembro = new MiembroGrupo(grupoId, usuario.Id, false);
            typeof(MiembroGrupo).GetProperty("afectarRecomendacion")!.SetValue(miembro, true);
            grupo.Miembros.Add(miembro);
            
            var votacion = new VotacionGrupo(grupoId) { Grupo = grupo };

            _mockUsuarioRepository
                .Setup(x => x.GetByFirebaseUidAsync(firebaseUid, It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            _mockVotacionRepository
                .Setup(x => x.ObtenerPorIdAsync(votacionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(votacion);

            _mockRestauranteRepository
                .Setup(x => x.GetRestauranteByIdAsync(restauranteId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Restaurante?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _useCase.HandleAsync(firebaseUid, votacionId, restauranteId));

            Assert.Equal("Restaurante no encontrado", exception.Message);
        }

        [Fact]
        public async Task HandleAsync_NuevoVoto_RegistraVoto()
        {
            // Arrange
            var firebaseUid = "firebase123";
            var votacionId = Guid.NewGuid();
            var restauranteId = Guid.NewGuid();
            var grupoId = Guid.NewGuid();
            var adminId = Guid.NewGuid();
            var comentario = "Me gusta este lugar";
            var usuario = new Usuario
            {
                Id = Guid.NewGuid(),
                FirebaseUid = "testFirebaseUid",
                Email = "test@test.com",
                Nombre = "Test",
                Apellido = "User"
            };
            var admin = new Usuario
            {
                Id = adminId,
                FirebaseUid = "adminFirebaseUid",
                Email = "admin@test.com",
                Nombre = "Admin",
                Apellido = "User"
            };
            var grupo = new Grupo("Test Grupo", adminId);
            
            var miembro = new MiembroGrupo(grupoId, usuario.Id, false);
            typeof(MiembroGrupo).GetProperty("afectarRecomendacion")!.SetValue(miembro, true);
            grupo.Miembros.Add(miembro);
            
            var votacion = new VotacionGrupo(grupoId) { Grupo = grupo };
            var restaurante = new Restaurante
            {
                Id = Guid.NewGuid(),
                PlaceId = "testPlaceId",
                Nombre = "Test Restaurant"
            };
            var nuevoVoto = new VotoRestaurante(votacionId, usuario.Id, restauranteId, comentario);

            _mockUsuarioRepository
                .Setup(x => x.GetByFirebaseUidAsync(firebaseUid, It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            _mockVotacionRepository
                .Setup(x => x.ObtenerPorIdAsync(votacionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(votacion);

            _mockRestauranteRepository
                .Setup(x => x.GetRestauranteByIdAsync(restauranteId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(restaurante);

            _mockVotacionRepository
                .Setup(x => x.ObtenerVotoUsuarioAsync(votacionId, usuario.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((VotoRestaurante?)null);

            _mockVotacionRepository
                .Setup(x => x.RegistrarVotoAsync(It.IsAny<VotoRestaurante>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(nuevoVoto);

            // Act
            var resultado = await _useCase.HandleAsync(firebaseUid, votacionId, restauranteId, comentario);

            // Assert
            Assert.NotNull(resultado);
            _mockVotacionRepository.Verify(
                x => x.RegistrarVotoAsync(It.IsAny<VotoRestaurante>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task HandleAsync_VotoExistente_ActualizaVoto()
        {
            // Arrange
            var firebaseUid = "firebase123";
            var votacionId = Guid.NewGuid();
            var restauranteId = Guid.NewGuid();
            var nuevoRestauranteId = Guid.NewGuid();
            var grupoId = Guid.NewGuid();
            var adminId = Guid.NewGuid();
            var nuevoComentario = "Cambié de opinión";
            var usuario = new Usuario
            {
                Id = Guid.NewGuid(),
                FirebaseUid = "testFirebaseUid",
                Email = "test@test.com",
                Nombre = "Test",
                Apellido = "User"
            };
            var admin = new Usuario
            {
                Id = adminId,
                FirebaseUid = "adminFirebaseUid",
                Email = "admin@test.com",
                Nombre = "Admin",
                Apellido = "User"
            };
            var grupo = new Grupo("Test Grupo", adminId);
            
            var miembro = new MiembroGrupo(grupoId, usuario.Id, false);
            typeof(MiembroGrupo).GetProperty("afectarRecomendacion")!.SetValue(miembro, true);
            grupo.Miembros.Add(miembro);
            
            var votacion = new VotacionGrupo(grupoId) { Grupo = grupo };
            var restaurante = new Restaurante
            {
                Id = Guid.NewGuid(),
                PlaceId = "testPlaceId",
                Nombre = "Test Restaurant"
            };
            var votoExistente = new VotoRestaurante(votacionId, usuario.Id, restauranteId, "Comentario viejo");

            _mockUsuarioRepository
                .Setup(x => x.GetByFirebaseUidAsync(firebaseUid, It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            _mockVotacionRepository
                .Setup(x => x.ObtenerPorIdAsync(votacionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(votacion);

            _mockRestauranteRepository
                .Setup(x => x.GetRestauranteByIdAsync(nuevoRestauranteId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(restaurante);

            _mockVotacionRepository
                .Setup(x => x.ObtenerVotoUsuarioAsync(votacionId, usuario.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(votoExistente);

            _mockVotacionRepository
                .Setup(x => x.ActualizarVotacionAsync(votacion, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var resultado = await _useCase.HandleAsync(firebaseUid, votacionId, nuevoRestauranteId, nuevoComentario);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(votoExistente, resultado);
            _mockVotacionRepository.Verify(
                x => x.ActualizarVotacionAsync(votacion, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task HandleAsync_VotoSinComentario_RegistraVotoSinComentario()
        {
            // Arrange
            var firebaseUid = "firebase123";
            var votacionId = Guid.NewGuid();
            var restauranteId = Guid.NewGuid();
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
            var admin = new Usuario
            {
                Id = adminId,
                FirebaseUid = "adminFirebaseUid",
                Email = "admin@test.com",
                Nombre = "Admin",
                Apellido = "User"
            };
            var grupo = new Grupo("Test Grupo", adminId);
            
            var miembro = new MiembroGrupo(grupoId, usuario.Id, false);
            typeof(MiembroGrupo).GetProperty("afectarRecomendacion")!.SetValue(miembro, true);
            grupo.Miembros.Add(miembro);
            
            var votacion = new VotacionGrupo(grupoId) { Grupo = grupo };
            var restaurante = new Restaurante
            {
                Id = Guid.NewGuid(),
                PlaceId = "testPlaceId",
                Nombre = "Test Restaurant"
            };
            var nuevoVoto = new VotoRestaurante(votacionId, usuario.Id, restauranteId, null);

            _mockUsuarioRepository
                .Setup(x => x.GetByFirebaseUidAsync(firebaseUid, It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            _mockVotacionRepository
                .Setup(x => x.ObtenerPorIdAsync(votacionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(votacion);

            _mockRestauranteRepository
                .Setup(x => x.GetRestauranteByIdAsync(restauranteId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(restaurante);

            _mockVotacionRepository
                .Setup(x => x.ObtenerVotoUsuarioAsync(votacionId, usuario.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((VotoRestaurante?)null);

            _mockVotacionRepository
                .Setup(x => x.RegistrarVotoAsync(It.IsAny<VotoRestaurante>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(nuevoVoto);

            // Act
            var resultado = await _useCase.HandleAsync(firebaseUid, votacionId, restauranteId);

            // Assert
            Assert.NotNull(resultado);
            _mockVotacionRepository.Verify(
                x => x.RegistrarVotoAsync(It.IsAny<VotoRestaurante>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
