using GustosApp.Application.Interfaces;
using GustosApp.Application.UseCases.UsuarioUseCases;
using GustosApp.Application.UseCases.UsuarioUseCases.GustoUseCases;
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
    public class BuscarGustoPorCoincidenciaUseCaseTest
    {
        [Fact]
        public async Task HandleAsync_DebeLlamarAlRepositorioConElNombreCorrecto()
        {
            var repo = new Mock<IGustoRepository>();

            repo.Setup(r => r.ObtenerGustoPorCoincidencia("pizza"))
                .Returns(new List<Gusto>());

            var useCase = new BuscarGustoPorCoincidenciaUseCase(repo.Object);

            await useCase.HandleAsync("pizza");

            repo.Verify(r => r.ObtenerGustoPorCoincidencia("pizza"), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_DebeRetornarResultadosDelRepositorio()
        {
            var resultados = new List<Gusto>
            {
                new Gusto { Id = Guid.NewGuid(), Nombre = "Pizza" },
                new Gusto { Id = Guid.NewGuid(), Nombre = "Milanesa" }
            };

            var gustoRepo = new Mock<IGustoRepository>();

            gustoRepo.Setup(r => r.ObtenerGustoPorCoincidencia(It.IsAny<string>()))
         .Returns(resultados);

            var casoDeUso = new BuscarGustoPorCoincidenciaUseCase(gustoRepo.Object);

            var respuesta = await casoDeUso.HandleAsync("Pizza");

            Assert.Equal(resultados, respuesta);
            Assert.Equal("Pizza", respuesta[0].Nombre);
            Assert.Equal("Milanesa", respuesta[1].Nombre);

        }

        [Fact]
        public async Task HandleAsync_SinCoincidencias_RetornaListaVacia()
        {
            var repo = new Mock<IGustoRepository>();

            repo.Setup(r => r.ObtenerGustoPorCoincidencia("xyz"))
                .Returns(new List<Gusto>());

            var useCase = new BuscarGustoPorCoincidenciaUseCase(repo.Object);

            var result = await useCase.HandleAsync("xyz");

            Assert.Empty(result);
        }

    }
}
