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
    public class RegistrarVotoUseCaseTests
    {
        private readonly Mock<IVotacionRepository> _mockVotacionRepository;
        private readonly Mock<IUsuarioRepository> _mockUsuarioRepository;
        private readonly Mock<IGrupoRepository> _mockGrupoRepository;
        private readonly Mock<IRestauranteRepository> _mockRestauranteRepository;
        private readonly Mock<INotificacionesVotacionService> _mockNotificaciones;
        private readonly RegistrarVotoUseCase _useCase;

        public RegistrarVotoUseCaseTests()
        {
            _mockVotacionRepository = new Mock<IVotacionRepository>();
            _mockUsuarioRepository = new Mock<IUsuarioRepository>();
            _mockGrupoRepository = new Mock<IGrupoRepository>();
            _mockRestauranteRepository = new Mock<IRestauranteRepository>();
            _mockNotificaciones = new Mock<INotificacionesVotacionService>();

            _useCase = new RegistrarVotoUseCase(
                _mockVotacionRepository.Object,
                _mockUsuarioRepository.Object,
                _mockGrupoRepository.Object,
                _mockRestauranteRepository.Object,
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
                _useCase.HandleAsync(firebaseUid, Guid.NewGuid(), Guid.NewGuid()));
        }


      
        [Fact]
        public async Task HandleAsync_VotacionNoEncontrada_LanzaArgumentException()
        {
            var firebaseUid = "uid";
            var usuario = new Usuario { Id = Guid.NewGuid(), FirebaseUid = firebaseUid };

            _mockUsuarioRepository
                .Setup(r => r.GetByFirebaseUidAsync(firebaseUid, default))
                .ReturnsAsync(usuario);

            _mockVotacionRepository
                .Setup(r => r.ObtenerPorIdConCandidatosAsync(It.IsAny<Guid>(), default))
                .ReturnsAsync((VotacionGrupo?)null);

            var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
                _useCase.HandleAsync(firebaseUid, Guid.NewGuid(), Guid.NewGuid()));

            Assert.Equal("Votación no encontrada", ex.Message);
        }


      
        [Fact]
        public async Task HandleAsync_VotacionNoActiva_LanzaInvalidOperationException()
        {
            var firebaseUid = "uid";
            var usuario = new Usuario { Id = Guid.NewGuid(), FirebaseUid = firebaseUid };

            var grupo = new Grupo("Test", usuario.Id) { Id = Guid.NewGuid() };
            var votacion = new VotacionGrupo(grupo.Id) { Grupo = grupo };
            votacion.CerrarVotacion();

            _mockUsuarioRepository.Setup(r => r.GetByFirebaseUidAsync(firebaseUid, default))
                .ReturnsAsync(usuario);

            _mockVotacionRepository
                .Setup(r => r.ObtenerPorIdConCandidatosAsync(It.IsAny<Guid>(), default))
                .ReturnsAsync(votacion);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _useCase.HandleAsync(firebaseUid, Guid.NewGuid(), Guid.NewGuid()));

            Assert.Equal("La votación no está activa", ex.Message);
        }


       
        [Fact]
        public async Task HandleAsync_UsuarioNoEsMiembro_LanzaUnauthorizedAccessException()
        {
            var firebaseUid = "uid";
            var usuario = new Usuario { Id = Guid.NewGuid(), FirebaseUid = firebaseUid };

            var grupo = new Grupo("Test", usuario.Id) { Id = Guid.NewGuid() };
            var votacion = new VotacionGrupo(grupo.Id) { Grupo = grupo };

            // Sin miembros → usuario NO es miembro
            grupo.Miembros.Clear();

            _mockUsuarioRepository.Setup(r => r.GetByFirebaseUidAsync(firebaseUid, default))
                .ReturnsAsync(usuario);

            _mockVotacionRepository.Setup(r => r.ObtenerPorIdConCandidatosAsync(It.IsAny<Guid>(), default))
                .ReturnsAsync(votacion);

            var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _useCase.HandleAsync(firebaseUid, Guid.NewGuid(), Guid.NewGuid()));

            Assert.Equal("No eres miembro de este grupo", ex.Message);
        }


      
        [Fact]
        public async Task HandleAsync_MiembroNoAfectaRecomendacion_LanzaInvalidOperationException()
        {
            var firebaseUid = "uid";
            var usuario = new Usuario { Id = Guid.NewGuid(), FirebaseUid = firebaseUid };
            var grupoId = Guid.NewGuid();

            var grupo = new Grupo("TestGrupo", usuario.Id) { Id = grupoId };
            var miembro = new MiembroGrupo(grupoId, usuario.Id);
            miembro.afectarRecomendacion = false; // NO participa
            grupo.Miembros.Add(miembro);

            var votacion = new VotacionGrupo(grupoId) { Grupo = grupo };

            _mockUsuarioRepository.Setup(r => r.GetByFirebaseUidAsync(firebaseUid, default))
                .ReturnsAsync(usuario);

            _mockVotacionRepository.Setup(r => r.ObtenerPorIdConCandidatosAsync(It.IsAny<Guid>(), default))
                .ReturnsAsync(votacion);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _useCase.HandleAsync(firebaseUid, Guid.NewGuid(), Guid.NewGuid()));

            Assert.Equal("No puedes votar porque no estás marcado para asistir a la reunión", ex.Message);
        }


       
        [Fact]
        public async Task HandleAsync_RestauranteNoEsCandidato_LanzaInvalidOperationException()
        {
            var firebaseUid = "uid";
            var usuario = new Usuario { Id = Guid.NewGuid(), FirebaseUid = firebaseUid };
            var grupoId = Guid.NewGuid();

            var grupo = new Grupo("Test", usuario.Id) { Id = grupoId };
            var miembro = new MiembroGrupo(grupoId, usuario.Id);
            grupo.Miembros.Add(miembro);

            var votacion = new VotacionGrupo(grupoId) { Grupo = grupo };
            votacion.RestaurantesCandidatos.Clear(); // ningún candidato

            _mockUsuarioRepository.Setup(r => r.GetByFirebaseUidAsync(firebaseUid, default))
                .ReturnsAsync(usuario);

            _mockVotacionRepository.Setup(r => r.ObtenerPorIdConCandidatosAsync(It.IsAny<Guid>(), default))
                .ReturnsAsync(votacion);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _useCase.HandleAsync(firebaseUid, Guid.NewGuid(), Guid.NewGuid()));

            Assert.Equal("Este restaurante no es candidato en esta votación.", ex.Message);
        }


       
        [Fact]
        public async Task HandleAsync_RestauranteNoExiste_LanzaArgumentException()
        {
            var firebaseUid = "uid";
            var usuario = new Usuario { Id = Guid.NewGuid(), FirebaseUid = firebaseUid };
            var grupoId = Guid.NewGuid();
            var candidato = Guid.NewGuid();

            var grupo = new Grupo("Test", usuario.Id) { Id = grupoId };
            var miembro = new MiembroGrupo(grupoId, usuario.Id);
            grupo.Miembros.Add(miembro);

            var votacion = new VotacionGrupo(grupoId) { Grupo = grupo };
            votacion.RestaurantesCandidatos.Add(new VotacionRestaurante(votacion.Id, candidato));

            _mockUsuarioRepository.Setup(r => r.GetByFirebaseUidAsync(firebaseUid, default))
                .ReturnsAsync(usuario);

            _mockVotacionRepository.Setup(r => r.ObtenerPorIdConCandidatosAsync(It.IsAny<Guid>(), default))
                .ReturnsAsync(votacion);

            _mockRestauranteRepository
                .Setup(r => r.GetRestauranteByIdAsync(candidato, default))
                .ReturnsAsync((Restaurante?)null);

            var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
                _useCase.HandleAsync(firebaseUid, Guid.NewGuid(), candidato));

            Assert.Equal("Restaurante no encontrado", ex.Message);
        }


        
        [Fact]
        public async Task HandleAsync_NuevoVoto_RegistraYNotifica()
        {
            var firebaseUid = "uid";
            var usuario = new Usuario { Id = Guid.NewGuid(), FirebaseUid = firebaseUid };
            var grupoId = Guid.NewGuid();
            var restauranteId = Guid.NewGuid();
            var comentario = "Buen lugar";

            var grupo = new Grupo("Test", usuario.Id) { Id = grupoId };
            var miembro = new MiembroGrupo(grupoId, usuario.Id);
            miembro.afectarRecomendacion = true;
            grupo.Miembros.Add(miembro);

            var votacion = new VotacionGrupo(grupoId) { Grupo = grupo };
            votacion.RestaurantesCandidatos.Add(new VotacionRestaurante(votacion.Id, restauranteId));

            var restaurante = new Restaurante { Id = restauranteId, Nombre = "Rest Test" };

            _mockUsuarioRepository.Setup(r => r.GetByFirebaseUidAsync(firebaseUid, default))
                .ReturnsAsync(usuario);

            _mockVotacionRepository.Setup(r => r.ObtenerPorIdConCandidatosAsync(votacion.Id, default))
                .ReturnsAsync(votacion);

            _mockRestauranteRepository.Setup(r => r.GetRestauranteByIdAsync(restauranteId, default))
                .ReturnsAsync(restaurante);

            _mockVotacionRepository.Setup(r =>
                r.ObtenerVotoUsuarioAsync(votacion.Id, usuario.Id, default))
                .ReturnsAsync((VotoRestaurante?)null);

            _mockVotacionRepository.Setup(r =>
                r.RegistrarVotoAsync(It.IsAny<VotoRestaurante>(), default))
                .ReturnsAsync(new VotoRestaurante(votacion.Id, usuario.Id, restauranteId, comentario));

            var result = await _useCase.HandleAsync(firebaseUid, votacion.Id, restauranteId, comentario);

            Assert.NotNull(result);

            _mockVotacionRepository.Verify(r =>
                r.RegistrarVotoAsync(It.IsAny<VotoRestaurante>(), default),
                Times.Once);

            _mockNotificaciones.Verify(n =>
                n.NotificarVotoRegistrado(grupoId, votacion.Id), Times.Once);

            _mockNotificaciones.Verify(n =>
                n.NotificarResultadosActualizados(grupoId, votacion.Id), Times.Once);
        }


       
        [Fact]
        public async Task HandleAsync_VotoExistente_ActualizaYNotifica()
        {
            var firebaseUid = "uid";
            var usuario = new Usuario { Id = Guid.NewGuid(), FirebaseUid = firebaseUid };
            var grupoId = Guid.NewGuid();
            var restauranteNuevo = Guid.NewGuid();

            var grupo = new Grupo("Test", usuario.Id) { Id = grupoId };
            var miembro = new MiembroGrupo(grupoId, usuario.Id);
            grupo.Miembros.Add(miembro);

            var votacion = new VotacionGrupo(grupoId) { Grupo = grupo };
            votacion.RestaurantesCandidatos.Add(new VotacionRestaurante(votacion.Id, restauranteNuevo));

            var votoExistente = new VotoRestaurante(votacion.Id, usuario.Id, Guid.NewGuid(), "viejo");

            var restaurante = new Restaurante { Id = restauranteNuevo, Nombre = "Nuevo" };

            _mockUsuarioRepository.Setup(r => r.GetByFirebaseUidAsync(firebaseUid, default))
                .ReturnsAsync(usuario);

            _mockVotacionRepository.Setup(r => r.ObtenerPorIdConCandidatosAsync(votacion.Id, default))
                .ReturnsAsync(votacion);

            _mockRestauranteRepository.Setup(r => r.GetRestauranteByIdAsync(restauranteNuevo, default))
                .ReturnsAsync(restaurante);

            _mockVotacionRepository.Setup(r =>
                r.ObtenerVotoUsuarioAsync(votacion.Id, usuario.Id, default))
                .ReturnsAsync(votoExistente);

            _mockVotacionRepository.Setup(r =>
                r.ActualizarVotoAsync(votoExistente, default))
                .Returns(Task.CompletedTask);

            var result = await _useCase.HandleAsync(firebaseUid, votacion.Id, restauranteNuevo, "nuevo");

            Assert.Equal(votoExistente, result);

            _mockNotificaciones.Verify(n =>
                n.NotificarVotoRegistrado(grupoId, votacion.Id), Times.Once);

            _mockNotificaciones.Verify(n =>
                n.NotificarResultadosActualizados(grupoId, votacion.Id), Times.Once);

            _mockVotacionRepository.Verify(r =>
                r.ActualizarVotoAsync(votoExistente, default), Times.Once);
        }
    }

}
