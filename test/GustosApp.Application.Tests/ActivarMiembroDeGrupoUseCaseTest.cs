using GustosApp.Application.UseCases.GrupoUseCases;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GustosApp.Application.Tests
{
    public class ActivarMiembroDeGrupoUseCaseTest
    {

        [Fact]
        public async Task Handle_UsuarioSolicitanteNoExiste_ThrowsUnauthorized()
        {
            var mockGrupoRepo = new Mock<IGrupoRepository>();
            var mockUsuarioRepo = new Mock<IUsuarioRepository>();
            var mockMiembroRepo = new Mock<IMiembroGrupoRepository>();

            var useCase = new ActivarMiembroDeGrupoUseCase(
                mockGrupoRepo.Object,
                mockUsuarioRepo.Object,
                mockMiembroRepo.Object
            );

            mockUsuarioRepo
                .Setup(r => r.GetByFirebaseUidAsync("uid", It.IsAny<CancellationToken>()))
                .ReturnsAsync((Usuario)null);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                useCase.Handle(Guid.NewGuid(), Guid.NewGuid(), "uid")
            );
        }
        [Fact]
        public async Task Handle_UsuarioObtenidoNoExiste_ThrowsUnauthorized()
        {
            var mockGrupoRepo = new Mock<IGrupoRepository>();
            var mockUsuarioRepo = new Mock<IUsuarioRepository>();
            var mockMiembroRepo = new Mock<IMiembroGrupoRepository>();

            var useCase = new ActivarMiembroDeGrupoUseCase(
                mockGrupoRepo.Object,
                mockUsuarioRepo.Object,
                mockMiembroRepo.Object
            );

            var solicitante = new Usuario { Id = Guid.NewGuid() };

            mockUsuarioRepo
                .Setup(r => r.GetByFirebaseUidAsync("uid", It.IsAny<CancellationToken>()))
                .ReturnsAsync(solicitante);

            mockUsuarioRepo
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Usuario)null);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                useCase.Handle(Guid.NewGuid(), Guid.NewGuid(), "uid")
            );
        }
        [Fact]
        public async Task Handle_GrupoNoExiste_ThrowsKeyNotFound()
        {
            var mockGrupoRepo = new Mock<IGrupoRepository>();
            var mockUsuarioRepo = new Mock<IUsuarioRepository>();
            var mockMiembroRepo = new Mock<IMiembroGrupoRepository>();

            var useCase = new ActivarMiembroDeGrupoUseCase(
                mockGrupoRepo.Object,
                mockUsuarioRepo.Object,
                mockMiembroRepo.Object
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
                useCase.Handle(Guid.NewGuid(), objetivo.Id, "uid")
            );
        }
        [Fact]
        public async Task Handle_NoEsAdminNiEsMismoUsuario_ThrowsUnauthorized()
        {
            var mockGrupoRepo = new Mock<IGrupoRepository>();
            var mockUsuarioRepo = new Mock<IUsuarioRepository>();
            var mockMiembroRepo = new Mock<IMiembroGrupoRepository>();

            var useCase = new ActivarMiembroDeGrupoUseCase(
                mockGrupoRepo.Object,
                mockUsuarioRepo.Object,
                mockMiembroRepo.Object
            );

            var solicitante = new Usuario { Id = Guid.NewGuid() };
            var objetivo = new Usuario { Id = Guid.NewGuid() };

            var grupo = new Grupo("Test Grupo", Guid.NewGuid()); // ✔️ instancia válida

            mockUsuarioRepo
                .Setup(r => r.GetByFirebaseUidAsync("uid", It.IsAny<CancellationToken>()))
                .ReturnsAsync(solicitante);

            mockUsuarioRepo
                .Setup(r => r.GetByIdAsync(objetivo.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(objetivo);

            mockGrupoRepo
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(grupo);

            mockGrupoRepo
                .Setup(r => r.UsuarioEsAdministradorAsync(It.IsAny<Guid>(), solicitante.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                useCase.Handle(Guid.NewGuid(), objetivo.Id, "uid")
            );
        }

        [Fact]
        public async Task Handle_MiembroNoExiste_ThrowsInvalidOperation()
        {
            var mockGrupoRepo = new Mock<IGrupoRepository>();
            var mockUsuarioRepo = new Mock<IUsuarioRepository>();
            var mockMiembroRepo = new Mock<IMiembroGrupoRepository>();

            var useCase = new ActivarMiembroDeGrupoUseCase(
                mockGrupoRepo.Object,
                mockUsuarioRepo.Object,
                mockMiembroRepo.Object
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
                .ReturnsAsync(new Grupo("grupo test", Guid.NewGuid()));

            mockGrupoRepo
                .Setup(r => r.UsuarioEsAdministradorAsync(It.IsAny<Guid>(), solicitante.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            mockMiembroRepo
                .Setup(r =>
                    r.GetByGrupoYUsuarioAsync(It.IsAny<Guid>(), objetivo.IdUsuario, It.IsAny<CancellationToken>())
                )
                .ReturnsAsync((MiembroGrupo)null);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                useCase.Handle(Guid.NewGuid(), objetivo.Id, "uid")
            );
        }

        [Fact]
        public async Task Handle_MiembroYaActivo_ReturnsTrue()
        {
            var mockGrupoRepo = new Mock<IGrupoRepository>();
            var mockUsuarioRepo = new Mock<IUsuarioRepository>();
            var mockMiembroRepo = new Mock<IMiembroGrupoRepository>();

            var useCase = new ActivarMiembroDeGrupoUseCase(
                mockGrupoRepo.Object,
                mockUsuarioRepo.Object,
                mockMiembroRepo.Object
            );

            var solicitante = new Usuario { Id = Guid.NewGuid() };
            var objetivo = new Usuario { Id = Guid.NewGuid() };

            mockUsuarioRepo.Setup(r => r.GetByFirebaseUidAsync("uid", It.IsAny<CancellationToken>()))
                           .ReturnsAsync(solicitante);

            mockUsuarioRepo.Setup(r => r.GetByIdAsync(objetivo.Id, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(objetivo);

            mockGrupoRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(new Grupo("test", Guid.NewGuid()));

            mockGrupoRepo.Setup(r => r.UsuarioEsAdministradorAsync(It.IsAny<Guid>(), solicitante.Id, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(true);

            var miembro = new MiembroGrupo(Guid.NewGuid(), objetivo.Id);
            miembro.afectarRecomendacion = true; // ya está activo

            mockMiembroRepo.Setup(r =>
                r.GetByGrupoYUsuarioAsync(It.IsAny<Guid>(), objetivo.IdUsuario, It.IsAny<CancellationToken>()))
                .ReturnsAsync(miembro);

            var result = await useCase.Handle(Guid.NewGuid(), objetivo.Id, "uid");

            Assert.True(result);
        }

        [Fact]
        public async Task Handle_MiembroInactivo_ActivaYReturnTrue()
        {
            var mockGrupoRepo = new Mock<IGrupoRepository>();
            var mockUsuarioRepo = new Mock<IUsuarioRepository>();
            var mockMiembroRepo = new Mock<IMiembroGrupoRepository>();

            var useCase = new ActivarMiembroDeGrupoUseCase(
                mockGrupoRepo.Object,
                mockUsuarioRepo.Object,
                mockMiembroRepo.Object
            );

            var solicitante = new Usuario { Id = Guid.NewGuid() };
            var objetivo = new Usuario { Id = Guid.NewGuid() };

            mockUsuarioRepo.Setup(r => r.GetByFirebaseUidAsync("uid", It.IsAny<CancellationToken>()))
                           .ReturnsAsync(solicitante);

            mockUsuarioRepo.Setup(r => r.GetByIdAsync(objetivo.Id, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(objetivo);

            mockGrupoRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(new Grupo("Test", Guid.NewGuid()));

            mockGrupoRepo.Setup(r => r.UsuarioEsAdministradorAsync(It.IsAny<Guid>(), solicitante.Id, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(true);

            // Miembro inactivo -> afectarRecomendacion = false
            mockMiembroRepo.Setup(r =>
                r.GetByGrupoYUsuarioAsync(It.IsAny<Guid>(), objetivo.IdUsuario, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new MiembroGrupo(Guid.NewGuid(), objetivo.Id)
                {
                    afectarRecomendacion = false
                });

            mockMiembroRepo.Setup(r =>
                r.ActivarMiembro(It.IsAny<Guid>(), objetivo.Id))
                .ReturnsAsync(true);

            var result = await useCase.Handle(Guid.NewGuid(), objetivo.Id, "uid");

            Assert.True(result);

            mockMiembroRepo.Verify(r =>
                r.ActivarMiembro(It.IsAny<Guid>(), objetivo.Id),
                Times.Once);
        }


    }
}
