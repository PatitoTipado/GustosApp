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
    public class ObtenerGustosPaginacionUseCaseTest
    {
        [Fact]
        public async Task HandleAsync_ObtenerLosGustosDeLaPag()
        {
            const int cantidadInicio = 5;
            const int final = 10;

            var resultadosEsperados = new List<Gusto>
            {
                new Gusto { Id = Guid.NewGuid(), Nombre = "Comida China" },
                new Gusto { Id = Guid.NewGuid(), Nombre = "Shusi" }
            };

            var mockGustoRepo = new Mock<IGustoRepository>();

            var mockUsuarioRepo = new Mock<IUsuarioRepository>();

            mockGustoRepo.Setup(r => r.obtenerGustosPorPaginacion(
                cantidadInicio,
                final)
            )
            .Returns(resultadosEsperados);

            var casoDeUso = new ObtenerGustosPaginacionUseCase(
                mockUsuarioRepo.Object,
                mockGustoRepo.Object
            );

            var respuestaReal = await casoDeUso.HandleAsync(cantidadInicio, final);

            Assert.NotNull(respuestaReal);
            Assert.Equal(resultadosEsperados.Count, respuestaReal.Count);
            Assert.Equal(resultadosEsperados, respuestaReal);

            mockGustoRepo.Verify(r => r.obtenerGustosPorPaginacion(
                cantidadInicio,
                final
            ),
            Times.Once,
            "El caso de uso debe delegar la llamada al repositorio con los parametros de paginacion exactamente una vez."
            );
        }
    }
}
