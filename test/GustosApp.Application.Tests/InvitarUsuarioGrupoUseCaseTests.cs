using System;
using System.Threading;
using System.Threading.Tasks;
using GustosApp.Application.Interfaces;
using GustosApp.Application.UseCases.GrupoUseCases.InvitacionGrupoUseCases;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using GustosApp.Domain.Model.@enum;
using Moq;
using Xunit;

namespace GustosApp.Application.Tests
{
    public class InvitarUsuarioGrupoUseCaseTests
    {
        private readonly Mock<IGrupoRepository> _grupoRepositoryMock;
        private readonly Mock<IUsuarioRepository> _usuarioRepositoryMock;
        private readonly Mock<IInvitacionGrupoRepository> _invitacionRepositoryMock;
        private readonly Mock<IMiembroGrupoRepository> _miembroGrupoRepositoryMock;
        private readonly Mock<INotificacionRepository> _notificacionRepositoryMock;
        private readonly Mock<INotificacionRealtimeService> _notificacionRealtimeServiceMock;
        private readonly InvitarUsuarioGrupoUseCase _sut;

        public InvitarUsuarioGrupoUseCaseTests()
        {
            _grupoRepositoryMock = new Mock<IGrupoRepository>();
            _usuarioRepositoryMock = new Mock<IUsuarioRepository>();
            _invitacionRepositoryMock = new Mock<IInvitacionGrupoRepository>();
            _miembroGrupoRepositoryMock = new Mock<IMiembroGrupoRepository>();
            _notificacionRepositoryMock = new Mock<INotificacionRepository>();
            _notificacionRealtimeServiceMock = new Mock<INotificacionRealtimeService>();

            _sut = new InvitarUsuarioGrupoUseCase(
                _grupoRepositoryMock.Object,
                _usuarioRepositoryMock.Object,
                _invitacionRepositoryMock.Object,
                _miembroGrupoRepositoryMock.Object,
                _notificacionRepositoryMock.Object,
                _notificacionRealtimeServiceMock.Object);
        }

        private static Usuario CreateUsuario(Guid? id = null, string firebaseUid = "firebase-uid", string username = "usuario1", string email = "user@test.com", string nombre = "Usuario Nombre")
        {
            var type = typeof(Usuario);
            var usuario = (Usuario)Activator.CreateInstance(type, nonPublic: true)!;

            type.GetProperty("Id")?.SetValue(usuario, id ?? Guid.NewGuid());
            type.GetProperty("FirebaseUid")?.SetValue(usuario, firebaseUid);
            type.GetProperty("IdUsuario")?.SetValue(usuario, username);
            type.GetProperty("Email")?.SetValue(usuario, email);
            type.GetProperty("Nombre")?.SetValue(usuario, nombre);

            return usuario;
        }

        private static Grupo CreateGrupo(Guid? id = null, string nombre = "Grupo prueba")
        {
            var type = typeof(Grupo);
            var grupo = (Grupo)Activator.CreateInstance(type, nonPublic: true)!;

            type.GetProperty("Id")?.SetValue(grupo, id ?? Guid.NewGuid());
            type.GetProperty("Nombre")?.SetValue(grupo, nombre);

            return grupo;
        }

        private static InvitacionGrupo CreateInvitacion(Guid? id = null, Guid? grupoId = null, Guid? usuarioInvitadoId = null, Guid? usuarioInvitadorId = null)
        {
            var type = typeof(InvitacionGrupo);
            var invitacion = (InvitacionGrupo)Activator.CreateInstance(type, nonPublic: true)!;

            if (id.HasValue)
                type.GetProperty("Id")?.SetValue(invitacion, id.Value);
            if (grupoId.HasValue)
                type.GetProperty("GrupoId")?.SetValue(invitacion, grupoId.Value);
            if (usuarioInvitadoId.HasValue)
                type.GetProperty("UsuarioInvitadoId")?.SetValue(invitacion, usuarioInvitadoId.Value);
            if (usuarioInvitadorId.HasValue)
                type.GetProperty("UsuarioInvitadorId")?.SetValue(invitacion, usuarioInvitadorId.Value);

            return invitacion;
        }

