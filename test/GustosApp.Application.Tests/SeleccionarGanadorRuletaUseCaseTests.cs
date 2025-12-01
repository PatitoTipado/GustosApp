using System;
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
    public class SeleccionarGanadorRuletaUseCaseTests
    {
        private readonly Mock<IVotacionRepository> _mockVotacionRepository;
        private readonly Mock<IUsuarioRepository> _mockUsuarioRepository;
        private readonly SeleccionarGanadorRuletaUseCase _useCase;

        public SeleccionarGanadorRuletaUseCaseTests()
        {
            _mockVotacionRepository = new Mock<IVotacionRepository>();
            _mockUsuarioRepository = new Mock<IUsuarioRepository>();

            _useCase = new SeleccionarGanadorRuletaUseCase(
                _mockVotacionRepository.Object,
                _mockUsuarioRepository.Object);
        }

        [Fact]
        public async Task HandleAsync_UsuarioNoEncontrado_LanzaUnauthorizedAccessException()
        {
            // Arrange
            var firebaseUid = "firebase123";
            var votacionId = Guid.NewGuid();
            var restauranteGanadorId = Guid.NewGuid();

            _mockUsuarioRepository
                .Setup(x => x.GetByFirebaseUidAsync(firebaseUid, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Usuario?)null);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _useCase.HandleAsync(firebaseUid, votacionId, restauranteGanadorId));
        }

        [Fact]
        public async Task HandleAsync_VotacionNoEncontrada_LanzaArgumentException()
        {
            // Arrange
            var firebaseUid = "firebase123";
            var votacionId = Guid.NewGuid();
            var restauranteGanadorId = Guid.NewGuid();
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
                () => _useCase.HandleAsync(firebaseUid, votacionId, restauranteGanadorId));

            Assert.Equal("Votación no encontrada", exception.Message);
        }

        [Fact]
        public async Task HandleAsync_UsuarioNoEsMiembro_LanzaUnauthorizedAccessException()
        {
            // Arrange
            var firebaseUid = "firebase123";
            var votacionId = Guid.NewGuid();
            var restauranteGanadorId = Guid.NewGuid();
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
                () => _useCase.HandleAsync(firebaseUid, votacionId, restauranteGanadorId));

            Assert.Equal("No eres miembro de este grupo", exception.Message);
        }

        [Fact]
        public async Task HandleAsync_VotacionNoActiva_LanzaInvalidOperationException()
        {
            // Arrange
            var firebaseUid = "firebase123";
            var votacionId = Guid.NewGuid();
            var restauranteGanadorId = Guid.NewGuid();
            var grupoId = Guid.NewGuid();
            var usuario = new Usuario
            {
                Id = Guid.NewGuid(),
                FirebaseUid = "firebase123",
                Email = "test@test.com",
                Nombre = "Test",
                Apellido = "User"
            };
            var grupo = new Grupo("Test Grupo", usuario.Id) { Id = grupoId };
            
            var miembro = new MiembroGrupo(grupoId, usuario.Id, false);
            grupo.Miembros.Add(miembro);
            
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
                () => _useCase.HandleAsync(firebaseUid, votacionId, restauranteGanadorId));

            Assert.Equal("La votación no está activa", exception.Message);
        }

        [Fact]
        public async Task HandleAsync_NoHayEmpate_LanzaInvalidOperationException()
        {
            // Arrange
            var firebaseUid = "firebase123";
            var votacionId = Guid.NewGuid();
            var restauranteGanadorId = Guid.NewGuid();
            var grupoId = Guid.NewGuid();
            var usuario = new Usuario
            {
                Id = Guid.NewGuid(),
                FirebaseUid = "firebase123",
                Email = "test@test.com",
                Nombre = "Test",
                Apellido = "User"
            };
            var grupo = new Grupo("Test Grupo", usuario.Id) { Id = grupoId };
            
            var miembro = new MiembroGrupo(grupoId, usuario.Id, false);
            grupo.Miembros.Add(miembro);
            
            var votacion = new VotacionGrupo(grupoId) { Grupo = grupo };
            // Sin votos, no hay empate

            _mockUsuarioRepository
                .Setup(x => x.GetByFirebaseUidAsync(firebaseUid, It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            _mockVotacionRepository
                .Setup(x => x.ObtenerPorIdAsync(votacionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(votacion);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _useCase.HandleAsync(firebaseUid, votacionId, restauranteGanadorId));

            Assert.Equal("No hay empate en esta votación", exception.Message);
        }

        [Fact]
        public async Task HandleAsync_GanadorYaSeleccionado_LanzaInvalidOperationException()
        {
            // Arrange
            var firebaseUid = "firebase123";
            var votacionId = Guid.NewGuid();
            var restauranteGanadorId = Guid.NewGuid();
            var restaurante1Id = Guid.NewGuid();
            var restaurante2Id = Guid.NewGuid();
            var grupoId = Guid.NewGuid();
            var usuario1 = new Usuario
            {
                Id = Guid.NewGuid(),
                FirebaseUid = "user1FirebaseUid",
                Email = "user1@test.com",
                Nombre = "User",
                Apellido = "One"
            };
            var usuario2 = new Usuario
            {
                Id = Guid.NewGuid(),
                FirebaseUid = "user2FirebaseUid",
                Email = "user2@test.com",
                Nombre = "User",
                Apellido = "Two"
            };
            var grupo = new Grupo("Test Grupo", usuario1.Id) { Id = grupoId };
            
            var miembro1 = new MiembroGrupo(grupoId, usuario1.Id, false);
            var miembro2 = new MiembroGrupo(grupoId, usuario2.Id, false);
            grupo.Miembros.Add(miembro1);
            grupo.Miembros.Add(miembro2);
            
            var votacion = new VotacionGrupo(grupoId) { Grupo = grupo };
            
            // Crear empate
            votacion.Votos.Add(new VotoRestaurante(votacionId, usuario1.Id, restaurante1Id, null));
            votacion.Votos.Add(new VotoRestaurante(votacionId, usuario2.Id, restaurante2Id, null));
            
            // Ya tiene ganador seleccionado
            votacion.EstablecerGanadorRuleta(restaurante1Id);

            _mockUsuarioRepository
                .Setup(x => x.GetByFirebaseUidAsync(firebaseUid, It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario1);

            _mockVotacionRepository
                .Setup(x => x.ObtenerPorIdAsync(votacionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(votacion);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _useCase.HandleAsync(firebaseUid, votacionId, restauranteGanadorId));

            Assert.Equal("Ya se seleccionó un ganador para esta votación", exception.Message);
        }

        [Fact]
        public async Task HandleAsync_RestauranteNoEnEmpate_LanzaArgumentException()
        {
            // Arrange
            var firebaseUid = "firebase123";
            var votacionId = Guid.NewGuid();
            var restauranteNoEmpatadoId = Guid.NewGuid();
            var restaurante1Id = Guid.NewGuid();
            var restaurante2Id = Guid.NewGuid();
            var grupoId = Guid.NewGuid();
            var usuario1 = new Usuario
            {
                Id = Guid.NewGuid(),
                FirebaseUid = "user1FirebaseUid",
                Email = "user1@test.com",
                Nombre = "User",
                Apellido = "One"
            };
            var usuario2 = new Usuario
            {
                Id = Guid.NewGuid(),
                FirebaseUid = "user2FirebaseUid",
                Email = "user2@test.com",
                Nombre = "User",
                Apellido = "Two"
            };
            var grupo = new Grupo("Test Grupo", usuario1.Id) { Id = grupoId };
            
            var miembro1 = new MiembroGrupo(grupoId, usuario1.Id, false);
            var miembro2 = new MiembroGrupo(grupoId, usuario2.Id, false);
            grupo.Miembros.Add(miembro1);
            grupo.Miembros.Add(miembro2);
            
            var votacion = new VotacionGrupo(grupoId) { Grupo = grupo };
            
            // Crear empate entre restaurante1 y restaurante2
            votacion.Votos.Add(new VotoRestaurante(votacionId, usuario1.Id, restaurante1Id, null));
            votacion.Votos.Add(new VotoRestaurante(votacionId, usuario2.Id, restaurante2Id, null));

            _mockUsuarioRepository
                .Setup(x => x.GetByFirebaseUidAsync(firebaseUid, It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario1);

            _mockVotacionRepository
                .Setup(x => x.ObtenerPorIdAsync(votacionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(votacion);

            // Act & Assert - intentar seleccionar restaurante que NO está en empate
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _useCase.HandleAsync(firebaseUid, votacionId, restauranteNoEmpatadoId));

            Assert.Equal("El restaurante seleccionado no está entre los empatados", exception.Message);
        }

        [Fact]
        public async Task HandleAsync_TodoValido_SeleccionaGanadorRuleta()
        {
            // Arrange
            var firebaseUid = "firebase123";
            var votacionId = Guid.NewGuid();
            var restaurante1Id = Guid.NewGuid();
            var restaurante2Id = Guid.NewGuid();
            var grupoId = Guid.NewGuid();
            var usuario1 = new Usuario
            {
                Id = Guid.NewGuid(),
                FirebaseUid = "user1FirebaseUid",
                Email = "user1@test.com",
                Nombre = "User",
                Apellido = "One"
            };
            var usuario2 = new Usuario
            {
                Id = Guid.NewGuid(),
                FirebaseUid = "user2FirebaseUid",
                Email = "user2@test.com",
                Nombre = "User",
                Apellido = "Two"
            };
            var grupo = new Grupo("Test Grupo", usuario1.Id) { Id = grupoId };
            
            var miembro1 = new MiembroGrupo(grupoId, usuario1.Id, false);
            var miembro2 = new MiembroGrupo(grupoId, usuario2.Id, false);
            grupo.Miembros.Add(miembro1);
            grupo.Miembros.Add(miembro2);
            
            var votacion = new VotacionGrupo(grupoId) { Grupo = grupo };
            
            // Crear empate
            votacion.Votos.Add(new VotoRestaurante(votacionId, usuario1.Id, restaurante1Id, null));
            votacion.Votos.Add(new VotoRestaurante(votacionId, usuario2.Id, restaurante2Id, null));

            _mockUsuarioRepository
                .Setup(x => x.GetByFirebaseUidAsync(firebaseUid, It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario1);

            _mockVotacionRepository
                .Setup(x => x.ObtenerPorIdAsync(votacionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(votacion);

            _mockVotacionRepository
                .Setup(x => x.ActualizarVotacionAsync(It.IsAny<VotacionGrupo>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act - seleccionar uno de los empatados
            var resultado = await _useCase.HandleAsync(firebaseUid, votacionId, restaurante1Id);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(restaurante1Id, resultado.RestauranteGanadorId);
            Assert.Equal(EstadoVotacion.Activa, resultado.Estado);
            _mockVotacionRepository.Verify(
                x => x.ActualizarVotacionAsync(It.IsAny<VotacionGrupo>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task HandleAsync_SeleccionarOtroRestauranteEmpatado_Funciona()
        {
            // Arrange
            var firebaseUid = "firebase123";
            var votacionId = Guid.NewGuid();
            var restaurante1Id = Guid.NewGuid();
            var restaurante2Id = Guid.NewGuid();
            var grupoId = Guid.NewGuid();
            var usuario1 = new Usuario
            {
                Id = Guid.NewGuid(),
                FirebaseUid = "user1FirebaseUid",
                Email = "user1@test.com",
                Nombre = "User",
                Apellido = "One"
            };
            var usuario2 = new Usuario
            {
                Id = Guid.NewGuid(),
                FirebaseUid = "user2FirebaseUid",
                Email = "user2@test.com",
                Nombre = "User",
                Apellido = "Two"
            };
            var grupo = new Grupo("Test Grupo", usuario1.Id) { Id = grupoId };
            
            var miembro1 = new MiembroGrupo(grupoId, usuario1.Id, false);
            var miembro2 = new MiembroGrupo(grupoId, usuario2.Id, false);
            grupo.Miembros.Add(miembro1);
            grupo.Miembros.Add(miembro2);
            
            var votacion = new VotacionGrupo(grupoId) { Grupo = grupo };
            
            // Crear empate
            votacion.Votos.Add(new VotoRestaurante(votacionId, usuario1.Id, restaurante1Id, null));
            votacion.Votos.Add(new VotoRestaurante(votacionId, usuario2.Id, restaurante2Id, null));

            _mockUsuarioRepository
                .Setup(x => x.GetByFirebaseUidAsync(firebaseUid, It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario1);

            _mockVotacionRepository
                .Setup(x => x.ObtenerPorIdAsync(votacionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(votacion);

            _mockVotacionRepository
                .Setup(x => x.ActualizarVotacionAsync(It.IsAny<VotacionGrupo>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act - seleccionar el segundo restaurante empatado
            var resultado = await _useCase.HandleAsync(firebaseUid, votacionId, restaurante2Id);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(restaurante2Id, resultado.RestauranteGanadorId);
        }

        [Fact]
        public async Task HandleAsync_EmpateDeMultiples_PermiteSeleccionarCualquiera()
        {
            // Arrange
            var firebaseUid = "firebase123";
            var votacionId = Guid.NewGuid();
            var restaurante1Id = Guid.NewGuid();
            var restaurante2Id = Guid.NewGuid();
            var restaurante3Id = Guid.NewGuid();
            var grupoId = Guid.NewGuid();
            var usuario1 = new Usuario
            {
                Id = Guid.NewGuid(),
                FirebaseUid = "user1FirebaseUid",
                Email = "user1@test.com",
                Nombre = "User",
                Apellido = "1"
            };
            var usuario2 = new Usuario
            {
                Id = Guid.NewGuid(),
                FirebaseUid = "user2FirebaseUid",
                Email = "user2@test.com",
                Nombre = "User",
                Apellido = "2"
            };
            var usuario3 = new Usuario
            {
                Id = Guid.NewGuid(),
                FirebaseUid = "user3FirebaseUid",
                Email = "user3@test.com",
                Nombre = "User",
                Apellido = "3"
            };
            var grupo = new Grupo("Test Grupo", usuario1.Id) { Id = grupoId };
            
            var miembro1 = new MiembroGrupo(grupoId, usuario1.Id, false);
            var miembro2 = new MiembroGrupo(grupoId, usuario2.Id, false);
            var miembro3 = new MiembroGrupo(grupoId, usuario3.Id, false);
            grupo.Miembros.Add(miembro1);
            grupo.Miembros.Add(miembro2);
            grupo.Miembros.Add(miembro3);
            
            var votacion = new VotacionGrupo(grupoId) { Grupo = grupo };
            
            // Crear empate triple
            votacion.Votos.Add(new VotoRestaurante(votacionId, usuario1.Id, restaurante1Id, null));
            votacion.Votos.Add(new VotoRestaurante(votacionId, usuario2.Id, restaurante2Id, null));
            votacion.Votos.Add(new VotoRestaurante(votacionId, usuario3.Id, restaurante3Id, null));

            _mockUsuarioRepository
                .Setup(x => x.GetByFirebaseUidAsync(firebaseUid, It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario1);

            _mockVotacionRepository
                .Setup(x => x.ObtenerPorIdAsync(votacionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(votacion);

            _mockVotacionRepository
                .Setup(x => x.ActualizarVotacionAsync(It.IsAny<VotacionGrupo>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act - seleccionar el tercero
            var resultado = await _useCase.HandleAsync(firebaseUid, votacionId, restaurante3Id);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(restaurante3Id, resultado.RestauranteGanadorId);
        }
    }
}
