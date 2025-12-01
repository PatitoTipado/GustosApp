using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GustosApp.Application.Interfaces;
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
        private readonly Mock<INotificacionesVotacionService> _mockNotificaciones;
        private readonly SeleccionarGanadorRuletaUseCase _useCase;

        public SeleccionarGanadorRuletaUseCaseTests()
        {
            _mockVotacionRepository = new Mock<IVotacionRepository>();
            _mockUsuarioRepository = new Mock<IUsuarioRepository>();
            _mockNotificaciones = new Mock<INotificacionesVotacionService>();

            _useCase = new SeleccionarGanadorRuletaUseCase(
                _mockVotacionRepository.Object,
                _mockUsuarioRepository.Object,
                _mockNotificaciones.Object
            );
        }

       
     
        [Fact]
        public async Task HandleAsync_UsuarioNoEncontrado_LanzaUnauthorizedAccessException()
        {
            var firebaseUid = "firebase123";
            var votacionId = Guid.NewGuid();

            _mockUsuarioRepository
                .Setup(x => x.GetByFirebaseUidAsync(firebaseUid, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Usuario?)null);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _useCase.HandleAsync(firebaseUid, votacionId, Guid.NewGuid()));
        }

        [Fact]
        public async Task HandleAsync_VotacionNoEncontrada_LanzaArgumentException()
        {
            var firebaseUid = "firebase123";
            var votacionId = Guid.NewGuid();
            var usuario = new Usuario { Id = Guid.NewGuid(), FirebaseUid = firebaseUid };

            _mockUsuarioRepository
                .Setup(x => x.GetByFirebaseUidAsync(firebaseUid, It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            _mockVotacionRepository
                .Setup(x => x.ObtenerPorIdConCandidatosAsync(votacionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((VotacionGrupo?)null);

            var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
                _useCase.HandleAsync(firebaseUid, votacionId, Guid.NewGuid()));

            Assert.Equal("Votación no encontrada", ex.Message);
        }

 
        [Fact]
        public async Task HandleAsync_UsuarioNoEsAdmin_LanzaUnauthorizedAccessException()
        {
            var firebaseUid = "firebase123";
            var votacionId = Guid.NewGuid();

            var usuario = new Usuario { Id = Guid.NewGuid(), FirebaseUid = firebaseUid };
            var grupo = new Grupo("Test", Guid.NewGuid()) { Id = Guid.NewGuid() }; // otro admin
            var votacion = new VotacionGrupo(grupo.Id) { Grupo = grupo };

            _mockUsuarioRepository.Setup(x => x.GetByFirebaseUidAsync(firebaseUid, default))
                .ReturnsAsync(usuario);

            _mockVotacionRepository.Setup(x => x.ObtenerPorIdConCandidatosAsync(votacionId, default))
                .ReturnsAsync(votacion);

            var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _useCase.HandleAsync(firebaseUid, votacionId, Guid.NewGuid()));

            Assert.Equal("Solo el administrador puede seleccionar el ganador", ex.Message);
        }

 
        [Fact]
        public async Task HandleAsync_VotacionNoActiva_LanzaInvalidOperationException()
        {
            var firebaseUid = "firebase123";
            var votacionId = Guid.NewGuid();

            var usuario = new Usuario { Id = Guid.NewGuid(), FirebaseUid = firebaseUid };
            var grupo = new Grupo("Test", usuario.Id) { Id = Guid.NewGuid() };
            var votacion = new VotacionGrupo(grupo.Id) { Grupo = grupo };

            votacion.CerrarVotacion();

            _mockUsuarioRepository.Setup(x => x.GetByFirebaseUidAsync(firebaseUid, default))
                .ReturnsAsync(usuario);

            _mockVotacionRepository.Setup(x => x.ObtenerPorIdConCandidatosAsync(votacionId, default))
                .ReturnsAsync(votacion);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _useCase.HandleAsync(firebaseUid, votacionId, Guid.NewGuid()));

            Assert.Equal("La votación no está activa", ex.Message);
        }

 
        [Fact]
        public async Task HandleAsync_GanadorYaSeleccionado_LanzaInvalidOperationException()
        {
            var firebaseUid = "firebase123";
            var restaurante1 = Guid.NewGuid();

            var usuario = new Usuario { Id = Guid.NewGuid(), FirebaseUid = firebaseUid };
            var grupo = new Grupo("Test", usuario.Id) { Id = Guid.NewGuid() };
            var votacion = new VotacionGrupo(grupo.Id) { Grupo = grupo };

            votacion.EstablecerGanadorRuleta(restaurante1);

            _mockUsuarioRepository.Setup(x => x.GetByFirebaseUidAsync(firebaseUid, default))
                .ReturnsAsync(usuario);

            _mockVotacionRepository.Setup(x => x.ObtenerPorIdConCandidatosAsync(It.IsAny<Guid>(), default))
                .ReturnsAsync(votacion);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _useCase.HandleAsync(firebaseUid, Guid.NewGuid(), Guid.NewGuid()));

            Assert.Equal("Ya se seleccionó un ganador para esta votación", ex.Message);
        }

        [Fact]
        public async Task HandleAsync_NoHayEmpate_LanzaInvalidOperationException()
        {
            var firebaseUid = "firebase123";
            var usuario = new Usuario { Id = Guid.NewGuid(), FirebaseUid = firebaseUid };
            var grupo = new Grupo("Test", usuario.Id) { Id = Guid.NewGuid() };
            var votacion = new VotacionGrupo(grupo.Id) { Grupo = grupo }; // sin votos → no hay empate

            _mockUsuarioRepository.Setup(x => x.GetByFirebaseUidAsync(firebaseUid, default))
                .ReturnsAsync(usuario);

            _mockVotacionRepository.Setup(x => x.ObtenerPorIdConCandidatosAsync(It.IsAny<Guid>(), default))
                .ReturnsAsync(votacion);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _useCase.HandleAsync(firebaseUid, Guid.NewGuid(), Guid.NewGuid()));

            Assert.Equal("No hay empate en esta votación", ex.Message);
        }

        [Fact]
        public async Task HandleAsync_RestauranteNoEmpatado_LanzaArgumentException()
        {
            var firebaseUid = "firebase123";
            var votacionId = Guid.NewGuid();

            var usuario = new Usuario { Id = Guid.NewGuid(), FirebaseUid = firebaseUid };
            var grupo = new Grupo("Test", usuario.Id) { Id = Guid.NewGuid() };

            var r1 = Guid.NewGuid();
            var r2 = Guid.NewGuid();
            var noEmpatado = Guid.NewGuid();

            var votacion = new VotacionGrupo(grupo.Id) { Grupo = grupo };

            votacion.Votos.Add(new VotoRestaurante(votacionId, usuario.Id, r1, null));
            votacion.Votos.Add(new VotoRestaurante(votacionId, Guid.NewGuid(), r2, null));

            votacion.RestaurantesCandidatos.Add(new VotacionRestaurante(votacion.Id, r1));
            votacion.RestaurantesCandidatos.Add(new VotacionRestaurante(votacion.Id, r2));

            _mockUsuarioRepository.Setup(x => x.GetByFirebaseUidAsync(firebaseUid, default))
                .ReturnsAsync(usuario);

            _mockVotacionRepository.Setup(x => x.ObtenerPorIdConCandidatosAsync(votacionId, default))
                .ReturnsAsync(votacion);

            var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
                _useCase.HandleAsync(firebaseUid, votacionId, noEmpatado));

            Assert.Equal("El restaurante seleccionado no está entre los empatados", ex.Message);
        }


        [Fact]
        public async Task HandleAsync_RestauranteNoEsCandidato_LanzaInvalidOperationException()
        {
            var firebaseUid = "firebase123";
            var votacionId = Guid.NewGuid();

            var usuario = new Usuario { Id = Guid.NewGuid(), FirebaseUid = firebaseUid };
            var grupo = new Grupo("Test", usuario.Id) { Id = Guid.NewGuid() };

            var r1 = Guid.NewGuid();
            var r2 = Guid.NewGuid();
            var ganador = r1;

            var votacion = new VotacionGrupo(grupo.Id) { Grupo = grupo };

            // Empate real
            votacion.Votos.Add(new VotoRestaurante(votacionId, usuario.Id, r1, null));
            votacion.Votos.Add(new VotoRestaurante(votacionId, Guid.NewGuid(), r2, null));

            // NO agrego candidatos → r1 NO es válido

            _mockUsuarioRepository.Setup(x => x.GetByFirebaseUidAsync(firebaseUid, default))
                .ReturnsAsync(usuario);

            _mockVotacionRepository.Setup(x => x.ObtenerPorIdConCandidatosAsync(votacionId, default))
                .ReturnsAsync(votacion);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _useCase.HandleAsync(firebaseUid, votacionId, ganador));

            Assert.Equal("El restaurante seleccionado no es un candidato válido", ex.Message);
        }

    
        [Fact]
        public async Task HandleAsync_TodoValido_SeleccionaGanador()
        {
            var firebaseUid = "firebase123";
            var votacionId = Guid.NewGuid();

            var usuario = new Usuario { Id = Guid.NewGuid(), FirebaseUid = firebaseUid };
            var grupo = new Grupo("Test", usuario.Id) { Id = Guid.NewGuid() };

            var r1 = Guid.NewGuid();
            var r2 = Guid.NewGuid();

            var votacion = new VotacionGrupo(grupo.Id) { Grupo = grupo };

            // Empate real
            votacion.Votos.Add(new VotoRestaurante(votacionId, usuario.Id, r1, null));
            votacion.Votos.Add(new VotoRestaurante(votacionId, Guid.NewGuid(), r2, null));

            // candidatos oficiales
            votacion.RestaurantesCandidatos.Add(new VotacionRestaurante(votacion.Id, r1));
            votacion.RestaurantesCandidatos.Add(new VotacionRestaurante(votacion.Id, r2));

            _mockUsuarioRepository.Setup(x => x.GetByFirebaseUidAsync(firebaseUid, default))
                .ReturnsAsync(usuario);

            _mockVotacionRepository.Setup(x => x.ObtenerPorIdConCandidatosAsync(votacionId, default))
                .ReturnsAsync(votacion);

            _mockVotacionRepository
                .Setup(x => x.ActualizarVotacionAsync(It.IsAny<VotacionGrupo>(), default))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _useCase.HandleAsync(firebaseUid, votacionId, r1);

            // Assert
            Assert.Equal(r1, result.RestauranteGanadorId);
            _mockNotificaciones.Verify(
                n => n.NotificarGanador(grupo.Id, votacion.Id, r1),
                Times.Once);
        }

  
        [Fact]
        public async Task HandleAsync_EmpateMultiple_PermiteSeleccionarCualquiera()
        {
            var firebaseUid = "firebase123";
            var votacionId = Guid.NewGuid();

            var usuario = new Usuario { Id = Guid.NewGuid(), FirebaseUid = firebaseUid };
            var grupo = new Grupo("Test", usuario.Id) { Id = Guid.NewGuid() };

            var r1 = Guid.NewGuid();
            var r2 = Guid.NewGuid();
            var r3 = Guid.NewGuid();

            var votacion = new VotacionGrupo(grupo.Id) { Grupo = grupo };

            votacion.Votos.Add(new VotoRestaurante(votacionId, usuario.Id, r1, null));
            votacion.Votos.Add(new VotoRestaurante(votacionId, Guid.NewGuid(), r2, null));
            votacion.Votos.Add(new VotoRestaurante(votacionId, Guid.NewGuid(), r3, null));

            votacion.RestaurantesCandidatos.Add(new VotacionRestaurante(votacion.Id, r1));
            votacion.RestaurantesCandidatos.Add(new VotacionRestaurante(votacion.Id, r2));
            votacion.RestaurantesCandidatos.Add(new VotacionRestaurante(votacion.Id, r3));

            _mockUsuarioRepository.Setup(x => x.GetByFirebaseUidAsync(firebaseUid, default))
                .ReturnsAsync(usuario);

            _mockVotacionRepository.Setup(x => x.ObtenerPorIdConCandidatosAsync(votacionId, default))
                .ReturnsAsync(votacion);

            _mockVotacionRepository
                .Setup(x => x.ActualizarVotacionAsync(It.IsAny<VotacionGrupo>(), default))
                .Returns(Task.CompletedTask);

            var result = await _useCase.HandleAsync(firebaseUid, votacionId, r3);

            Assert.Equal(r3, result.RestauranteGanadorId);
        }
    }
}

