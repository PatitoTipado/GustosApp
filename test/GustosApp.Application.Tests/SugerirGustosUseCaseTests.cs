using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GustosApp.Application.UseCases;
using GustosApp.Application.DTO;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using Moq;
using Xunit;
using Microsoft.VisualBasic;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace GustosApp.Application.Tests
{
    public class SugerirGustosUseCaseTests
    {

        [Fact]
        public async Task HandleAsync_DeberiaDevolverSugerenciasPorCoocurrencia()
        {
            var usuario = new Usuario("uid1", "u1@gmail", "Nombre", "Apellido", "idUsuario") { };

            var gustoA = new Gusto { Id = Guid.NewGuid(), Nombre = "Pizza" };
            var gustoB = new Gusto { Id = Guid.NewGuid(), Nombre = "Shushi" };
            var gustoC = new Gusto { Id = Guid.NewGuid(), Nombre = "Ensalada" };

            usuario.Gustos.Add(gustoA);

            var other1 = new Usuario("uid2", "u2@gmail", "Nombre2", "Apellido2", "idUsuario2") { };
            other1.Gustos.Add(gustoA);
            other1.Gustos.Add(gustoB);

            var other2 = new Usuario("uid3", "u3@gmail", "Nombre3", "Apellido3", "idUsuario3") { };
            other2.Gustos.Add(gustoA);
            other2.Gustos.Add(gustoC);

            var usuarios = new List<Usuario> { usuario, other1, other2 };

            var mockUserRepo = new Mock<IUsuarioRepository>();
            mockUserRepo.Setup(r => r.GetByFirebaseUidWithGustosAsync("uid1", It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);
            mockUserRepo.Setup(r => r.GetAllWithGustosAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuarios);

            var mockGustoRepo = new Mock<IGustoRepository>();
            mockGustoRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                         .ReturnsAsync(new List<Gusto> { gustoA, gustoB, gustoC });

            var useCase = new SugerirGustosUseCase(mockUserRepo.Object, mockGustoRepo.Object);

            var result = await useCase.HandleAsync("uid1", top: 5);

            Assert.NotNull(result);
            Assert.Contains(result, g => g.nombre == "Shushi");
            Assert.Contains(result, g => g.nombre == "Ensalada");
        }

        [Fact]
        public async Task HandleAsync_FallBackSiNoHaySimilares_DevuelvePopulares()
        {
            var usuario = new Usuario("uidX", "x@gmail", "N", "A", "idX");
            var gustoA = new Gusto { Id = Guid.NewGuid(), Nombre = "Pizza" };
            var gustoB = new Gusto { Id = Guid.NewGuid(), Nombre = "Shushi" };

            var u1 = new Usuario("u1", "a@gmail", "N1", "A1", "id1"); u1.Gustos.Add(gustoA);
            var u2 = new Usuario("u2", "b@gmail", "N2", "A2", "id2"); u2.Gustos.Add(gustoA); u2.Gustos.Add(gustoB);

            var usuarios = new List<Usuario> { usuario, u1, u2 };

            var mockUserRepo = new Mock<IUsuarioRepository>();
            mockUserRepo.Setup(r => r.GetByFirebaseUidWithGustosAsync("uidX", It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            mockUserRepo.Setup(r => r.GetAllWithGustosAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuarios);

            var mockGustoRepo = new Mock<IGustoRepository>();
            mockGustoRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Gusto> { gustoA, gustoB });

            var useCase = new SugerirGustosUseCase(mockUserRepo.Object, mockGustoRepo.Object);
            var result = await useCase.HandleAsync("uidX", top: 2);

            Assert.NotNull(result);
            Assert.True(result.Count > 0);
        }
    }
}