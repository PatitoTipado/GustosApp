using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using GustosApp.Application.Common.Exceptions;
using GustosApp.Application.UseCases;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using GustosApp.Domain.Model.@enum;
using Moq;

namespace GustosApp.Application.Tests
{
    public class CrearGrupoUseCaseTests
    {
        private readonly Mock<IGrupoRepository> _grupoRepo;
        private readonly Mock<IMiembroGrupoRepository> _miembroRepo;
        private readonly Mock<IUsuarioRepository> _usuarioRepo;
        private readonly Mock<IGustosGrupoRepository> _gustosGrupoRepo;

        public CrearGrupoUseCaseTests()
        {
            _grupoRepo = new Mock<IGrupoRepository>();
            _miembroRepo = new Mock<IMiembroGrupoRepository>();
            _usuarioRepo = new Mock<IUsuarioRepository>();
            _gustosGrupoRepo = new Mock<IGustosGrupoRepository>();
        }

        private CrearGrupoUseCase CreateUseCase() =>
            new CrearGrupoUseCase(
                _grupoRepo.Object,
                _miembroRepo.Object,
                _usuarioRepo.Object,
                _gustosGrupoRepo.Object
            );


        [Fact]
        public async Task HandleAsync_UsuarioNoExiste_DeberiaLanzarUnauthorized()
        {
            _usuarioRepo
                .Setup(r => r.GetByFirebaseUidAsync("uid", It.IsAny<CancellationToken>()))
                .ReturnsAsync((Usuario?)null);

            var useCase = CreateUseCase();

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                useCase.HandleAsync("uid", "Grupo 1", "Desc", CancellationToken.None));
        }

    
        [Fact]
        public async Task HandleAsync_UsuarioFreeCon3Grupos_DeberiaLanzarLimite()
        {
            var usuario = new Usuario
            {
                Id = Guid.NewGuid(),
                FirebaseUid = "uid",

                Gustos = new List<Gusto>
                           {
                               new Gusto { Nombre = "pizza" }
                           },

                Plan = PlanUsuario.Free,

            };

            _usuarioRepo
                .Setup(r => r.GetByFirebaseUidAsync("uid", It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            _grupoRepo
                .Setup(r => r.GetGruposByUsuarioIdAsync(usuario.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Grupo> {   new Grupo("Grupo 1", Guid.NewGuid(), "desc"),
                                  new Grupo("Grupo 2", Guid.NewGuid(), "desc"),
                                  new Grupo("Grupo 3", Guid.NewGuid(), "desc")
 });

            var useCase = CreateUseCase();

            await Assert.ThrowsAsync<LimiteGruposAlcanzadoException>(() =>
                useCase.HandleAsync("uid", "Grupo", "Desc", CancellationToken.None));
        }

        [Fact]
        public async Task HandleAsync_UsuarioFreeConMenosDe3Grupos_DeberiaCrearGrupo()
        {
            var usuario = new Usuario
            {
                Id = Guid.NewGuid(),
                FirebaseUid = "uid",
                Plan = PlanUsuario.Free,
                Gustos = new List<Gusto>
                   {
                       new Gusto { Nombre = "pizza" }
                   }
            };

            var grupoCreado = new Grupo("Grupo Test", usuario.Id, "Desc");
            var miembroCreado = new MiembroGrupo(grupoCreado.Id, usuario.Id, true);

            _usuarioRepo
                .Setup(r => r.GetByFirebaseUidAsync("uid", It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            _grupoRepo
                .Setup(r => r.GetGruposByUsuarioIdAsync(usuario.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Grupo> { new Grupo("Grupo 1", Guid.NewGuid(), "desc") });

            _grupoRepo
                .Setup(r => r.CreateAsync(It.IsAny<Grupo>(), It.IsAny<CancellationToken>()))
                .Callback<Grupo, CancellationToken>((g, _) => grupoCreado = g)
                .ReturnsAsync(grupoCreado); // <-- fix: use ReturnsAsync

            _miembroRepo
                .Setup(r => r.CreateAsync(It.IsAny<MiembroGrupo>(), It.IsAny<CancellationToken>()))
                .Callback<MiembroGrupo, CancellationToken>((m, _) => miembroCreado = m)
                .ReturnsAsync(miembroCreado); // <-- fix: use ReturnsAsync

            _gustosGrupoRepo
                .Setup(r => r.AgregarGustosAlGrupo(grupoCreado.Id, usuario.Gustos.ToList(), miembroCreado.Id))
                .ReturnsAsync(true);

            _grupoRepo
     .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
     .ReturnsAsync((Guid id, CancellationToken _) => id == grupoCreado.Id ? grupoCreado : null);

            var useCase = CreateUseCase();

            var result = await useCase.HandleAsync("uid", "Grupo Test", "Desc Test", CancellationToken.None);

            result.Should().NotBeNull();
            result.Nombre.Should().Be("Grupo Test");

            _miembroRepo.Verify(r => r.CreateAsync(
                It.Is<MiembroGrupo>(m => m.EsAdministrador == true && m.UsuarioId == usuario.Id),
                It.IsAny<CancellationToken>()),
                Times.Once);

            _gustosGrupoRepo.Verify(r => r.AgregarGustosAlGrupo(
                grupoCreado.Id,
                usuario.Gustos.ToList(),
                miembroCreado.Id),
                Times.Once);
        }
   
        [Fact]
        public async Task HandleAsync_UsuarioPremium_NoDebeVerificarLimite()
        {
            var usuario = new Usuario
            {
                Id = Guid.NewGuid(),
                FirebaseUid = "uid",
                Plan = PlanUsuario.Plus,
                  Gustos = new List<Gusto>
                           {
                               new Gusto { Nombre = "pizza" }
                           }
            };

            var grupoCreado = new Grupo("Grupo Premium", usuario.Id, "Desc P");

            _usuarioRepo.Setup(r => r.GetByFirebaseUidAsync("uid", It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            _grupoRepo
                .Setup(r => r.CreateAsync(It.IsAny<Grupo>(), It.IsAny<CancellationToken>()))
                .Callback<Grupo, CancellationToken>((g, _) => grupoCreado = g)
                .ReturnsAsync(grupoCreado);

            _grupoRepo
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(grupoCreado);

            var useCase = CreateUseCase();

            var result = await useCase.HandleAsync("uid", "Grupo Premium", "Desc", CancellationToken.None);

            result.Should().NotBeNull();
            result.Nombre.Should().Be("Grupo Premium");

            _grupoRepo.Verify(r => r.GetGruposByUsuarioIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        }

      
        [Fact]
        public async Task HandleAsync_ErrorAlObtenerGrupoFinal_DeberiaLanzarInvalidOperationException()
        {
            var usuario = new Usuario
            {
                Id = Guid.NewGuid(),
                FirebaseUid = "uid",
                Plan = PlanUsuario.Plus
            };

            var grupoTemporal = new Grupo("Grupo X", usuario.Id, "Desc");

            _usuarioRepo
                .Setup(r => r.GetByFirebaseUidAsync("uid", It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            _grupoRepo
                .Setup(r => r.CreateAsync(It.IsAny<Grupo>(), It.IsAny<CancellationToken>()))
                .Callback<Grupo, CancellationToken>((g, _) => grupoTemporal = g)
                .ReturnsAsync(grupoTemporal);

            _grupoRepo
                .Setup(r => r.GetByIdAsync(grupoTemporal.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Grupo?)null);

            var useCase = CreateUseCase();

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                useCase.HandleAsync("uid", "Grupo X", "Desc", CancellationToken.None));
        }
    }
}
