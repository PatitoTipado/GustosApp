using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GustosApp.Application.UseCases.GrupoUseCases;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using Moq;
using Xunit;

namespace GustosApp.Application.Tests
{
    public class RemoverMiembroGrupoUseCaseTests
    {
        private readonly Mock<IGrupoRepository> _grupoRepositoryMock;
        private readonly Mock<IUsuarioRepository> _usuarioRepositoryMock;
        private readonly Mock<IMiembroGrupoRepository> _miembroGrupoRepositoryMock;
        private readonly RemoverMiembroGrupoUseCase _sut;

        public RemoverMiembroGrupoUseCaseTests()
        {
            _grupoRepositoryMock = new Mock<IGrupoRepository>();
            _usuarioRepositoryMock = new Mock<IUsuarioRepository>();
            _miembroGrupoRepositoryMock = new Mock<IMiembroGrupoRepository>();

            _sut = new RemoverMiembroGrupoUseCase(
                _grupoRepositoryMock.Object,
                _usuarioRepositoryMock.Object,
                _miembroGrupoRepositoryMock.Object);
        }

        private static Usuario CreateUsuario(Guid? id = null, string firebaseUid = "firebase-uid")
        {
            var type = typeof(Usuario);
            var usuario = (Usuario)Activator.CreateInstance(type, nonPublic: true)!;

            type.GetProperty("Id")?.SetValue(usuario, id ?? Guid.NewGuid());
            type.GetProperty("FirebaseUid")?.SetValue(usuario, firebaseUid);

            return usuario;
        }

        private static Grupo CreateGrupo(Guid? id = null)
        {
            var type = typeof(Grupo);
            var grupo = (Grupo)Activator.CreateInstance(type, nonPublic: true)!;

            type.GetProperty("Id")?.SetValue(grupo, id ?? Guid.NewGuid());

            return grupo;
        }

        private static MiembroGrupo CreateMiembroGrupo(bool activo = true, bool esAdministrador = false)
        {
            var type = typeof(MiembroGrupo);
            var miembro = (MiembroGrupo)Activator.CreateInstance(type, nonPublic: true)!;

            type.GetProperty("Activo")?.SetValue(miembro, activo);
            type.GetProperty("EsAdministrador")?.SetValue(miembro, esAdministrador);

            return miembro;
        }

        // Verifica que cuando el usuario actor no existe se lanza UnauthorizedAccessException con el mensaje correcto
        [Fact]
        public async Task HandleAsync_UsuarioActorNoEncontrado_LanzaUnauthorizedAccessException()
        {
            var firebaseUid = "uid-inexistente";
            var grupoId = Guid.NewGuid();
            var ct = CancellationToken.None;

            _usuarioRepositoryMock
                .Setup(r => r.GetByFirebaseUidAsync(firebaseUid, ct))
                .ReturnsAsync((Usuario?)null);

            var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _sut.HandleAsync(firebaseUid, grupoId, "username", ct));

            Assert.Equal("Usuario no encontrado", ex.Message);

