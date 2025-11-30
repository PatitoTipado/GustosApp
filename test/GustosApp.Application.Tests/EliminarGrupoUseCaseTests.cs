using System;
using System.Threading;
using System.Threading.Tasks;
using GustosApp.Application.UseCases;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using Moq;
using Xunit;

namespace GustosApp.Application.Tests
{
    public class EliminarGrupoUseCaseTests
    {
        private readonly Mock<IGrupoRepository> _grupoRepositoryMock;
        private readonly Mock<IUsuarioRepository> _usuarioRepositoryMock;
        private readonly EliminarGrupoUseCase _sut;

        public EliminarGrupoUseCaseTests()
        {
            _grupoRepositoryMock = new Mock<IGrupoRepository>();
            _usuarioRepositoryMock = new Mock<IUsuarioRepository>();

            _sut = new EliminarGrupoUseCase(
                _grupoRepositoryMock.Object,
                _usuarioRepositoryMock.Object);
        }

        private static Usuario CreateUsuario()
        {
            return (Usuario)Activator.CreateInstance(typeof(Usuario), nonPublic: true)!;
        }

        //Usuario no encontrado 
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
                g => g.UsuarioEsAdministradorAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
                Times.Never);

            _grupoRepositoryMock.Verify(
                g => g.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        // Usuario no es admin 
        [Fact]
        public async Task HandleAsync_UsuarioNoEsAdmin_LanzaUnauthorizedAccessException()
        {
            var firebaseUid = "uid-valido";
            var grupoId = Guid.NewGuid();
            var ct = CancellationToken.None;

            var usuario = CreateUsuario();

            _usuarioRepositoryMock
                .Setup(r => r.GetByFirebaseUidAsync(firebaseUid, ct))
                .ReturnsAsync(usuario);

            _grupoRepositoryMock
                .Setup(g => g.UsuarioEsAdministradorAsync(grupoId, It.IsAny<Guid>(), ct))
                .ReturnsAsync(false);

            var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _sut.HandleAsync(firebaseUid, grupoId, ct));

            Assert.Equal("No tienes permisos para eliminar este grupo", ex.Message);

            _grupoRepositoryMock.Verify(
                g => g.UsuarioEsAdministradorAsync(grupoId, It.IsAny<Guid>(), ct),
                Times.Once);

            _grupoRepositoryMock.Verify(
                g => g.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        // Usuario admin -
        [Fact]
        public async Task HandleAsync_UsuarioEsAdmin_EliminaGrupoYDevuelveTrue()
        {
            var firebaseUid = "uid-admin";
            var grupoId = Guid.NewGuid();
            var ct = CancellationToken.None;

            var usuario = CreateUsuario();

            _usuarioRepositoryMock
                .Setup(r => r.GetByFirebaseUidAsync(firebaseUid, ct))
                .ReturnsAsync(usuario);

            _grupoRepositoryMock
                .Setup(g => g.UsuarioEsAdministradorAsync(grupoId, It.IsAny<Guid>(), ct))
                .ReturnsAsync(true);

            _grupoRepositoryMock
                .Setup(g => g.DeleteAsync(grupoId, ct))
                .Returns(Task.CompletedTask)
                .Verifiable();

            var resultado = await _sut.HandleAsync(firebaseUid, grupoId, ct);

            Assert.True(resultado);

            _grupoRepositoryMock.Verify(
                g => g.UsuarioEsAdministradorAsync(grupoId, It.IsAny<Guid>(), ct),
                Times.Once);

            _grupoRepositoryMock.Verify(
                g => g.DeleteAsync(grupoId, ct),
                Times.Once);
        }
    }
}
