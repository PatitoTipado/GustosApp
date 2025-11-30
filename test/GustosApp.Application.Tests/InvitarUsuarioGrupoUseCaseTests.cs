using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
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
        private readonly Mock<IGrupoRepository> _grupoRepoMock = new();
        private readonly Mock<IUsuarioRepository> _usuarioRepoMock = new();
        private readonly Mock<IInvitacionGrupoRepository> _invitacionRepoMock = new();
        private readonly Mock<IMiembroGrupoRepository> _miembroGrupoRepoMock = new();
        private readonly Mock<INotificacionRepository> _notificacionRepoMock = new();
        private readonly Mock<INotificacionRealtimeService> _notificacionRealtimeMock = new();

        private InvitarUsuarioGrupoUseCase CreateSut()
        {
            return new InvitarUsuarioGrupoUseCase(
                _grupoRepoMock.Object,
                _usuarioRepoMock.Object,
                _invitacionRepoMock.Object,
                _miembroGrupoRepoMock.Object,
                _notificacionRepoMock.Object,
                _notificacionRealtimeMock.Object
            );
        }

        private Usuario CrearUsuario(
            Guid? id = null,
            string firebaseUid = "firebase-uid",
            string email = "user@test.com",
            string nombre = "Nombre",
            string apellido = "Apellido",
            string idUsuario = "user123")
        {
            return new Usuario
            {
                Id = id ?? Guid.NewGuid(),
                FirebaseUid = firebaseUid,
                Email = email,
                Nombre = nombre,
                Apellido = apellido,
                IdUsuario = idUsuario,
                Activo = true,
                EsPrivado = false
            };
        }

        private Grupo CrearGrupo(Guid? id = null, string nombre = "Grupo Test")
        {
            return new Grupo(nombre, Guid.NewGuid())
            {
                Id = id ?? Guid.NewGuid()
            };
        }

      
        [Fact]
        public async Task HandleAsync_Deberia_CrearInvitacionYNotificacion_CuandoTodoEsValido()
        {
            // Arrange
            var sut = CreateSut();
            var ct = CancellationToken.None;

            var grupoId = Guid.NewGuid();
            var firebaseInvitador = "invitador-fb";
            var usuarioInvitador = CrearUsuario(firebaseUid: firebaseInvitador, idUsuario: "admin123");
            var usuarioInvitado = CrearUsuario(firebaseUid: "invitado-fb", idUsuario: "user456");
            var grupo = CrearGrupo(grupoId, "Grupo Comidas");

            _usuarioRepoMock
                .Setup(r => r.GetByFirebaseUidAsync(firebaseInvitador, ct))
                .ReturnsAsync(usuarioInvitador);

            _grupoRepoMock
                .Setup(r => r.GetByIdAsync(grupoId, ct))
                .ReturnsAsync(grupo);

            _grupoRepoMock
                .Setup(r => r.UsuarioEsAdministradorAsync(grupoId, usuarioInvitador.Id, ct))
                .ReturnsAsync(true);

            // Lo encuentra por UsuarioId
            _usuarioRepoMock
                .Setup(r => r.GetByIdAsync(usuarioInvitado.Id, ct))
                .ReturnsAsync(usuarioInvitado);

            // No es miembro activo
            _miembroGrupoRepoMock
                .Setup(r => r.UsuarioEsMiembroActivoAsync(grupoId, usuarioInvitado.Id, ct))
                .ReturnsAsync(false);

            // No hay invitación previa
            _invitacionRepoMock
                .Setup(r => r.ObtenerUltimaInvitacionAsync(grupoId, usuarioInvitado.Id, ct))
                .ReturnsAsync((InvitacionGrupo?)null);

            Notificacion? notificacionCreada = null;
            _notificacionRepoMock
                .Setup(r => r.crearAsync(It.IsAny<Notificacion>(), ct))
                .Callback<Notificacion, CancellationToken>((n, _) => notificacionCreada = n)
                .Returns(Task.CompletedTask);

            InvitacionGrupo? invitacionCreada = null;
            _invitacionRepoMock
                .Setup(r => r.CreateAsync(It.IsAny<InvitacionGrupo>(), ct))
                .Callback<InvitacionGrupo, CancellationToken>((i, _) => invitacionCreada = i)
                 .ReturnsAsync((InvitacionGrupo?)null);


            _notificacionRepoMock
                .Setup(r => r.UpdateAsync(It.IsAny<Notificacion>(), ct))
                .Returns(Task.CompletedTask);

            // GetByIdAsync devuelve la invitación creada
            _invitacionRepoMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), ct))
                .ReturnsAsync(() => invitacionCreada!);

            // Act
            var resultado = await sut.HandleAsync(
                firebaseInvitador,
                grupoId,
                EmailUsuario: null,
                UsuarioId: usuarioInvitado.Id,
                UsuarioUsername: null,
                MensajePersonalizado: "Te invito al grupo",
                ct: ct);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeSameAs(invitacionCreada);

            // Notificación creada correctamente
            notificacionCreada.Should().NotBeNull();
            notificacionCreada!.UsuarioDestinoId.Should().Be(usuarioInvitado.Id);
            notificacionCreada.Titulo.Should().Be("Invitación a grupo");
            notificacionCreada.Tipo.Should().Be(TipoNotificacion.InvitacionGrupo);

            // Invitación creada correctamente
            invitacionCreada.Should().NotBeNull();
            invitacionCreada!.GrupoId.Should().Be(grupoId);
            invitacionCreada.UsuarioInvitadoId.Should().Be(usuarioInvitado.Id);
            invitacionCreada.UsuarioInvitadorId.Should().Be(usuarioInvitador.Id);
            invitacionCreada.MensajePersonalizado.Should().Be("Te invito al grupo");

            // Notificación actualizada con InvitacionId
            _notificacionRepoMock.Verify(
                r => r.UpdateAsync(It.Is<Notificacion>(n =>
                    n.InvitacionId == invitacionCreada.Id), ct),
                Times.Once);

            // Notificación realtime enviada
            _notificacionRealtimeMock.Verify(r => r.EnviarNotificacionAsync(
                usuarioInvitado.FirebaseUid,
                notificacionCreada.Titulo,
                notificacionCreada.Mensaje,
                notificacionCreada.Tipo.ToString(),
                ct,
                notificacionCreada.Id,
                invitacionCreada.Id
            ), Times.Once);
        }

       

        [Fact]
        public async Task HandleAsync_Deberia_Lanzar_Unauthorized_SiUsuarioInvitadorNoExiste()
        {
            // Arrange
            var sut = CreateSut();
            var ct = CancellationToken.None;

            _usuarioRepoMock
                .Setup(r => r.GetByFirebaseUidAsync("uid-invalido", ct))
                .ReturnsAsync((Usuario?)null);

            // Act
            var act = () => sut.HandleAsync(
                "uid-invalido",
                Guid.NewGuid(),
                null, null, null, null,
                ct);

            // Assert
            await act.Should()
                .ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("Usuario no encontrado");
        }

        [Fact]
        public async Task HandleAsync_Deberia_Lanzar_ArgumentException_SiGrupoNoExiste()
        {
            // Arrange
            var sut = CreateSut();
            var ct = CancellationToken.None;

            var firebaseInvitador = "fb-1";
            var usuarioInvitador = CrearUsuario(firebaseUid: firebaseInvitador);

            _usuarioRepoMock
                .Setup(r => r.GetByFirebaseUidAsync(firebaseInvitador, ct))
                .ReturnsAsync(usuarioInvitador);

            _grupoRepoMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), ct))
                .ReturnsAsync((Grupo?)null);

            // Act
            var act = () => sut.HandleAsync(
                firebaseInvitador,
                Guid.NewGuid(),
                null, null, null, null,
                ct);

            // Assert
            await act.Should()
                .ThrowAsync<ArgumentException>()
                .WithMessage("Grupo no encontrado");
        }

        [Fact]
        public async Task HandleAsync_Deberia_Lanzar_Unauthorized_SiUsuarioNoEsAdministrador()
        {
            // Arrange
            var sut = CreateSut();
            var ct = CancellationToken.None;

            var grupoId = Guid.NewGuid();
            var firebaseInvitador = "fb-1";
            var usuarioInvitador = CrearUsuario(firebaseUid: firebaseInvitador);
            var grupo = CrearGrupo(grupoId);

            _usuarioRepoMock
                .Setup(r => r.GetByFirebaseUidAsync(firebaseInvitador, ct))
                .ReturnsAsync(usuarioInvitador);

            _grupoRepoMock
                .Setup(r => r.GetByIdAsync(grupoId, ct))
                .ReturnsAsync(grupo);

            _grupoRepoMock
                .Setup(r => r.UsuarioEsAdministradorAsync(grupoId, usuarioInvitador.Id, ct))
                .ReturnsAsync(false);

            // Act
            var act = () => sut.HandleAsync(
                firebaseInvitador,
                grupoId,
                null, null, null, null,
                ct);

            // Assert
            await act.Should()
                .ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("Solo los administradores pueden invitar usuarios");
        }

        [Fact]
        public async Task HandleAsync_Deberia_Lanzar_ArgumentException_SiUsuarioInvitadoNoSeEncuentra()
        {
            // Arrange
            var sut = CreateSut();
            var ct = CancellationToken.None;

            var grupoId = Guid.NewGuid();
            var firebaseInvitador = "fb-1";
            var usuarioInvitador = CrearUsuario(firebaseUid: firebaseInvitador);
            var grupo = CrearGrupo(grupoId);

            _usuarioRepoMock
                .Setup(r => r.GetByFirebaseUidAsync(firebaseInvitador, ct))
                .ReturnsAsync(usuarioInvitador);

            _grupoRepoMock
                .Setup(r => r.GetByIdAsync(grupoId, ct))
                .ReturnsAsync(grupo);

            _grupoRepoMock
                .Setup(r => r.UsuarioEsAdministradorAsync(grupoId, usuarioInvitador.Id, ct))
                .ReturnsAsync(true);

            // ObtenerUsuarioInvitado devuelve null
            _usuarioRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), ct))
                .ReturnsAsync((Usuario?)null);
            _usuarioRepoMock.Setup(r => r.GetByUsernameAsync(It.IsAny<string>(), ct))
                .ReturnsAsync((Usuario?)null);
            _usuarioRepoMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>(), ct))
                .ReturnsAsync((Usuario?)null);

            // Act
            var act = () => sut.HandleAsync(
                firebaseInvitador,
                grupoId,
                EmailUsuario: null,
                UsuarioId: null,
                UsuarioUsername: null,
                MensajePersonalizado: null,
                ct: ct);

            // Assert
            await act.Should()
                .ThrowAsync<ArgumentException>()
                .WithMessage("No se encontró el usuario a invitar");
        }

        [Fact]
        public async Task HandleAsync_Deberia_Lanzar_ArgumentException_CuandoUsuarioSeInvitaASiMismo()
        {
            // Arrange
            var sut = CreateSut();
            var ct = CancellationToken.None;

            var grupoId = Guid.NewGuid();
            var firebaseInvitador = "fb-1";
            var usuarioInvitador = CrearUsuario(firebaseUid: firebaseInvitador);
            var grupo = CrearGrupo(grupoId);

            _usuarioRepoMock
                .Setup(r => r.GetByFirebaseUidAsync(firebaseInvitador, ct))
                .ReturnsAsync(usuarioInvitador);

            _grupoRepoMock
                .Setup(r => r.GetByIdAsync(grupoId, ct))
                .ReturnsAsync(grupo);

            _grupoRepoMock
                .Setup(r => r.UsuarioEsAdministradorAsync(grupoId, usuarioInvitador.Id, ct))
                .ReturnsAsync(true);

            // Vamos a hacer que el invitado sea el mismo usuario (mismo Id)
            _usuarioRepoMock
                .Setup(r => r.GetByIdAsync(usuarioInvitador.Id, ct))
                .ReturnsAsync(usuarioInvitador);

            // Act
            var act = () => sut.HandleAsync(
                firebaseInvitador,
                grupoId,
                EmailUsuario: null,
                UsuarioId: usuarioInvitador.Id,
                UsuarioUsername: null,
                MensajePersonalizado: null,
                ct: ct);

            // Assert
            await act.Should()
                .ThrowAsync<ArgumentException>()
                .WithMessage("No puedes invitarte a ti mismo");
        }

        [Fact]
        public async Task HandleAsync_Deberia_Lanzar_ArgumentException_CuandoUsuarioEsMiembroActivo()
        {
            // Arrange
            var sut = CreateSut();
            var ct = CancellationToken.None;

            var grupoId = Guid.NewGuid();
            var firebaseInvitador = "fb-1";
            var usuarioInvitador = CrearUsuario(firebaseUid: firebaseInvitador);
            var usuarioInvitado = CrearUsuario(firebaseUid: "invitado-fb");
            var grupo = CrearGrupo(grupoId);

            _usuarioRepoMock
                .Setup(r => r.GetByFirebaseUidAsync(firebaseInvitador, ct))
                .ReturnsAsync(usuarioInvitador);

            _grupoRepoMock
                .Setup(r => r.GetByIdAsync(grupoId, ct))
                .ReturnsAsync(grupo);

            _grupoRepoMock
                .Setup(r => r.UsuarioEsAdministradorAsync(grupoId, usuarioInvitador.Id, ct))
                .ReturnsAsync(true);

            _usuarioRepoMock
                .Setup(r => r.GetByIdAsync(usuarioInvitado.Id, ct))
                .ReturnsAsync(usuarioInvitado);

            _miembroGrupoRepoMock
                .Setup(r => r.UsuarioEsMiembroActivoAsync(grupoId, usuarioInvitado.Id, ct))
                .ReturnsAsync(true);

            // Act
            var act = () => sut.HandleAsync(
                firebaseInvitador,
                grupoId,
                EmailUsuario: null,
                UsuarioId: usuarioInvitado.Id,
                UsuarioUsername: null,
                MensajePersonalizado: null,
                ct: ct);

            // Assert
            await act.Should()
                .ThrowAsync<ArgumentException>()
                .WithMessage("El usuario ya es miembro del grupo");
        }

        [Fact]
        public async Task HandleAsync_Deberia_Lanzar_ArgumentException_SiHayInvitacionPendiente()
        {
            // Arrange
            var sut = CreateSut();
            var ct = CancellationToken.None;

            var grupoId = Guid.NewGuid();
            var firebaseInvitador = "fb-1";
            var usuarioInvitador = CrearUsuario(firebaseUid: firebaseInvitador);
            var usuarioInvitado = CrearUsuario(firebaseUid: "invitado-fb");
            var grupo = CrearGrupo(grupoId);

            _usuarioRepoMock
                .Setup(r => r.GetByFirebaseUidAsync(firebaseInvitador, ct))
                .ReturnsAsync(usuarioInvitador);

            _grupoRepoMock
                .Setup(r => r.GetByIdAsync(grupoId, ct))
                .ReturnsAsync(grupo);

            _grupoRepoMock
                .Setup(r => r.UsuarioEsAdministradorAsync(grupoId, usuarioInvitador.Id, ct))
                .ReturnsAsync(true);

            _usuarioRepoMock
                .Setup(r => r.GetByIdAsync(usuarioInvitado.Id, ct))
                .ReturnsAsync(usuarioInvitado);

            _miembroGrupoRepoMock
                .Setup(r => r.UsuarioEsMiembroActivoAsync(grupoId, usuarioInvitado.Id, ct))
                .ReturnsAsync(false);

            var invitacionPendiente = new InvitacionGrupo(grupoId, usuarioInvitado.Id, usuarioInvitador.Id, null);
            // Estado por defecto: Pendiente

            _invitacionRepoMock
                .Setup(r => r.ObtenerUltimaInvitacionAsync(grupoId, usuarioInvitado.Id, ct))
                .ReturnsAsync(invitacionPendiente);

            // Act
            var act = () => sut.HandleAsync(
                firebaseInvitador,
                grupoId,
                EmailUsuario: null,
                UsuarioId: usuarioInvitado.Id,
                UsuarioUsername: null,
                MensajePersonalizado: null,
                ct: ct);

            // Assert
            await act.Should()
                .ThrowAsync<ArgumentException>()
                .WithMessage("Ya existe una invitación pendiente para este usuario");
        }

        [Fact]
        public async Task HandleAsync_Deberia_Lanzar_ArgumentException_SiUltimaInvitacionAceptadaYUsuarioSigueEnGrupo()
        {
            // Arrange
            var sut = CreateSut();
            var ct = CancellationToken.None;

            var grupoId = Guid.NewGuid();
            var firebaseInvitador = "fb-1";
            var usuarioInvitador = CrearUsuario(firebaseUid: firebaseInvitador);
            var usuarioInvitado = CrearUsuario(firebaseUid: "invitado-fb");
            var grupo = CrearGrupo(grupoId);

            _usuarioRepoMock
                .Setup(r => r.GetByFirebaseUidAsync(firebaseInvitador, ct))
                .ReturnsAsync(usuarioInvitador);

            _grupoRepoMock
                .Setup(r => r.GetByIdAsync(grupoId, ct))
                .ReturnsAsync(grupo);

            _grupoRepoMock
                .Setup(r => r.UsuarioEsAdministradorAsync(grupoId, usuarioInvitador.Id, ct))
                .ReturnsAsync(true);

            _usuarioRepoMock
                .Setup(r => r.GetByIdAsync(usuarioInvitado.Id, ct))
                .ReturnsAsync(usuarioInvitado);

            _miembroGrupoRepoMock
                .Setup(r => r.UsuarioEsMiembroActivoAsync(grupoId, usuarioInvitado.Id, ct))
                .ReturnsAsync(true);

            var invitacionAceptada = new InvitacionGrupo(grupoId, usuarioInvitado.Id, usuarioInvitador.Id, null);
            invitacionAceptada.Aceptar(); // asumiendo que el método existe y cambia Estado

            _invitacionRepoMock
                .Setup(r => r.ObtenerUltimaInvitacionAsync(grupoId, usuarioInvitado.Id, ct))
                .ReturnsAsync(invitacionAceptada);

            // Act
            var act = () => sut.HandleAsync(
                firebaseInvitador,
                grupoId,
                EmailUsuario: null,
                UsuarioId: usuarioInvitado.Id,
                UsuarioUsername: null,
                MensajePersonalizado: null,
                ct: ct);

            // Assert
            await act.Should()
                .ThrowAsync<ArgumentException>()
                .WithMessage("El usuario ya es miembro del grupo");
        }

        [Fact]
        public async Task HandleAsync_Deberia_Lanzar_InvalidOperation_SiGetByIdAsync_RetornaNull()
        {
            // Arrange
            var sut = CreateSut();
            var ct = CancellationToken.None;

            var grupoId = Guid.NewGuid();
            var firebaseInvitador = "fb-1";
            var usuarioInvitador = CrearUsuario(firebaseUid: firebaseInvitador);
            var usuarioInvitado = CrearUsuario(firebaseUid: "invitado-fb");
            var grupo = CrearGrupo(grupoId);

            _usuarioRepoMock
                .Setup(r => r.GetByFirebaseUidAsync(firebaseInvitador, ct))
                .ReturnsAsync(usuarioInvitador);

            _grupoRepoMock
                .Setup(r => r.GetByIdAsync(grupoId, ct))
                .ReturnsAsync(grupo);

            _grupoRepoMock
                .Setup(r => r.UsuarioEsAdministradorAsync(grupoId, usuarioInvitador.Id, ct))
                .ReturnsAsync(true);

            _usuarioRepoMock
                .Setup(r => r.GetByIdAsync(usuarioInvitado.Id, ct))
                .ReturnsAsync(usuarioInvitado);

            _miembroGrupoRepoMock
                .Setup(r => r.UsuarioEsMiembroActivoAsync(grupoId, usuarioInvitado.Id, ct))
                .ReturnsAsync(false);

            _invitacionRepoMock
                .Setup(r => r.ObtenerUltimaInvitacionAsync(grupoId, usuarioInvitado.Id, ct))
                .ReturnsAsync((InvitacionGrupo?)null);

            _notificacionRepoMock
                .Setup(r => r.crearAsync(It.IsAny<Notificacion>(), ct))
                .Returns(Task.CompletedTask);

            _invitacionRepoMock
                .Setup(r => r.CreateAsync(It.IsAny<InvitacionGrupo>(), ct))
            .ReturnsAsync((InvitacionGrupo?)null);

            _notificacionRepoMock
                .Setup(r => r.UpdateAsync(It.IsAny<Notificacion>(), ct))
                .Returns(Task.CompletedTask);

            _invitacionRepoMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), ct))
                .ReturnsAsync((InvitacionGrupo?)null);

            // Act
            var act = () => sut.HandleAsync(
                firebaseInvitador,
                grupoId,
                EmailUsuario: null,
                UsuarioId: usuarioInvitado.Id,
                UsuarioUsername: null,
                MensajePersonalizado: null,
                ct: ct);

            // Assert
            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("Error al crear la invitación");
        }
    }
}