            _grupoRepositoryMock.Verify(
                r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        // Verifica que cuando el grupo no existe se lanza ArgumentException con el mensaje correcto
        [Fact]
        public async Task HandleAsync_GrupoNoEncontrado_LanzaArgumentException()
        {
            var firebaseUid = "uid-valido";
            var grupoId = Guid.NewGuid();
            var ct = CancellationToken.None;
            var usuarioActor = CreateUsuario(firebaseUid: firebaseUid);

            _usuarioRepositoryMock
                .Setup(r => r.GetByFirebaseUidAsync(firebaseUid, ct))
                .ReturnsAsync(usuarioActor);

            _grupoRepositoryMock
                .Setup(r => r.GetByIdAsync(grupoId, ct))
                .ReturnsAsync((Grupo?)null);

            var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
                _sut.HandleAsync(firebaseUid, grupoId, "username", ct));

            Assert.Equal("Grupo no encontrado", ex.Message);

            _grupoRepositoryMock.Verify(
                r => r.GetByIdAsync(grupoId, ct),
                Times.Once);

            _miembroGrupoRepositoryMock.Verify(
                r => r.GetByGrupoYUsuarioAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        // Verifica que cuando el usuario actor no es administrador se lanza UnauthorizedAccessException con el mensaje correcto
        [Fact]
        public async Task HandleAsync_UsuarioActorNoAdministrador_LanzaUnauthorizedAccessException()
        {
            var firebaseUid = "uid-valido";
            var grupoId = Guid.NewGuid();
            var ct = CancellationToken.None;
            var usuarioActor = CreateUsuario(firebaseUid: firebaseUid);
            var grupo = CreateGrupo(grupoId);

            _usuarioRepositoryMock
                .Setup(r => r.GetByFirebaseUidAsync(firebaseUid, ct))
                .ReturnsAsync(usuarioActor);

            _grupoRepositoryMock
                .Setup(r => r.GetByIdAsync(grupoId, ct))
                .ReturnsAsync(grupo);

            _grupoRepositoryMock
                .Setup(r => r.UsuarioEsAdministradorAsync(grupoId, usuarioActor.Id, ct))
                .ReturnsAsync(false);

            var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _sut.HandleAsync(firebaseUid, grupoId, "username", ct));

            Assert.Equal("Solo los administradores pueden eliminar miembros", ex.Message);

            _miembroGrupoRepositoryMock.Verify(
                r => r.GetByGrupoYUsuarioAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        // Verifica que cuando el usuario a remover no es miembro activo se lanza ArgumentException con el mensaje correcto.
        [Fact]
        public async Task HandleAsync_MiembroNoEncontradoOLNoActivo_LanzaArgumentException()
        {
            var firebaseUid = "uid-valido";
            var grupoId = Guid.NewGuid();
            var ct = CancellationToken.None;
            var usuarioActor = CreateUsuario(firebaseUid: firebaseUid);
            var grupo = CreateGrupo(grupoId);

            _usuarioRepositoryMock
                .Setup(r => r.GetByFirebaseUidAsync(firebaseUid, ct))
                .ReturnsAsync(usuarioActor);

            _grupoRepositoryMock
                .Setup(r => r.GetByIdAsync(grupoId, ct))
                .ReturnsAsync(grupo);

            _grupoRepositoryMock
                .Setup(r => r.UsuarioEsAdministradorAsync(grupoId, usuarioActor.Id, ct))
                .ReturnsAsync(true);

            var miembroInactivo = CreateMiembroGrupo(activo: false, esAdministrador: false);

            _miembroGrupoRepositoryMock
                .Setup(r => r.GetByGrupoYUsuarioAsync(grupoId, "user-removido", ct))
                .ReturnsAsync(miembroInactivo);

            var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
                _sut.HandleAsync(firebaseUid, grupoId, "user-removido", ct));

            Assert.Equal("El usuario no es miembro activo del grupo", ex.Message);

            _miembroGrupoRepositoryMock.Verify(
                r => r.UpdateAsync(It.IsAny<MiembroGrupo>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        // Verifica que no se permite eliminar al único administrador activo del grupo.
        [Fact]
        public async Task HandleAsync_UnicoAdministradorActivo_LanzaInvalidOperationException()
        {
            var firebaseUid = "uid-valido";
            var grupoId = Guid.NewGuid();
            var ct = CancellationToken.None;
            var usuarioActor = CreateUsuario(firebaseUid: firebaseUid);
            var grupo = CreateGrupo(grupoId);

            _usuarioRepositoryMock
                .Setup(r => r.GetByFirebaseUidAsync(firebaseUid, ct))
                .ReturnsAsync(usuarioActor);

            _grupoRepositoryMock
                .Setup(r => r.GetByIdAsync(grupoId, ct))
                .ReturnsAsync(grupo);

            _grupoRepositoryMock
                .Setup(r => r.UsuarioEsAdministradorAsync(grupoId, usuarioActor.Id, ct))
                .ReturnsAsync(true);

            var miembroAdmin = CreateMiembroGrupo(activo: true, esAdministrador: true);

            _miembroGrupoRepositoryMock
                .Setup(r => r.GetByGrupoYUsuarioAsync(grupoId, "admin-unico", ct))
                .ReturnsAsync(miembroAdmin);

            _miembroGrupoRepositoryMock
                .Setup(r => r.GetMiembrosByGrupoIdAsync(grupoId, ct))
                .ReturnsAsync(new List<MiembroGrupo> { miembroAdmin });

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _sut.HandleAsync(firebaseUid, grupoId, "admin-unico", ct));

            Assert.Equal("No puedes eliminar al único administrador del grupo", ex.Message);

            _miembroGrupoRepositoryMock.Verify(
                r => r.UpdateAsync(It.IsAny<MiembroGrupo>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        // Verifica que cuando todo es válido se marca al miembro como inactivo y se actualiza en el repositorio.
        [Fact]
        public async Task HandleAsync_CaminoFeliz_RemueveMiembroYDevuelveTrue()
        {
            var firebaseUid = "uid-valido";
            var grupoId = Guid.NewGuid();
            var ct = CancellationToken.None;
            var usuarioActor = CreateUsuario(firebaseUid: firebaseUid);
            var grupo = CreateGrupo(grupoId);

            _usuarioRepositoryMock
                .Setup(r => r.GetByFirebaseUidAsync(firebaseUid, ct))
                .ReturnsAsync(usuarioActor);

            _grupoRepositoryMock
                .Setup(r => r.GetByIdAsync(grupoId, ct))
                .ReturnsAsync(grupo);

            _grupoRepositoryMock
                .Setup(r => r.UsuarioEsAdministradorAsync(grupoId, usuarioActor.Id, ct))
                .ReturnsAsync(true);

            var miembro = CreateMiembroGrupo(activo: true, esAdministrador: false);

            _miembroGrupoRepositoryMock
                .Setup(r => r.GetByGrupoYUsuarioAsync(grupoId, "miembro-a-remover", ct))
                .ReturnsAsync(miembro);

            _miembroGrupoRepositoryMock
                .Setup(r => r.UpdateAsync(miembro, ct))
                .ReturnsAsync(miembro);

            var resultado = await _sut.HandleAsync(firebaseUid, grupoId, "miembro-a-remover", ct);

            Assert.True(resultado);

            _miembroGrupoRepositoryMock.Verify(
                r => r.UpdateAsync(miembro, ct),
                Times.Once);
        }

        // Verifica que cuando hay más de un administrador activo se permite remover a uno de ellos.
        [Fact]
        public async Task HandleAsync_AdminPeroHayMasDeUnAdministrador_PermiteRemover()
        {
            var firebaseUid = "uid-valido";
            var grupoId = Guid.NewGuid();
            var ct = CancellationToken.None;
            var usuarioActor = CreateUsuario(firebaseUid: firebaseUid);
            var grupo = CreateGrupo(grupoId);

            _usuarioRepositoryMock
                .Setup(r => r.GetByFirebaseUidAsync(firebaseUid, ct))
                .ReturnsAsync(usuarioActor);

            _grupoRepositoryMock
                .Setup(r => r.GetByIdAsync(grupoId, ct))
                .ReturnsAsync(grupo);

            _grupoRepositoryMock
                .Setup(r => r.UsuarioEsAdministradorAsync(grupoId, usuarioActor.Id, ct))
                .ReturnsAsync(true);

            var adminAEliminar = CreateMiembroGrupo(activo: true, esAdministrador: true);
            var otroAdmin = CreateMiembroGrupo(activo: true, esAdministrador: true);

            _miembroGrupoRepositoryMock
                .Setup(r => r.GetByGrupoYUsuarioAsync(grupoId, "admin-a-remover", ct))
                .ReturnsAsync(adminAEliminar);

            _miembroGrupoRepositoryMock
                .Setup(r => r.GetMiembrosByGrupoIdAsync(grupoId, ct))
                .ReturnsAsync(new List<MiembroGrupo> { adminAEliminar, otroAdmin });

            _miembroGrupoRepositoryMock
                .Setup(r => r.UpdateAsync(adminAEliminar, ct))
                .ReturnsAsync(adminAEliminar);

            var resultado = await _sut.HandleAsync(firebaseUid, grupoId, "admin-a-remover", ct);

            Assert.True(resultado);

            _miembroGrupoRepositoryMock.Verify(
                r => r.UpdateAsync(adminAEliminar, ct),
                Times.Once);
        }
    }
}
