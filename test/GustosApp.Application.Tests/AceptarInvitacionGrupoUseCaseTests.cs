using System;
using System.Threading;
using System.Threading.Tasks;
using GustosApp.Application.UseCases.GrupoUseCases.InvitacionGrupoUseCases;
using GustosApp.Application.UseCases.NotificacionUseCases;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using GustosApp.Domain.Model.@enum;
using Moq;
using Xunit;
using GustosApp.Application.Interfaces;

namespace GustosApp.Application.Tests
{
    public class AceptarInvitacionGrupoUseCaseTests
    {
        private readonly Mock<IInvitacionGrupoRepository> _invitacionRepositoryMock;
        private readonly Mock<IUsuarioRepository> _usuarioRepositoryMock;
        private readonly Mock<IMiembroGrupoRepository> _miembroGrupoRepositoryMock;
        private readonly Mock<IGustosGrupoRepository> _gustosGrupoRepositoryMock;
        private readonly Mock<INotificacionRealtimeService> _notificacionRealtimeServiceMock;
        private readonly Mock<IGrupoRepository> _grupoRepositoryMock;
        private readonly Mock<INotificacionRepository> _notificacionRepositoryMock;
        private readonly EliminarNotificacionUseCase _eliminarNotificacionUseCase;
        private readonly AceptarInvitacionGrupoUseCase _sut;

        public AceptarInvitacionGrupoUseCaseTests()
        {
            _invitacionRepositoryMock = new Mock<IInvitacionGrupoRepository>();
            _usuarioRepositoryMock = new Mock<IUsuarioRepository>();
            _miembroGrupoRepositoryMock = new Mock<IMiembroGrupoRepository>();
            _gustosGrupoRepositoryMock = new Mock<IGustosGrupoRepository>();
            _notificacionRealtimeServiceMock = new Mock<INotificacionRealtimeService>();
            _grupoRepositoryMock = new Mock<IGrupoRepository>();
            _notificacionRepositoryMock = new Mock<INotificacionRepository>();

            _eliminarNotificacionUseCase = new EliminarNotificacionUseCase(_notificacionRepositoryMock.Object);

            _sut = new AceptarInvitacionGrupoUseCase(
                _invitacionRepositoryMock.Object,
                _usuarioRepositoryMock.Object,
                _miembroGrupoRepositoryMock.Object,
                _gustosGrupoRepositoryMock.Object,
                _eliminarNotificacionUseCase,
                _notificacionRealtimeServiceMock.Object,
                _grupoRepositoryMock.Object);
        }

        private static Usuario CreateUsuario(Guid? id = null, string firebaseUid = "firebase-uid", string idUsuario = "usuario1")
        {
            var type = typeof(Usuario);
            var usuario = (Usuario)Activator.CreateInstance(type, nonPublic: true)!;

            type.GetProperty("Id")?.SetValue(usuario, id ?? Guid.NewGuid());
            type.GetProperty("FirebaseUid")?.SetValue(usuario, firebaseUid);
            type.GetProperty("IdUsuario")?.SetValue(usuario, idUsuario);

            return usuario;
        }

        private static InvitacionGrupo CreateInvitacion(Guid grupoId, Guid usuarioInvitadoId, EstadoInvitacion estado)
        {
            var type = typeof(InvitacionGrupo);
            var invitacion = (InvitacionGrupo)Activator.CreateInstance(type, nonPublic: true)!;

            type.GetProperty("GrupoId")?.SetValue(invitacion, grupoId);
            type.GetProperty("UsuarioInvitadoId")?.SetValue(invitacion, usuarioInvitadoId);
            type.GetProperty("Estado")?.SetValue(invitacion, estado);

            return invitacion;
        }