        // Verifica que si el usuario invitador no existe se lanza UnauthorizedAccessException con el mensaje correcto.
        [Fact]
        public async Task HandleAsync_UsuarioInvitadorNoEncontrado_LanzaUnauthorizedAccessException()
        {
            var firebaseUid = "uid-inexistente";
            var grupoId = Guid.NewGuid();
            var ct = CancellationToken.None;

            _usuarioRepositoryMock
                .Setup(r => r.GetByFirebaseUidAsync(firebaseUid, ct))
                .ReturnsAsync((Usuario?)null);

            var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _sut.HandleAsync(firebaseUid, grupoId, null, null, null, null, ct));

            Assert.Equal("Usuario no encontrado", ex.Message);

            _grupoRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        // Verifica que si el grupo no existe se lanza ArgumentException con el mensaje correcto.
        [Fact]
        public async Task HandleAsync_GrupoNoEncontrado_LanzaArgumentException()
        {
            var firebaseUid = "uid-valido";
            var grupoId = Guid.NewGuid();
            var ct = CancellationToken.None;

            var usuarioInvitador = CreateUsuario();

            _usuarioRepositoryMock
                .Setup(r => r.GetByFirebaseUidAsync(firebaseUid, ct))
                .ReturnsAsync(usuarioInvitador);

            _grupoRepositoryMock
                .Setup(r => r.GetByIdAsync(grupoId, ct))
                .ReturnsAsync((Grupo?)null);

            var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
                _sut.HandleAsync(firebaseUid, grupoId, null, null, null, null, ct));

            Assert.Equal("Grupo no encontrado", ex.Message);

            _grupoRepositoryMock.Verify(r => r.UsuarioEsAdministradorAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        // Verifica que si el usuario invitador no es administrador se lanza UnauthorizedAccessException.
        [Fact]
        public async Task HandleAsync_UsuarioNoEsAdmin_LanzaUnauthorizedAccessException()
        {
            var firebaseUid = "uid-valido";
            var grupoId = Guid.NewGuid();
            var ct = CancellationToken.None;

            var usuarioInvitador = CreateUsuario();
            var grupo = CreateGrupo(grupoId);

            _usuarioRepositoryMock
                .Setup(r => r.GetByFirebaseUidAsync(firebaseUid, ct))
                .ReturnsAsync(usuarioInvitador);

            _grupoRepositoryMock
                .Setup(r => r.GetByIdAsync(grupoId, ct))
                .ReturnsAsync(grupo);

            _grupoRepositoryMock
                .Setup(r => r.UsuarioEsAdministradorAsync(grupoId, usuarioInvitador.Id, ct))
                .ReturnsAsync(false);

            var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _sut.HandleAsync(firebaseUid, grupoId, null, null, null, null, ct));

            Assert.Equal("Solo los administradores pueden invitar usuarios", ex.Message);

            _usuarioRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        // Verifica que si no se encuentra ningún usuario invitado se lanza ArgumentException.
        [Fact]
        public async Task HandleAsync_UsuarioInvitadoNoEncontrado_LanzaArgumentException()
        {
            var firebaseUid = "uid-valido";
            var grupoId = Guid.NewGuid();
            var ct = CancellationToken.None;

            var usuarioInvitador = CreateUsuario();
            var grupo = CreateGrupo(grupoId);

            _usuarioRepositoryMock
                .Setup(r => r.GetByFirebaseUidAsync(firebaseUid, ct))
                .ReturnsAsync(usuarioInvitador);

            _grupoRepositoryMock
                .Setup(r => r.GetByIdAsync(grupoId, ct))
                .ReturnsAsync(grupo);

            _grupoRepositoryMock
                .Setup(r => r.UsuarioEsAdministradorAsync(grupoId, usuarioInvitador.Id, ct))
                .ReturnsAsync(true);

            _usuarioRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), ct))
                .ReturnsAsync((Usuario?)null);

            _usuarioRepositoryMock
                .Setup(r => r.GetByUsernameAsync(It.IsAny<string>(), ct))
                .ReturnsAsync((Usuario?)null);

