using GustosApp.Application.UseCases.GrupoUseCases;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using Moq;
using Xunit;

namespace GustosApp.Application.Tests
{
    public class EliminarGustosGrupoUseCaseTests
    {
        [Fact]
        public async Task Handle_UsuarioNoExiste_ThrowsUnauthorized()
        {
            var mockGrupoRepo = new Mock<IGrupoRepository>();
            var mockGustoRepo = new Mock<IGustoRepository>();
            var mockGustosGrupoRepo = new Mock<IGustosGrupoRepository>();
            var mockUsuarioRepo = new Mock<IUsuarioRepository>();
            var mockMiembroRepo = new Mock<IMiembroGrupoRepository>();

            var useCase = new EliminarGustosGrupoUseCase(
                mockGrupoRepo.Object,
                mockGustoRepo.Object,
                mockGustosGrupoRepo.Object,
                mockUsuarioRepo.Object,
                mockMiembroRepo.Object);

            mockUsuarioRepo
                .Setup(r => r.GetByFirebaseUidAsync("uid", It.IsAny<CancellationToken>()))
                .ReturnsAsync((Usuario)null);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                useCase.Handle(new List<string> { "Pizza" }, Guid.NewGuid(), "uid"));
        }

        [Fact]
        public async Task Handle_GrupoNoExiste_ThrowsKeyNotFound()
        {
            var mockGrupoRepo = new Mock<IGrupoRepository>();
            var mockGustoRepo = new Mock<IGustoRepository>();
            var mockGustosGrupoRepo = new Mock<IGustosGrupoRepository>();
            var mockUsuarioRepo = new Mock<IUsuarioRepository>();
            var mockMiembroRepo = new Mock<IMiembroGrupoRepository>();

            var useCase = new EliminarGustosGrupoUseCase(
                mockGrupoRepo.Object,
                mockGustoRepo.Object,
                mockGustosGrupoRepo.Object,
                mockUsuarioRepo.Object,
                mockMiembroRepo.Object);

            var usuario = new Usuario { Id = Guid.NewGuid() };

            mockUsuarioRepo.Setup(r => r.GetByFirebaseUidAsync("uid", It.IsAny<CancellationToken>()))
                           .ReturnsAsync(usuario);

            mockGrupoRepo.Setup(r => r.ExistsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(false);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                useCase.Handle(new List<string> { "Hamburguesa" }, Guid.NewGuid(), "uid"));
        }

        [Fact]
        public async Task Handle_MiembroNoActivo_ThrowsUnauthorized()
        {
            var mockGrupoRepo = new Mock<IGrupoRepository>();
            var mockGustoRepo = new Mock<IGustoRepository>();
            var mockGustosGrupoRepo = new Mock<IGustosGrupoRepository>();
            var mockUsuarioRepo = new Mock<IUsuarioRepository>();
            var mockMiembroRepo = new Mock<IMiembroGrupoRepository>();

            var useCase = new EliminarGustosGrupoUseCase(
                mockGrupoRepo.Object,
                mockGustoRepo.Object,
                mockGustosGrupoRepo.Object,
                mockUsuarioRepo.Object,
                mockMiembroRepo.Object);

            var usuario = new Usuario { Id = Guid.NewGuid() };

            mockUsuarioRepo.Setup(r => r.GetByFirebaseUidAsync("uid", It.IsAny<CancellationToken>()))
                           .ReturnsAsync(usuario);

            mockGrupoRepo.Setup(r => r.ExistsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(true);

            mockMiembroRepo
                .Setup(r => r.UsuarioEsMiembroActivoAsync(It.IsAny<Guid>(), usuario.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                useCase.Handle(new List<string> { "Pizza" }, Guid.NewGuid(), "uid"));
        }

        [Fact]
        public async Task Handle_GustosNoExisten_ThrowsKeyNotFound()
        {
            var mockGrupoRepo = new Mock<IGrupoRepository>();
            var mockGustoRepo = new Mock<IGustoRepository>();
            var mockGustosGrupoRepo = new Mock<IGustosGrupoRepository>();
            var mockUsuarioRepo = new Mock<IUsuarioRepository>();
            var mockMiembroRepo = new Mock<IMiembroGrupoRepository>();

            var useCase = new EliminarGustosGrupoUseCase(
                mockGrupoRepo.Object,
                mockGustoRepo.Object,
                mockGustosGrupoRepo.Object,
                mockUsuarioRepo.Object,
                mockMiembroRepo.Object);

            var usuario = new Usuario { Id = Guid.NewGuid() };

            mockUsuarioRepo.Setup(r => r.GetByFirebaseUidAsync("uid", It.IsAny<CancellationToken>()))
                           .ReturnsAsync(usuario);

            mockGrupoRepo.Setup(r => r.ExistsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(true);

            mockMiembroRepo.Setup(r =>
                r.UsuarioEsMiembroActivoAsync(It.IsAny<Guid>(), usuario.Id, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(true);

            mockGustoRepo.Setup(r => r.obtenerGustosPorNombre(It.IsAny<List<string>>()))
                         .ReturnsAsync(new List<Gusto>()); // vacío

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                useCase.Handle(new List<string> { "Fideos" }, Guid.NewGuid(), "uid"));
        }

        [Fact]
        public async Task Handle_TodoValido_EliminaYReturnTrue()
        {
            var mockGrupoRepo = new Mock<IGrupoRepository>();
            var mockGustoRepo = new Mock<IGustoRepository>();
            var mockGustosGrupoRepo = new Mock<IGustosGrupoRepository>();
            var mockUsuarioRepo = new Mock<IUsuarioRepository>();
            var mockMiembroRepo = new Mock<IMiembroGrupoRepository>();

            var useCase = new EliminarGustosGrupoUseCase(
                mockGrupoRepo.Object,
                mockGustoRepo.Object,
                mockGustosGrupoRepo.Object,
                mockUsuarioRepo.Object,
                mockMiembroRepo.Object);

            var usuario = new Usuario { Id = Guid.NewGuid() };
            var gustos = new List<Gusto> { new Gusto { Id = Guid.NewGuid(), Nombre = "Tacos" } };

            mockUsuarioRepo.Setup(r => r.GetByFirebaseUidAsync("uid", It.IsAny<CancellationToken>()))
                           .ReturnsAsync(usuario);

            mockGrupoRepo.Setup(r => r.ExistsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(true);

            mockMiembroRepo
                .Setup(r => r.UsuarioEsMiembroActivoAsync(It.IsAny<Guid>(), usuario.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            mockGustoRepo.Setup(r => r.obtenerGustosPorNombre(It.IsAny<List<string>>()))
                         .ReturnsAsync(gustos);

            mockGustosGrupoRepo.Setup(r =>
                r.EliminarGustosAlGrupo(It.IsAny<Guid>(), gustos, usuario.Id))
                .ReturnsAsync(true);

            var result = await useCase.Handle(new List<string> { "Tacos" }, Guid.NewGuid(), "uid");

            Assert.True(result);

            mockGustosGrupoRepo.Verify(r =>
                r.EliminarGustosAlGrupo(It.IsAny<Guid>(), gustos, usuario.Id),
                Times.Once);
        }
    }
}