using System;
using System.Threading;
using System.Threading.Tasks;
using GustosApp.Application.UseCases.GrupoUseCases;
using GustosApp.Domain.Interfaces;
using Moq;
using Xunit;

namespace GustosApp.Application.Tests
{
    public class VerificarSiMiembroEstaEnGrupoUseCaseTests
    {
        private readonly Mock<IGrupoRepository> _grupoRepositoryMock;
        private readonly VerificarSiMiembroEstaEnGrupoUseCase _sut;

        public VerificarSiMiembroEstaEnGrupoUseCaseTests()
        {
            _grupoRepositoryMock = new Mock<IGrupoRepository>();
            _sut = new VerificarSiMiembroEstaEnGrupoUseCase(_grupoRepositoryMock.Object);
        }

        // Verifica que cuando el repositorio devuelve true, el use case devuelve true
        [Fact]
        public async Task HandleAsync_UsuarioEsMiembro_DevuelveTrue()
        {
            var firebaseUid = "uid-test";
            var grupoId = Guid.NewGuid();
            var ct = CancellationToken.None;

            _grupoRepositoryMock
                .Setup(r => r.UsuarioEsMiembroAsync(grupoId, firebaseUid, ct))
                .ReturnsAsync(true);

            var result = await _sut.HandleAsync(firebaseUid, grupoId, ct);

            Assert.True(result);

            _grupoRepositoryMock.Verify(
                r => r.UsuarioEsMiembroAsync(grupoId, firebaseUid, ct),
                Times.Once);
        }

        // Verifica que cuando el repositorio devuelve false, el use case devuelve false
        [Fact]
        public async Task HandleAsync_UsuarioNoEsMiembro_DevuelveFalse()
        {
            var firebaseUid = "uid-test";
            var grupoId = Guid.NewGuid();
            var ct = CancellationToken.None;

            _grupoRepositoryMock
                .Setup(r => r.UsuarioEsMiembroAsync(grupoId, firebaseUid, ct))
                .ReturnsAsync(false);

            var result = await _sut.HandleAsync(firebaseUid, grupoId, ct);

            Assert.False(result);

            _grupoRepositoryMock.Verify(
                r => r.UsuarioEsMiembroAsync(grupoId, firebaseUid, ct),
                Times.Once);
        }
    }
}
