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
using System.Reflection;
using FluentAssertions;

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

        private readonly Mock<IChatRealTimeService> _chatRealtimeServiceMock;
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
            _chatRealtimeServiceMock = new Mock<IChatRealTimeService>();

            _sut = new AceptarInvitacionGrupoUseCase(
                _invitacionRepositoryMock.Object,
                _usuarioRepositoryMock.Object,
                _miembroGrupoRepositoryMock.Object,
                _gustosGrupoRepositoryMock.Object,
                _eliminarNotificacionUseCase,
                _notificacionRealtimeServiceMock.Object,
                _chatRealtimeServiceMock.Object,
                _grupoRepositoryMock.Object 
            ); 
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
            type.GetProperty("FechaExpiracion")?.SetValue(invitacion, DateTime.UtcNow.AddDays(1));

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

        [Fact]
        public async Task HandleAsync_InvitacionExpirada_LanzaInvalidOperationException()
        {
            var ct = CancellationToken.None;
            var usuario = CreateUsuario(id: Guid.NewGuid());
            var grupoId = Guid.NewGuid();

            var invitacion = CreateInvitacion(grupoId, usuario.Id, EstadoInvitacion.Pendiente);

            // forzar expiración
            var type = typeof(InvitacionGrupo);
            type.GetMethod("MarcarComoExpirada", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.Invoke(invitacion, null);

            _usuarioRepositoryMock.Setup(r => r.GetByFirebaseUidAsync(usuario.FirebaseUid, ct))
                .ReturnsAsync(usuario);

            _invitacionRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), ct))
                .ReturnsAsync(invitacion);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _sut.HandleAsync(usuario.FirebaseUid, Guid.NewGuid(), ct));

            Assert.Equal("Error al aceptar la invitación", ex.Message);
        }
        [Fact]
        public async Task HandleAsync_UsuarioYaMiembroActivo_LanzaArgumentException()
        {
            var ct = CancellationToken.None;
            var usuario = CreateUsuario();
            var invitacion = CreateInvitacion(Guid.NewGuid(), usuario.Id, EstadoInvitacion.Pendiente);

            _usuarioRepositoryMock.Setup(r => r.GetByFirebaseUidAsync(usuario.FirebaseUid, ct))
                .ReturnsAsync(usuario);

            _invitacionRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), ct))
                .ReturnsAsync(invitacion);

            _miembroGrupoRepositoryMock
                .Setup(r => r.UsuarioEsMiembroActivoAsync(invitacion.GrupoId, usuario.Id, ct))
                .ReturnsAsync(true);

            var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
                _sut.HandleAsync(usuario.FirebaseUid, Guid.NewGuid(), ct));

            Assert.Equal("Ya eres miembro de este grupo", ex.Message);
        }

        [Fact]
        public async Task HandleAsync_ReactivarMiembroExistente_FuncionaCorrectamente()
        {
            var ct = CancellationToken.None;
            var usuario = CreateUsuario();
            var grupoId = Guid.NewGuid();
            var invitacion = CreateInvitacion(grupoId, usuario.Id, EstadoInvitacion.Pendiente);
            invitacion.NotificacionId = null;

            var miembroExistente = new MiembroGrupo(grupoId, usuario.Id);
            miembroExistente.AbandonarGrupo(); // para que Reincorporar cambie estado

            _usuarioRepositoryMock.Setup(r => r.GetByFirebaseUidAsync(usuario.FirebaseUid, ct))
                .ReturnsAsync(usuario);

            _invitacionRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), ct))
                .ReturnsAsync(invitacion);

            _miembroGrupoRepositoryMock.Setup(r =>
                r.UsuarioEsMiembroActivoAsync(grupoId, usuario.Id, ct))
                .ReturnsAsync(false);

            _miembroGrupoRepositoryMock.Setup(r =>
                r.GetByGrupoYUsuarioAsync(grupoId, usuario.IdUsuario, ct))
                .ReturnsAsync(miembroExistente);

            _invitacionRepositoryMock.Setup(r => r.UpdateAsync(invitacion, ct))
                  .ReturnsAsync(invitacion);
            _miembroGrupoRepositoryMock.Setup(r => r.UpdateAsync(miembroExistente, ct))
                .ReturnsAsync(miembroExistente);

            _grupoRepositoryMock.Setup(r => r.GetByIdAsync(grupoId, ct))
                .ReturnsAsync(new Grupo("G", usuario.Id));

            var grupo = await _sut.HandleAsync(usuario.FirebaseUid, Guid.NewGuid(), ct);

            miembroExistente.Activo.Should().BeTrue();
            _miembroGrupoRepositoryMock.Verify(r => r.UpdateAsync(miembroExistente, ct), Times.Once);
            _miembroGrupoRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<MiembroGrupo>(), It.IsAny<CancellationToken>()), Times.Never);
            _gustosGrupoRepositoryMock.Verify(r =>
                r.AgregarGustosAlGrupo(It.IsAny<Guid>(), It.IsAny<List<Gusto>>(), It.IsAny<Guid>()),
                Times.Never);
        }
        [Fact]
        public async Task HandleAsync_CrearNuevoMiembroYAgregarGustos_FuncionaCorrectamente()
        {
            var ct = CancellationToken.None;
            var usuario = CreateUsuario();
            var grupoId = Guid.NewGuid();
            var invitacion = CreateInvitacion(grupoId, usuario.Id, EstadoInvitacion.Pendiente);

            _usuarioRepositoryMock.Setup(r => r.GetByFirebaseUidAsync(usuario.FirebaseUid, ct))
                .ReturnsAsync(usuario);

            _invitacionRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), ct))
                .ReturnsAsync(invitacion);

            _miembroGrupoRepositoryMock.Setup(r =>
                r.UsuarioEsMiembroActivoAsync(grupoId, usuario.Id, ct))
                .ReturnsAsync(false);

            _miembroGrupoRepositoryMock.Setup(r =>
                r.GetByGrupoYUsuarioAsync(grupoId, usuario.IdUsuario, ct))
                .ReturnsAsync((MiembroGrupo)null); // NO existe → crear nuevo

            _grupoRepositoryMock.Setup(r => r.GetByIdAsync(grupoId, ct))
                .ReturnsAsync(new Grupo("Grupo", usuario.Id));

            MiembroGrupo miembroCreado = null!;
            _miembroGrupoRepositoryMock.Setup(r =>
                r.CreateAsync(It.IsAny<MiembroGrupo>(), ct))
                .Callback<MiembroGrupo, CancellationToken>((m, c) => miembroCreado = m)
                 .ReturnsAsync((MiembroGrupo)null!); 

            _gustosGrupoRepositoryMock.Setup(r =>
                r.AgregarGustosAlGrupo(grupoId, usuario.Gustos.ToList(), It.IsAny<Guid>()))
                 .ReturnsAsync(true);

            await _sut.HandleAsync(usuario.FirebaseUid, Guid.NewGuid(), ct);

            miembroCreado.Should().NotBeNull();
            miembroCreado.UsuarioId.Should().Be(usuario.Id);

            _gustosGrupoRepositoryMock.Verify(r =>
                r.AgregarGustosAlGrupo(grupoId, usuario.Gustos.ToList(), miembroCreado.Id),
                Times.Once);
        }
        [Fact]
        public async Task HandleAsync_ConNotificacion_EliminaYEnviaRealtime()
        {
            var ct = CancellationToken.None;
            var usuario = CreateUsuario();
            var grupoId = Guid.NewGuid();
            var invitacion = CreateInvitacion(grupoId, usuario.Id, EstadoInvitacion.Pendiente);
            invitacion.NotificacionId = Guid.NewGuid();

            _usuarioRepositoryMock.Setup(r => r.GetByFirebaseUidAsync(usuario.FirebaseUid, ct))
                .ReturnsAsync(usuario);

            _invitacionRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), ct))
                .ReturnsAsync(invitacion);

            _miembroGrupoRepositoryMock.Setup(r =>
                r.UsuarioEsMiembroActivoAsync(grupoId, usuario.Id, ct))
                .ReturnsAsync(false);

            _grupoRepositoryMock.Setup(r => r.GetByIdAsync(grupoId, ct))
                .ReturnsAsync(new Grupo("G", usuario.Id));

            await _sut.HandleAsync(usuario.FirebaseUid, Guid.NewGuid(), ct);
            _notificacionRealtimeServiceMock.Verify(r =>
                r.EnviarNotificacionAsync(
                    usuario.FirebaseUid,
                    "NotificacionEliminada",
                    invitacion.NotificacionId.Value.ToString(),
                    "InvitacionGrupoEliminada",
                    ct,
                    invitacion.NotificacionId.Value,
                    null),
                Times.Once);
        }
        [Fact]
        public async Task HandleAsync_GrupoNullDespuesDeFetch_LanzaInvalidOperation()
        {
            var usuario = CreateUsuario();
            var invitacion = CreateInvitacion(Guid.NewGuid(), usuario.Id, EstadoInvitacion.Pendiente);

            _usuarioRepositoryMock.Setup(r => r.GetByFirebaseUidAsync(usuario.FirebaseUid, default))
                .ReturnsAsync(usuario);

            _invitacionRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), default))
                .ReturnsAsync(invitacion);

            _miembroGrupoRepositoryMock
                .Setup(r => r.UsuarioEsMiembroActivoAsync(It.IsAny<Guid>(), usuario.Id, default))
                .ReturnsAsync(false);

            _miembroGrupoRepositoryMock
                .Setup(r => r.GetByGrupoYUsuarioAsync(It.IsAny<Guid>(), usuario.IdUsuario, default))
                .ReturnsAsync((MiembroGrupo)null);

            // Grupo NO viene en invitación y repository devuelve null → error
            _grupoRepositoryMock.Setup(r =>
                r.GetByIdAsync(invitacion.GrupoId, default))
                .ReturnsAsync((Grupo)null!);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _sut.HandleAsync(usuario.FirebaseUid, Guid.NewGuid()));

            ex.Message.Should().Be("Error al aceptar la invitación");
        }
        [Fact]
        public async Task HandleAsync_LlamaAChatRealtime_UsuarioSeUnio()
        {
            var ct = CancellationToken.None;
            var usuario = CreateUsuario();
            var grupoId = Guid.NewGuid();

            var invitacion = CreateInvitacion(grupoId, usuario.Id, EstadoInvitacion.Pendiente);

            _usuarioRepositoryMock.Setup(r => r.GetByFirebaseUidAsync(usuario.FirebaseUid, ct))
                .ReturnsAsync(usuario);

            _invitacionRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), ct))
                .ReturnsAsync(invitacion);

            _miembroGrupoRepositoryMock.Setup(r =>
                r.UsuarioEsMiembroActivoAsync(grupoId, usuario.Id, ct))
                .ReturnsAsync(false);

            _miembroGrupoRepositoryMock.Setup(r =>
                r.GetByGrupoYUsuarioAsync(grupoId, usuario.IdUsuario, ct))
                .ReturnsAsync((MiembroGrupo)null);

            _grupoRepositoryMock.Setup(r => r.GetByIdAsync(grupoId, ct))
                .ReturnsAsync(new Grupo("G", usuario.Id));

            await _sut.HandleAsync(usuario.FirebaseUid, Guid.NewGuid(), ct);

            _chatRealtimeServiceMock.Verify(r =>
                r.UsuarioSeUnio(
                    grupoId,
                    usuario.Id,
                    usuario.IdUsuario,
                    usuario.FotoPerfilUrl),
                Times.Once);
        }

    }
}
