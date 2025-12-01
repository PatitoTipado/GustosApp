using System;
using System.Collections.Generic;
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
    public class CerrarVotacionUseCaseTests
    {
        private readonly Mock<IVotacionRepository> _mockVotacionRepository;
        private readonly Mock<IUsuarioRepository> _mockUsuarioRepository;
        private readonly Mock<IGrupoRepository> _mockGrupoRepository;
        private readonly Mock<INotificacionesVotacionService> _mockNotificaciones;
        private readonly CerrarVotacionUseCase _useCase;

        public CerrarVotacionUseCaseTests()
        {
            _mockVotacionRepository = new Mock<IVotacionRepository>();
            _mockUsuarioRepository = new Mock<IUsuarioRepository>();
            _mockGrupoRepository = new Mock<IGrupoRepository>();
            _mockNotificaciones = new Mock<INotificacionesVotacionService>();

            _useCase = new CerrarVotacionUseCase(
                _mockVotacionRepository.Object,
                _mockUsuarioRepository.Object,
                _mockGrupoRepository.Object,
                _mockNotificaciones.Object);
        }

       
        [Fact]
        public async Task HandleAsync_UsuarioNoEncontrado_LanzaUnauthorizedAccessException()
        {
            var firebaseUid = "firebase123";
            var votacionId = Guid.NewGuid();

            _mockUsuarioRepository
                .Setup(x => x.GetByFirebaseUidAsync(firebaseUid, default))
                .ReturnsAsync((Usuario?)null);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _useCase.HandleAsync(firebaseUid, votacionId));
        }

      
        [Fact]
        public async Task HandleAsync_VotacionNoEncontrada_LanzaArgumentException()
        {
            var firebaseUid = "firebase123";
            var votacionId = Guid.NewGuid();
            var usuario = new Usuario { Id = Guid.NewGuid(), FirebaseUid = firebaseUid };

            _mockUsuarioRepository
                .Setup(x => x.GetByFirebaseUidAsync(firebaseUid, default))
                .ReturnsAsync(usuario);

            _mockVotacionRepository
                .Setup(x => x.ObtenerPorIdConCandidatosAsync(votacionId, default))
                .ReturnsAsync((VotacionGrupo?)null);

            var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
                _useCase.HandleAsync(firebaseUid, votacionId));

            Assert.Equal("Votación no encontrada", ex.Message);
        }

        [Fact]
        public async Task HandleAsync_UsuarioNoEsAdmin_LanzaUnauthorizedAccessException()
        {
            var firebaseUid = "firebase123";
            var usuario = new Usuario { Id = Guid.NewGuid(), FirebaseUid = firebaseUid };

            var grupo = new Grupo("Test", Guid.NewGuid()) { Id = Guid.NewGuid() }; // otro admin
            var votacion = new VotacionGrupo(grupo.Id) { Grupo = grupo };

            _mockUsuarioRepository.Setup(x => x.GetByFirebaseUidAsync(firebaseUid, default))
                .ReturnsAsync(usuario);

            _mockVotacionRepository.Setup(x => x.ObtenerPorIdConCandidatosAsync(It.IsAny<Guid>(), default))
                .ReturnsAsync(votacion);

            var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _useCase.HandleAsync(firebaseUid, Guid.NewGuid()));

            Assert.Equal("Solo el administrador puede cerrar la votación", ex.Message);
        }

        
        [Fact]
        public async Task HandleAsync_VotacionNoActiva_LanzaInvalidOperationException()
        {
            var firebaseUid = "firebase123";
            var usuario = new Usuario { Id = Guid.NewGuid(), FirebaseUid = firebaseUid };

            var grupo = new Grupo("Test", usuario.Id) { Id = Guid.NewGuid() };
            var votacion = new VotacionGrupo(grupo.Id) { Grupo = grupo };

            votacion.CerrarVotacion();

            _mockUsuarioRepository.Setup(x => x.GetByFirebaseUidAsync(firebaseUid, default))
                .ReturnsAsync(usuario);

            _mockVotacionRepository
                .Setup(x => x.ObtenerPorIdConCandidatosAsync(It.IsAny<Guid>(), default))
                .ReturnsAsync(votacion);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _useCase.HandleAsync(firebaseUid, Guid.NewGuid()));

            Assert.Equal("La votación no está activa", ex.Message);
        }

       
        [Fact]
        public async Task HandleAsync_GanadorManualNoCandidato_LanzaInvalidOperationException()
        {
            var firebaseUid = "firebase123";
            var ganador = Guid.NewGuid();

            var usuario = new Usuario { Id = Guid.NewGuid(), FirebaseUid = firebaseUid };
            var grupo = new Grupo("Test", usuario.Id) { Id = Guid.NewGuid() };
            var votacion = new VotacionGrupo(grupo.Id) { Grupo = grupo };

            // No hay candidatos → el ganador NO es candidato
            votacion.RestaurantesCandidatos.Clear();

            _mockUsuarioRepository.Setup(x => x.GetByFirebaseUidAsync(firebaseUid, default))
                .ReturnsAsync(usuario);

            _mockVotacionRepository
                .Setup(x => x.ObtenerPorIdConCandidatosAsync(It.IsAny<Guid>(), default))
                .ReturnsAsync(votacion);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _useCase.HandleAsync(firebaseUid, Guid.NewGuid(), ganador));

            Assert.Equal("El ganador debe ser un restaurante candidato", ex.Message);
        }

       
        [Fact]
        public async Task HandleAsync_GanadorManualEsCandidato_CierraCorrectamente()
        {
            var firebaseUid = "firebase123";
            var candidato = Guid.NewGuid();

            var usuario = new Usuario { Id = Guid.NewGuid(), FirebaseUid = firebaseUid };
            var grupo = new Grupo("Test", usuario.Id) { Id = Guid.NewGuid() };
            var votacion = new VotacionGrupo(grupo.Id) { Grupo = grupo };

            votacion.RestaurantesCandidatos.Add(new VotacionRestaurante(votacion.Id, candidato));

            _mockUsuarioRepository.Setup(x => x.GetByFirebaseUidAsync(firebaseUid, default))
                .ReturnsAsync(usuario);

            _mockVotacionRepository
                .Setup(x => x.ObtenerPorIdConCandidatosAsync(It.IsAny<Guid>(), default))
                .ReturnsAsync(votacion);

            _mockVotacionRepository
                .Setup(x => x.ActualizarVotacionAsync(votacion, default))
                .Returns(Task.CompletedTask);

            var resultado = await _useCase.HandleAsync(firebaseUid, Guid.NewGuid(), candidato);

            Assert.Equal(EstadoVotacion.Cerrada, resultado.Estado);
            Assert.Equal(candidato, resultado.RestauranteGanadorId);

            _mockNotificaciones.Verify(n =>
                n.NotificarVotacionCerrada(grupo.Id, votacion.Id, candidato), Times.Once);
        }

       
        [Fact]
        public async Task HandleAsync_GanadorAutomaticoClaro_CierraYAsignaGanador()
        {
            var firebaseUid = "firebase123";
            var usuarioId = Guid.NewGuid();
            var restaurante1 = Guid.NewGuid();
            var restaurante2 = Guid.NewGuid();

            var usuario = new Usuario { Id = usuarioId, FirebaseUid = firebaseUid };

            var grupo = new Grupo("Test", usuarioId) { Id = Guid.NewGuid() };
            var votacion = new VotacionGrupo(grupo.Id) { Grupo = grupo };

            votacion.RestaurantesCandidatos.Add(new VotacionRestaurante(votacion.Id, restaurante1));
            votacion.RestaurantesCandidatos.Add(new VotacionRestaurante(votacion.Id, restaurante2));

            // restaurante1 gana 2-1
            votacion.Votos.Add(new VotoRestaurante(votacion.Id, usuarioId, restaurante1, null));
            votacion.Votos.Add(new VotoRestaurante(votacion.Id, Guid.NewGuid(), restaurante1, null));
            votacion.Votos.Add(new VotoRestaurante(votacion.Id, Guid.NewGuid(), restaurante2, null));

            _mockUsuarioRepository.Setup(x => x.GetByFirebaseUidAsync(firebaseUid, default))
                .ReturnsAsync(usuario);

            _mockVotacionRepository
                .Setup(x => x.ObtenerPorIdConCandidatosAsync(It.IsAny<Guid>(), default))
                .ReturnsAsync(votacion);

            var result = await _useCase.HandleAsync(firebaseUid, Guid.NewGuid());

            Assert.Equal(restaurante1, result.RestauranteGanadorId);
            Assert.Equal(EstadoVotacion.Cerrada, result.Estado);

            _mockNotificaciones.Verify(n =>
                n.NotificarVotacionCerrada(grupo.Id, votacion.Id, restaurante1), Times.Once);
        }

       
        [Fact]
        public async Task HandleAsync_Empate_CierraSinGanador()
        {
            var firebaseUid = "firebase123";
            var usuarioId = Guid.NewGuid();

            var r1 = Guid.NewGuid();
            var r2 = Guid.NewGuid();

            var usuario = new Usuario { Id = usuarioId, FirebaseUid = firebaseUid };

            var grupo = new Grupo("Test", usuarioId) { Id = Guid.NewGuid() };
            var votacion = new VotacionGrupo(grupo.Id) { Grupo = grupo };

            votacion.RestaurantesCandidatos.Add(new VotacionRestaurante(votacion.Id, r1));
            votacion.RestaurantesCandidatos.Add(new VotacionRestaurante(votacion.Id, r2));

            votacion.Votos.Add(new VotoRestaurante(votacion.Id, usuarioId, r1, null));
            votacion.Votos.Add(new VotoRestaurante(votacion.Id, Guid.NewGuid(), r2, null));

            _mockUsuarioRepository.Setup(x => x.GetByFirebaseUidAsync(firebaseUid, default))
                .ReturnsAsync(usuario);

            _mockVotacionRepository
                .Setup(x => x.ObtenerPorIdConCandidatosAsync(It.IsAny<Guid>(), default))
                .ReturnsAsync(votacion);

            var result = await _useCase.HandleAsync(firebaseUid, Guid.NewGuid());

            Assert.Null(result.RestauranteGanadorId);
            Assert.Equal(EstadoVotacion.Cerrada, result.Estado);

            _mockNotificaciones.Verify(n =>
                n.NotificarVotacionCerrada(grupo.Id, votacion.Id, null), Times.Once);
        }

        
        [Fact]
        public async Task HandleAsync_SinVotos_CierraSinGanador()
        {
            var firebaseUid = "firebase123";
            var usuario = new Usuario { Id = Guid.NewGuid(), FirebaseUid = firebaseUid };

            var grupo = new Grupo("Test", usuario.Id) { Id = Guid.NewGuid() };
            var votacion = new VotacionGrupo(grupo.Id) { Grupo = grupo };

            votacion.RestaurantesCandidatos.Add(new VotacionRestaurante(votacion.Id, Guid.NewGuid()));

            _mockUsuarioRepository.Setup(x => x.GetByFirebaseUidAsync(firebaseUid, default))
                .ReturnsAsync(usuario);

            _mockVotacionRepository
                .Setup(x => x.ObtenerPorIdConCandidatosAsync(It.IsAny<Guid>(), default))
                .ReturnsAsync(votacion);

            var result = await _useCase.HandleAsync(firebaseUid, Guid.NewGuid());

            Assert.Null(result.RestauranteGanadorId);
            Assert.Equal(EstadoVotacion.Cerrada, result.Estado);

            _mockNotificaciones.Verify(n =>
                n.NotificarVotacionCerrada(grupo.Id, votacion.Id, null), Times.Once);
        }
    }

}