            _usuarioRepositoryMock
                .Setup(r => r.GetByEmailAsync(It.IsAny<string>(), ct))
                .ReturnsAsync((Usuario?)null);

            var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
                _sut.HandleAsync(firebaseUid, grupoId, "mail@test.com", Guid.NewGuid(), "userX", "hola", ct));

            Assert.Equal("No se encontró el usuario a invitar", ex.Message);
        }

        // Verifica que si el usuario intenta invitarse a sí mismo se lanza ArgumentException.
        [Fact]
        public async Task HandleAsync_UsuarioSeInvitaASiMismo_LanzaArgumentException()
        {
            var firebaseUid = "uid-valido";
            var grupoId = Guid.NewGuid();
            var ct = CancellationToken.None;

            var usuarioId = Guid.NewGuid();
            var usuarioInvitador = CreateUsuario(id: usuarioId);
            var grupo = CreateGrupo(grupoId);

            _usuarioRepositoryMock
                .Setup(r => r.GetByFirebaseUidAsync(firebaseUid, ct))
                .ReturnsAsync(usuarioInvitador);

            _grupoRepositoryMock
                .Setup(r => r.GetByIdAsync(grupoId, ct))
                .ReturnsAsync(grupo);

            _grupoRepositoryMock
                .Setup(r => r.UsuarioEsAdministradorAsync(grupoId, usuarioInvitador.Id, ct))
                .ReturnsAsync(true);

            _usuarioRepositoryMock
                .Setup(r => r.GetByIdAsync(usuarioId, ct))
                .ReturnsAsync(usuarioInvitador);

            var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
                _sut.HandleAsync(firebaseUid, grupoId, null, usuarioId, null, null, ct));

            Assert.Equal("No puedes invitarte a ti mismo", ex.Message);

            _miembroGrupoRepositoryMock.Verify(r =>
                r.UsuarioEsMiembroActivoAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        // Verifica que si el usuario ya es miembro activo del grupo se lanza ArgumentException.
        [Fact]
        public async Task HandleAsync_UsuarioYaEsMiembro_LanzaArgumentException()
        {
            var firebaseUid = "uid-valido";
            var grupoId = Guid.NewGuid();
            var ct = CancellationToken.None;

            var usuarioInvitador = CreateUsuario();
            var usuarioInvitado = CreateUsuario();
            var grupo = CreateGrupo(grupoId);

            _usuarioRepositoryMock
                .Setup(r => r.GetByFirebaseUidAsync(firebaseUid, ct))
                .ReturnsAsync(usuarioInvitador);

            _grupoRepositoryMock
                .Setup(r => r.GetByIdAsync(grupoId, ct))
                .ReturnsAsync(grupo);

            _grupoRepositoryMock
                .Setup(r => r.UsuarioEsAdministradorAsync(grupoId, usuarioInvitador.Id, ct))
                .ReturnsAsync(true);

            _usuarioRepositoryMock
                .Setup(r => r.GetByIdAsync(usuarioInvitado.Id, ct))
                .ReturnsAsync(usuarioInvitado);

            _miembroGrupoRepositoryMock
                .Setup(r => r.UsuarioEsMiembroActivoAsync(grupoId, usuarioInvitado.Id, ct))
                .ReturnsAsync(true);

            var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
                _sut.HandleAsync(firebaseUid, grupoId, null, usuarioInvitado.Id, null, null, ct));

            Assert.Equal("El usuario ya es miembro del grupo", ex.Message);

            _invitacionRepositoryMock.Verify(r =>
                r.ExisteInvitacionPendienteAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        // Verifica que si ya existe una invitación pendiente se lanza ArgumentException.
        [Fact]
        public async Task HandleAsync_YaExisteInvitacionPendiente_LanzaArgumentException()
        {
            var firebaseUid = "uid-valido";
            var grupoId = Guid.NewGuid();
            var ct = CancellationToken.None;

            var usuarioInvitador = CreateUsuario();
            var usuarioInvitado = CreateUsuario();
            var grupo = CreateGrupo(grupoId);

            _usuarioRepositoryMock
                .Setup(r => r.GetByFirebaseUidAsync(firebaseUid, ct))
                .ReturnsAsync(usuarioInvitador);

            _grupoRepositoryMock
                .Setup(r => r.GetByIdAsync(grupoId, ct))
                .ReturnsAsync(grupo);

            _grupoRepositoryMock
                .Setup(r => r.UsuarioEsAdministradorAsync(grupoId, usuarioInvitador.Id, ct))
                .ReturnsAsync(true);

            _usuarioRepositoryMock
                .Setup(r => r.GetByIdAsync(usuarioInvitado.Id, ct))
                .ReturnsAsync(usuarioInvitado);

            _miembroGrupoRepositoryMock
                .Setup(r => r.UsuarioEsMiembroActivoAsync(grupoId, usuarioInvitado.Id, ct))
                .ReturnsAsync(false);

            _invitacionRepositoryMock
                .Setup(r => r.ExisteInvitacionPendienteAsync(grupoId, usuarioInvitado.Id, ct))
                .ReturnsAsync(true);

            var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
                _sut.HandleAsync(firebaseUid, grupoId, null, usuarioInvitado.Id, null, null, ct));

            Assert.Equal("Ya existe una invitación pendiente para este usuario", ex.Message);

            _notificacionRepositoryMock.Verify(r =>
                r.crearAsync(It.IsAny<Notificacion>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        // Verifica que cuando todo es válido se crea la invitación, la notificación y se devuelve la invitación completa.
        [Fact]
        public async Task HandleAsync_TodoValido_CreaInvitacionYNotificacionYDevuelveInvitacionCompleta()
        {
            var firebaseUid = "uid-valido";
            var grupoId = Guid.NewGuid();
            var ct = CancellationToken.None;

            var usuarioInvitador = CreateUsuario(nombre: "Admin Invitador");
            var usuarioInvitado = CreateUsuario(firebaseUid: "firebase-invitado");
            var grupo = CreateGrupo(grupoId, "Grupo de prueba");

            _usuarioRepositoryMock
                .Setup(r => r.GetByFirebaseUidAsync(firebaseUid, ct))
                .ReturnsAsync(usuarioInvitador);

            _grupoRepositoryMock
                .Setup(r => r.GetByIdAsync(grupoId, ct))
                .ReturnsAsync(grupo);

            _grupoRepositoryMock
                .Setup(r => r.UsuarioEsAdministradorAsync(grupoId, usuarioInvitador.Id, ct))
                .ReturnsAsync(true);

            _usuarioRepositoryMock
                .Setup(r => r.GetByIdAsync(usuarioInvitado.Id, ct))
                .ReturnsAsync(usuarioInvitado);

            _miembroGrupoRepositoryMock
                .Setup(r => r.UsuarioEsMiembroActivoAsync(grupoId, usuarioInvitado.Id, ct))
                .ReturnsAsync(false);

            _invitacionRepositoryMock
                .Setup(r => r.ExisteInvitacionPendienteAsync(grupoId, usuarioInvitado.Id, ct))
                .ReturnsAsync(false);

            _notificacionRepositoryMock
                .Setup(r => r.crearAsync(It.IsAny<Notificacion>(), ct))
                .Returns(Task.CompletedTask);

            var invitacionCompleta = CreateInvitacion(
                id: Guid.NewGuid(),
                grupoId: grupoId,
                usuarioInvitadoId: usuarioInvitado.Id,
                usuarioInvitadorId: usuarioInvitador.Id);

            _invitacionRepositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<InvitacionGrupo>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((InvitacionGrupo invitacion, CancellationToken _) => invitacion);


            _notificacionRepositoryMock
                .Setup(r => r.UpdateAsync(It.IsAny<Notificacion>(), ct))
                .Returns(Task.CompletedTask);

            _invitacionRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), ct))
                .ReturnsAsync(invitacionCompleta);

            _notificacionRealtimeServiceMock
                .Setup(s => s.EnviarNotificacionAsync(
                    usuarioInvitado.FirebaseUid,
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    ct,
                    It.IsAny<Guid>(),
                    It.IsAny<Guid>()))
                .Returns(Task.CompletedTask);

            var result = await _sut.HandleAsync(
                firebaseUid,
                grupoId,
                null,
                usuarioInvitado.Id,
                null,
                "Mensaje personalizado",
                ct);

            Assert.NotNull(result);
            Assert.Equal(invitacionCompleta, result);

            _notificacionRepositoryMock.Verify(
                r => r.crearAsync(It.IsAny<Notificacion>(), ct),
                Times.Once);

            _invitacionRepositoryMock.Verify(
                r => r.CreateAsync(It.IsAny<InvitacionGrupo>(), ct),
                Times.Once);

            _notificacionRepositoryMock.Verify(
                r => r.UpdateAsync(It.IsAny<Notificacion>(), ct),
                Times.Once);

            _notificacionRealtimeServiceMock.Verify(
                s => s.EnviarNotificacionAsync(
                    usuarioInvitado.FirebaseUid,
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    ct,
                    It.IsAny<Guid>(),
                    It.IsAny<Guid>()),
                Times.Once);

            _invitacionRepositoryMock.Verify(
                r => r.GetByIdAsync(It.IsAny<Guid>(), ct),
                Times.Once);
        }

        // Verifica que si falla la obtención de la invitación completa se lanza InvalidOperationException.
        [Fact]
        public async Task HandleAsync_ErrorObteniendoInvitacionCompleta_LanzaInvalidOperationException()
        {
            var firebaseUid = "uid-valido";
            var grupoId = Guid.NewGuid();
            var ct = CancellationToken.None;

            var usuarioInvitador = CreateUsuario();
            var usuarioInvitado = CreateUsuario();
            var grupo = CreateGrupo(grupoId);

            _usuarioRepositoryMock
                .Setup(r => r.GetByFirebaseUidAsync(firebaseUid, ct))
                .ReturnsAsync(usuarioInvitador);

            _grupoRepositoryMock
                .Setup(r => r.GetByIdAsync(grupoId, ct))
                .ReturnsAsync(grupo);

            _grupoRepositoryMock
                .Setup(r => r.UsuarioEsAdministradorAsync(grupoId, usuarioInvitador.Id, ct))
                .ReturnsAsync(true);

            _usuarioRepositoryMock
                .Setup(r => r.GetByIdAsync(usuarioInvitado.Id, ct))
                .ReturnsAsync(usuarioInvitado);

            _miembroGrupoRepositoryMock
                .Setup(r => r.UsuarioEsMiembroActivoAsync(grupoId, usuarioInvitado.Id, ct))
                .ReturnsAsync(false);

            _invitacionRepositoryMock
                .Setup(r => r.ExisteInvitacionPendienteAsync(grupoId, usuarioInvitado.Id, ct))
                .ReturnsAsync(false);

            _notificacionRepositoryMock
                .Setup(r => r.crearAsync(It.IsAny<Notificacion>(), ct))
                .Returns(Task.CompletedTask);

            var invitacionDummy = (InvitacionGrupo)Activator.CreateInstance(typeof(InvitacionGrupo), nonPublic: true)!;

            _invitacionRepositoryMock
                .Setup(r => r.CreateAsync(It.IsAny<InvitacionGrupo>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(invitacionDummy);

            _notificacionRepositoryMock
                .Setup(r => r.UpdateAsync(It.IsAny<Notificacion>(), ct))
                .Returns(Task.CompletedTask);

            _invitacionRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), ct))
                .ReturnsAsync((InvitacionGrupo?)null);

            _notificacionRealtimeServiceMock
                .Setup(s => s.EnviarNotificacionAsync(
                    usuarioInvitado.FirebaseUid,
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    ct,
                    It.IsAny<Guid>(),
                    It.IsAny<Guid>()))
                .Returns(Task.CompletedTask);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _sut.HandleAsync(firebaseUid, grupoId, null, usuarioInvitado.Id, null, "msg", ct));

            Assert.Equal("Error al crear la invitación", ex.Message);
        }
    }
}