        // Verifica que al no encontrar el usuario se lanza UnauthorizedAccessException con el mensaje correcto.
        [Fact]
        public async Task HandleAsync_UsuarioNoEncontrado_LanzaUnauthorizedAccessException()
        {
            var firebaseUid = "uid-inexistente";
            var invitacionId = Guid.NewGuid();
            var ct = CancellationToken.None;

            _usuarioRepositoryMock
                .Setup(r => r.GetByFirebaseUidAsync(firebaseUid, ct))
                .ReturnsAsync((Usuario?)null);

            var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _sut.HandleAsync(firebaseUid, invitacionId, ct));

            Assert.Equal("Usuario no encontrado", ex.Message);

            _invitacionRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        // Verifica que al no encontrar la invitación se lanza ArgumentException con el mensaje correcto.
        [Fact]
        public async Task HandleAsync_InvitacionNoEncontrada_LanzaArgumentException()
        {
            var firebaseUid = "uid-valido";
            var invitacionId = Guid.NewGuid();
            var ct = CancellationToken.None;

            var usuario = CreateUsuario();

            _usuarioRepositoryMock
                .Setup(r => r.GetByFirebaseUidAsync(firebaseUid, ct))
                .ReturnsAsync(usuario);

            _invitacionRepositoryMock
                .Setup(r => r.GetByIdAsync(invitacionId, ct))
                .ReturnsAsync((InvitacionGrupo?)null);

            var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
                _sut.HandleAsync(firebaseUid, invitacionId, ct));

            Assert.Equal("Invitación no encontrada", ex.Message);

            _miembroGrupoRepositoryMock.Verify(r =>
                r.UsuarioEsMiembroActivoAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        // Verifica que cuando la invitación no pertenece al usuario se lanza UnauthorizedAccessException.
        [Fact]
        public async Task HandleAsync_InvitacionNoEsParaUsuario_LanzaUnauthorizedAccessException()
        {
            var firebaseUid = "uid-valido";
            var invitacionId = Guid.NewGuid();
            var ct = CancellationToken.None;

            var usuario = CreateUsuario(id: Guid.NewGuid());
            var grupoId = Guid.NewGuid();
            var otroUsuarioId = Guid.NewGuid();

            var invitacion = CreateInvitacion(grupoId, otroUsuarioId, EstadoInvitacion.Pendiente);

            _usuarioRepositoryMock
                .Setup(r => r.GetByFirebaseUidAsync(firebaseUid, ct))
                .ReturnsAsync(usuario);

            _invitacionRepositoryMock
                .Setup(r => r.GetByIdAsync(invitacionId, ct))
                .ReturnsAsync(invitacion);

            var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _sut.HandleAsync(firebaseUid, invitacionId, ct));

            Assert.Equal("Esta invitación no es para ti", ex.Message);

            _miembroGrupoRepositoryMock.Verify(r =>
                r.UsuarioEsMiembroActivoAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        // Verifica que cuando la invitación no está pendiente se lanza ArgumentException.
        [Fact]
        public async Task HandleAsync_InvitacionNoPendiente_LanzaArgumentException()
        {
            var firebaseUid = "uid-valido";
            var invitacionId = Guid.NewGuid();
            var ct = CancellationToken.None;

            var usuarioId = Guid.NewGuid();
            var usuario = CreateUsuario(id: usuarioId);
            var grupoId = Guid.NewGuid();

            var invitacion = CreateInvitacion(grupoId, usuarioId, EstadoInvitacion.Aceptada);

            _usuarioRepositoryMock
                .Setup(r => r.GetByFirebaseUidAsync(firebaseUid, ct))
                .ReturnsAsync(usuario);

            _invitacionRepositoryMock
                .Setup(r => r.GetByIdAsync(invitacionId, ct))
                .ReturnsAsync(invitacion);

            var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
                _sut.HandleAsync(firebaseUid, invitacionId, ct));

            Assert.Equal("Esta invitación ya fue procesada", ex.Message);

            _miembroGrupoRepositoryMock.Verify(r =>
                r.UsuarioEsMiembroActivoAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }
    }
}
