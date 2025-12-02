using GustosApp.Application.UseCases.GrupoUseCases;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using Moq;

namespace GustosApp.Application.Tests
{
    public class DesactivarMiembroDeGrupoUseCaseTests
    {
        [Fact]
        public async Task Handle_UsuarioSolicitanteNoExiste_ThrowsUnauthorized()
        {
            var mockGrupoRepo = new Mock<IGrupoRepository>();
            var mockUsuarioRepo = new Mock<IUsuarioRepository>();
            var mockMiembroRepo = new Mock<IMiembroGrupoRepository>();
           var mockVotacionRepo = new Mock<IVotacionRepository>();
            var useCase = new DesactivarMiembroDeGrupoUseCase(
                mockGrupoRepo.Object,
                mockUsuarioRepo.Object,
                mockMiembroRepo.Object,
                mockVotacionRepo.Object
            );

            mockUsuarioRepo.Setup(r => r.GetByFirebaseUidAsync("uid",It.IsAny<CancellationToken>())).ReturnsAsync((Usuario)null);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                useCase.Handle(Guid.NewGuid(), Guid.NewGuid(), "uid"));
        }

        [Fact]
        public async Task Handle_UsuarioADesactivarNoExiste_ThrowsArgumentException()
        {
            var mockGrupoRepo = new Mock<IGrupoRepository>();
            var mockUsuarioRepo = new Mock<IUsuarioRepository>();
            var mockMiembroRepo = new Mock<IMiembroGrupoRepository>();
            var mockVotacionRepo = new Mock<IVotacionRepository>();
            var useCase = new DesactivarMiembroDeGrupoUseCase(
                mockGrupoRepo.Object,
                mockUsuarioRepo.Object,
                mockMiembroRepo.Object
                , mockVotacionRepo.Object
            );

            var solicitante = new Usuario { Id = Guid.NewGuid() };

            mockUsuarioRepo
                .Setup(r => r.GetByFirebaseUidAsync("uid", It.IsAny<CancellationToken>()))
                .ReturnsAsync(solicitante);

            mockUsuarioRepo
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Usuario)null);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                useCase.Handle(Guid.NewGuid(), Guid.NewGuid(), "uid"));
        }

        [Fact]
        public async Task Handle_GrupoNoExiste_ThrowsKeyNotFound()
        {
            var mockGrupoRepo = new Mock<IGrupoRepository>();
            var mockUsuarioRepo = new Mock<IUsuarioRepository>();
            var mockMiembroRepo = new Mock<IMiembroGrupoRepository>();
            var mockVotacionRepo = new Mock<IVotacionRepository>();
            var useCase = new DesactivarMiembroDeGrupoUseCase(
                mockGrupoRepo.Object,
                mockUsuarioRepo.Object,
                mockMiembroRepo.Object
                , mockVotacionRepo.Object
            );

            var solicitante = new Usuario { Id = Guid.NewGuid() };
            var objetivo = new Usuario { Id = Guid.NewGuid() };

            mockUsuarioRepo
                .Setup(r => r.GetByFirebaseUidAsync("uid", It.IsAny<CancellationToken>()))
                .ReturnsAsync(solicitante);

            mockUsuarioRepo
                .Setup(r => r.GetByIdAsync(objetivo.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(objetivo);

            mockGrupoRepo
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Grupo)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                useCase.Handle(Guid.NewGuid(), objetivo.Id, "uid"));
        }

        [Fact]
        public async Task Handle_NoEsAdminNiElMismo_ThrowsUnauthorized()
        {
            var mockGrupoRepo = new Mock<IGrupoRepository>();
            var mockUsuarioRepo = new Mock<IUsuarioRepository>();
            var mockMiembroRepo = new Mock<IMiembroGrupoRepository>();
            var mockVotacionRepo = new Mock<IVotacionRepository>();
            var useCase = new DesactivarMiembroDeGrupoUseCase(
                mockGrupoRepo.Object,
                mockUsuarioRepo.Object,
                mockMiembroRepo.Object,
                  mockVotacionRepo.Object
            );

            var solicitante = new Usuario { Id = Guid.NewGuid() };
            var objetivo = new Usuario { Id = Guid.NewGuid() };

            mockUsuarioRepo
                .Setup(r => r.GetByFirebaseUidAsync("uid", It.IsAny<CancellationToken>()))
                .ReturnsAsync(solicitante);

            mockUsuarioRepo
                .Setup(r => r.GetByIdAsync(objetivo.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(objetivo);

            mockGrupoRepo
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Grupo("",Guid.NewGuid()));

            mockGrupoRepo
                .Setup(r => r.UsuarioEsAdministradorAsync(It.IsAny<Guid>(), solicitante.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                useCase.Handle(Guid.NewGuid(), objetivo.Id, "uid"));
        }


        [Fact]
        public async Task Handle_MiembroNoExiste_ThrowsInvalidOperation()
        {
            var mockGrupoRepo = new Mock<IGrupoRepository>();
            var mockUsuarioRepo = new Mock<IUsuarioRepository>();
            var mockMiembroRepo = new Mock<IMiembroGrupoRepository>();
            var mockVotacionRepo = new Mock<IVotacionRepository>();
            var useCase = new DesactivarMiembroDeGrupoUseCase(
                mockGrupoRepo.Object,
                mockUsuarioRepo.Object,
                mockMiembroRepo.Object,
              mockVotacionRepo.Object
            );

            var solicitante = new Usuario { Id = Guid.NewGuid() };
            var objetivo = new Usuario { Id = Guid.NewGuid() };

            mockUsuarioRepo.Setup(r => r.GetByFirebaseUidAsync("uid", It.IsAny<CancellationToken>()))
                           .ReturnsAsync(solicitante);

            mockUsuarioRepo.Setup(r => r.GetByIdAsync(objetivo.Id, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(objetivo);

            mockGrupoRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(new Grupo("", solicitante.Id));

            mockGrupoRepo.Setup(r => r.UsuarioEsAdministradorAsync(It.IsAny<Guid>(), solicitante.Id, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(true);

            mockMiembroRepo.Setup(r => r.GetByGrupoYUsuarioAsync(It.IsAny<Guid>(), objetivo.IdUsuario, It.IsAny<CancellationToken>()))
                           .ReturnsAsync((MiembroGrupo)null);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                useCase.Handle(Guid.NewGuid(), objetivo.Id, "uid"));
        }


        [Fact]
        public async Task Handle_MiembroYaDesactivado_ReturnsTrue()
        {
            var mockGrupoRepo = new Mock<IGrupoRepository>();
            var mockUsuarioRepo = new Mock<IUsuarioRepository>();
            var mockMiembroRepo = new Mock<IMiembroGrupoRepository>();
            var mockVotacionRepo = new Mock<IVotacionRepository>();
            var useCase = new DesactivarMiembroDeGrupoUseCase(
                mockGrupoRepo.Object,
                mockUsuarioRepo.Object,
                mockMiembroRepo.Object,
                mockVotacionRepo.Object
            );

            var solicitante = new Usuario { Id = Guid.NewGuid() };
            var objetivo = new Usuario { Id = Guid.NewGuid() };

            mockUsuarioRepo.Setup(r => r.GetByFirebaseUidAsync("uid", It.IsAny<CancellationToken>()))
                           .ReturnsAsync(solicitante);

            mockUsuarioRepo.Setup(r => r.GetByIdAsync(objetivo.Id, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(objetivo);

            mockGrupoRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(new Grupo("", solicitante.Id));

            mockGrupoRepo.Setup(r => r.UsuarioEsAdministradorAsync(It.IsAny<Guid>(), solicitante.Id, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(true);

            mockMiembroRepo.Setup(r => r.GetByGrupoYUsuarioAsync(It.IsAny<Guid>(), objetivo.IdUsuario, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(new MiembroGrupo (It.IsAny<Guid>(),objetivo.Id){ afectarRecomendacion = false });

            var result = await useCase.Handle(Guid.NewGuid(), objetivo.Id, "uid");

            Assert.True(result);
            mockMiembroRepo.Verify(r => r.DesactivarMiembroDeGrupo(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task Handle_MiembroActivo_DesactivaYRetornaTrue()
        {
            var mockGrupoRepo = new Mock<IGrupoRepository>();
            var mockUsuarioRepo = new Mock<IUsuarioRepository>();
            var mockMiembroRepo = new Mock<IMiembroGrupoRepository>();
            var mockVotacionRepo = new Mock<IVotacionRepository>();
            var useCase = new DesactivarMiembroDeGrupoUseCase(
                mockGrupoRepo.Object,
                mockUsuarioRepo.Object,
                mockMiembroRepo.Object,
                mockVotacionRepo.Object
            );

            var solicitante = new Usuario { Id = Guid.NewGuid() };
            var objetivo = new Usuario { Id = Guid.NewGuid() };
            var grupo = new Grupo("",solicitante.Id);

            mockUsuarioRepo.Setup(r => r.GetByFirebaseUidAsync("uid", It.IsAny<CancellationToken>()))
                           .ReturnsAsync(solicitante);

            mockUsuarioRepo.Setup(r => r.GetByIdAsync(objetivo.Id, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(objetivo);

            mockGrupoRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(grupo);

            mockGrupoRepo.Setup(r => r.UsuarioEsAdministradorAsync(It.IsAny<Guid>(), solicitante.Id, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(true);

            mockMiembroRepo.Setup(r => r.GetByGrupoYUsuarioAsync(It.IsAny<Guid>(), objetivo.IdUsuario, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(new MiembroGrupo(grupo.Id,objetivo.Id) { afectarRecomendacion = true });

            mockMiembroRepo.Setup(r => r.DesactivarMiembroDeGrupo(It.IsAny<Guid>(), objetivo.Id))
                           .ReturnsAsync(true);

            var result = await useCase.Handle(Guid.NewGuid(), objetivo.Id, "uid");

            Assert.True(result);
            mockMiembroRepo.Verify(r => r.DesactivarMiembroDeGrupo(It.IsAny<Guid>(), objetivo.Id), Times.Once);
        }

    }
}
