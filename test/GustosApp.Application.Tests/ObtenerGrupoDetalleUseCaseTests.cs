using System;
using System.Threading;
using System.Threading.Tasks;
using GustosApp.Application.UseCases.GrupoUseCases;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using Moq;
using Xunit;

namespace GustosApp.Application.Tests
{
    public class ObtenerGrupoDetalleUseCaseTests
    {
        private readonly Mock<IGrupoRepository> _grupoRepositoryMock;
        private readonly Mock<IUsuarioRepository> _usuarioRepositoryMock;
        private readonly ObtenerGrupoDetalleUseCase _sut;

        public ObtenerGrupoDetalleUseCaseTests()
        {
            _grupoRepositoryMock = new Mock<IGrupoRepository>();
            _usuarioRepositoryMock = new Mock<IUsuarioRepository>();

            _sut = new ObtenerGrupoDetalleUseCase(
                _grupoRepositoryMock.Object,
                _usuarioRepositoryMock.Object);
        }

        private static Usuario CreateUsuario(Guid? id = null, string firebaseUid = "firebase-uid")
        {
            var type = typeof(Usuario);
            var usuario = (Usuario)Activator.CreateInstance(type, nonPublic: true)!;

            type.GetProperty("Id")?.SetValue(usuario, id ?? Guid.NewGuid());
            type.GetProperty("FirebaseUid")?.SetValue(usuario, firebaseUid);

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

        // Verifica que cuando el usuario no existe se lanza UnauthorizedAccessException con el mensaje correcto
        [Fact]
        public async Task HandleAsync_UsuarioNoEncontrado_LanzaUnauthorizedAccessException()
        {
            var firebaseUid = "uid-inexistente";
            var grupoId = Guid.NewGuid();
            var ct = CancellationToken.None;

            _usuarioRepositoryMock
                .Setup(r => r.GetByFirebaseUidAsync(firebaseUid, ct))
                .ReturnsAsync((Usuario?)null);

            var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _sut.HandleAsync(firebaseUid, grupoId, ct));

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

            var usuario = CreateUsuario(firebaseUid: firebaseUid);

            _usuarioRepositoryMock
                .Setup(r => r.GetByFirebaseUidAsync(firebaseUid, ct))
                .ReturnsAsync(usuario);

            _grupoRepositoryMock
                .Setup(r => r.GetByIdAsync(grupoId, ct))
                .ReturnsAsync((Grupo?)null);

            var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
                _sut.HandleAsync(firebaseUid, grupoId, ct));

            Assert.Equal("Grupo no encontrado", ex.Message);

            _grupoRepositoryMock.Verify(
                r => r.GetByIdAsync(grupoId, ct),
                Times.Once);
        }

        // Verifica que cuando usuario y grupo existen se devuelve el grupo obtenido del repositorio
        [Fact]
        public async Task HandleAsync_UsuarioYGrupoValidos_DevuelveGrupo()
        {
            var firebaseUid = "uid-valido";
            var grupoId = Guid.NewGuid();
            var ct = CancellationToken.None;

            var usuario = CreateUsuario(firebaseUid: firebaseUid);
            var grupo = CreateGrupo(grupoId, "Grupo de detalle");

            _usuarioRepositoryMock
                .Setup(r => r.GetByFirebaseUidAsync(firebaseUid, ct))
                .ReturnsAsync(usuario);

            _grupoRepositoryMock
                .Setup(r => r.GetByIdAsync(grupoId, ct))
                .ReturnsAsync(grupo);

            var resultado = await _sut.HandleAsync(firebaseUid, grupoId, ct);

            Assert.NotNull(resultado);
            Assert.Same(grupo, resultado);

            _usuarioRepositoryMock.Verify(
                r => r.GetByFirebaseUidAsync(firebaseUid, ct),
                Times.Once);

            _grupoRepositoryMock.Verify(
                r => r.GetByIdAsync(grupoId, ct),
                Times.Once);
        }
    }
}
