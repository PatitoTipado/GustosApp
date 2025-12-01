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
    public class ObtenerResultadosVotacionUseCaseTests
    {
        private readonly Mock<IVotacionRepository> _mockVotacionRepository;
        private readonly Mock<IUsuarioRepository> _mockUsuarioRepository;
        private readonly Mock<IGrupoRepository> _mockGrupoRepository;
        private readonly Mock<INotificacionesVotacionService> _mockNotificaciones;
        private readonly ObtenerResultadosVotacionUseCase _useCase;

        public ObtenerResultadosVotacionUseCaseTests()
        {
            _mockVotacionRepository = new Mock<IVotacionRepository>();
            _mockUsuarioRepository = new Mock<IUsuarioRepository>();
            _mockGrupoRepository = new Mock<IGrupoRepository>();
            _mockNotificaciones = new Mock<INotificacionesVotacionService>();

            _useCase = new ObtenerResultadosVotacionUseCase(
                _mockVotacionRepository.Object,
                _mockUsuarioRepository.Object,
                _mockGrupoRepository.Object,
                _mockNotificaciones.Object
            );
        }

       
        [Fact]
        public async Task HandleAsync_UsuarioNoEncontrado_LanzaUnauthorizedAccessException()
        {
            var firebaseUid = "firebase123";

            _mockUsuarioRepository
                .Setup(r => r.GetByFirebaseUidAsync(firebaseUid, default))
                .ReturnsAsync((Usuario?)null);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _useCase.HandleAsync(firebaseUid, Guid.NewGuid()));
        }


       
        [Fact]
        public async Task HandleAsync_VotacionNoEncontrada_LanzaArgumentException()
        {
            var firebaseUid = "uid";
            var usuario = new Usuario { Id = Guid.NewGuid(), FirebaseUid = firebaseUid };

            _mockUsuarioRepository.Setup(r => r.GetByFirebaseUidAsync(firebaseUid, default))
                .ReturnsAsync(usuario);

            _mockVotacionRepository
                .Setup(r => r.ObtenerPorIdConCandidatosAsync(It.IsAny<Guid>(), default))
                .ReturnsAsync((VotacionGrupo?)null);

            var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
                _useCase.HandleAsync(firebaseUid, Guid.NewGuid()));

            Assert.Equal("Votación no encontrada", ex.Message);
        }


        [Fact]
        public async Task HandleAsync_UsuarioNoEsMiembroActivo_LanzaUnauthorizedAccessException()
        {
            var firebaseUid = "uid";
            var usuario = new Usuario { Id = Guid.NewGuid(), FirebaseUid = firebaseUid };

            var grupoId = Guid.NewGuid();
            var grupo = new Grupo("Test", usuario.Id) { Id = grupoId };
            var votacion = new VotacionGrupo(grupoId) { Grupo = grupo };

            // NO agregar miembro → no es miembro activo
            grupo.Miembros.Clear();

            _mockUsuarioRepository.Setup(r => r.GetByFirebaseUidAsync(firebaseUid, default))
                .ReturnsAsync(usuario);

            _mockVotacionRepository
                .Setup(r => r.ObtenerPorIdConCandidatosAsync(It.IsAny<Guid>(), default))
                .ReturnsAsync(votacion);

            _mockGrupoRepository
                .Setup(r => r.UsuarioEsMiembroAsync(grupoId, firebaseUid, default))
                .ReturnsAsync(false);

            var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _useCase.HandleAsync(firebaseUid, Guid.NewGuid()));

            Assert.Equal("No eres un miembro activo del grupo.", ex.Message);
        }


       
        [Fact]
        public async Task HandleAsync_SinVotos_RetornaResultadosVacios()
        {
            var firebaseUid = "uid";
            var grupoId = Guid.NewGuid();
            var usuario = new Usuario { Id = Guid.NewGuid(), FirebaseUid = firebaseUid };

            var grupo = new Grupo("Grupo Test", usuario.Id) { Id = grupoId };
            var miembro = new MiembroGrupo(grupoId, usuario.Id);
            miembro.afectarRecomendacion = true;
            grupo.Miembros.Add(miembro);

            var votacion = new VotacionGrupo(grupoId) { Grupo = grupo };

            _mockUsuarioRepository.Setup(r => r.GetByFirebaseUidAsync(firebaseUid, default))
                .ReturnsAsync(usuario);

            _mockVotacionRepository.Setup(r =>
                r.ObtenerPorIdConCandidatosAsync(votacion.Id, default))
                .ReturnsAsync(votacion);

            _mockGrupoRepository.Setup(r =>
                r.UsuarioEsMiembroAsync(grupoId, firebaseUid, default))
                .ReturnsAsync(true);

            var result = await _useCase.HandleAsync(firebaseUid, votacion.Id);

            Assert.NotNull(result);
            Assert.Equal(1, result.MiembrosActivos);
            Assert.Equal(0, result.TotalVotos);
            Assert.False(result.HayEmpate);
            Assert.Null(result.GanadorId);
            Assert.Empty(result.RestaurantesVotados);
        }


       
        [Fact]
        public async Task HandleAsync_ConGanadorClaro_RetornaGanador()
        {
            var firebaseUid = "uid";
            var grupoId = Guid.NewGuid();

            var usuario1 = new Usuario { Id = Guid.NewGuid(), FirebaseUid = firebaseUid };
            var usuario2 = new Usuario { Id = Guid.NewGuid(), FirebaseUid = "u2" };
            var usuario3 = new Usuario { Id = Guid.NewGuid(), FirebaseUid = "u3" };

            var ganadorId = Guid.NewGuid();
            var otroId = Guid.NewGuid();

            var restauranteGanador = new Restaurante { Id = ganadorId, Nombre = "Ganador" };
            var restaurante2 = new Restaurante { Id = otroId, Nombre = "Otro" };

            var grupo = new Grupo("Test", usuario1.Id) { Id = grupoId };

            grupo.Miembros.Add(new MiembroGrupo(grupoId, usuario1.Id) { afectarRecomendacion = true });
            grupo.Miembros.Add(new MiembroGrupo(grupoId, usuario2.Id) { afectarRecomendacion = true });
            grupo.Miembros.Add(new MiembroGrupo(grupoId, usuario3.Id) { afectarRecomendacion = true });

            var votacion = new VotacionGrupo(grupoId) { Grupo = grupo };

            votacion.Votos.Add(new VotoRestaurante(votacion.Id, usuario1.Id, ganadorId, null) { Restaurante = restauranteGanador, Usuario = usuario1 });
            votacion.Votos.Add(new VotoRestaurante(votacion.Id, usuario2.Id, ganadorId, null) { Restaurante = restauranteGanador, Usuario = usuario2 });
            votacion.Votos.Add(new VotoRestaurante(votacion.Id, usuario3.Id, otroId, null) { Restaurante = restaurante2, Usuario = usuario3 });

            _mockUsuarioRepository.Setup(r => r.GetByFirebaseUidAsync(firebaseUid, default)).ReturnsAsync(usuario1);
            _mockVotacionRepository.Setup(r =>
                r.ObtenerPorIdConCandidatosAsync(votacion.Id, default)).ReturnsAsync(votacion);
            _mockGrupoRepository.Setup(r =>
                r.UsuarioEsMiembroAsync(grupoId, firebaseUid, default)).ReturnsAsync(true);

            var result = await _useCase.HandleAsync(firebaseUid, votacion.Id);

            Assert.NotNull(result);
            Assert.Equal(ganadorId, result.GanadorId);
            Assert.False(result.HayEmpate);
            Assert.Equal(3, result.TotalVotos);
        }


       
        [Fact]
        public async Task HandleAsync_ConEmpate_NotificaEmpate()
        {
            var firebaseUid = "uid";
            var grupoId = Guid.NewGuid();

            var usuario1 = new Usuario { Id = Guid.NewGuid(), FirebaseUid = firebaseUid };
            var usuario2 = new Usuario { Id = Guid.NewGuid(), FirebaseUid = "u2" };

            var r1 = Guid.NewGuid();
            var r2 = Guid.NewGuid();

            var grupo = new Grupo("Test", usuario1.Id) { Id = grupoId };
            grupo.Miembros.Add(new MiembroGrupo(grupoId, usuario1.Id) { afectarRecomendacion = true });
            grupo.Miembros.Add(new MiembroGrupo(grupoId, usuario2.Id) { afectarRecomendacion = true });

            var votacion = new VotacionGrupo(grupoId) { Grupo = grupo };

            votacion.Votos.Add(new VotoRestaurante(votacion.Id, usuario1.Id, r1, null));
            votacion.Votos.Add(new VotoRestaurante(votacion.Id, usuario2.Id, r2, null));

            _mockUsuarioRepository.Setup(r => r.GetByFirebaseUidAsync(firebaseUid, default)).ReturnsAsync(usuario1);
            _mockVotacionRepository.Setup(r =>
                r.ObtenerPorIdConCandidatosAsync(votacion.Id, default)).ReturnsAsync(votacion);
            _mockGrupoRepository.Setup(r =>
                r.UsuarioEsMiembroAsync(grupoId, firebaseUid, default)).ReturnsAsync(true);

            var result = await _useCase.HandleAsync(firebaseUid, votacion.Id);

            Assert.True(result.HayEmpate);
            Assert.Contains(r1, result.RestaurantesEmpatados);
            Assert.Contains(r2, result.RestaurantesEmpatados);

            _mockNotificaciones.Verify(n =>
                n.NotificarEmpate(grupoId, votacion.Id),
                Times.Once);
        }


       
        [Fact]
        public async Task HandleAsync_ConGanadorRuleta_RetornaGanadorRuleta()
        {
            var firebaseUid = "uid";
            var usuario = new Usuario { Id = Guid.NewGuid(), FirebaseUid = firebaseUid };

            var grupoId = Guid.NewGuid();
            var ganador = Guid.NewGuid();

            var grupo = new Grupo("Test", usuario.Id) { Id = grupoId };
            grupo.Miembros.Add(new MiembroGrupo(grupoId, usuario.Id) { afectarRecomendacion = true });

            var votacion = new VotacionGrupo(grupoId) { Grupo = grupo };
            votacion.EstablecerGanadorRuleta(ganador);

            _mockUsuarioRepository.Setup(r => r.GetByFirebaseUidAsync(firebaseUid, default)).ReturnsAsync(usuario);
            _mockVotacionRepository.Setup(r =>
                r.ObtenerPorIdConCandidatosAsync(votacion.Id, default)).ReturnsAsync(votacion);
            _mockGrupoRepository.Setup(r =>
                r.UsuarioEsMiembroAsync(grupoId, firebaseUid, default)).ReturnsAsync(true);

            var result = await _useCase.HandleAsync(firebaseUid, votacion.Id);

            Assert.Equal(ganador, result.GanadorId);
            Assert.False(result.HayEmpate);
            _mockNotificaciones.Verify(n =>
                n.NotificarEmpate(It.IsAny<Guid>(), It.IsAny<Guid>()),
                Times.Never);
        }
    }

}
