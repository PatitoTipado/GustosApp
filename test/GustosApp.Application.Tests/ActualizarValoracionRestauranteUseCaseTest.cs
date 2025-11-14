using GustosApp.Application.Interfaces;
using GustosApp.Application.UseCases;
using GustosApp.Application.UseCases.RestauranteUseCases;
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
    public class ActualizarValoracionRestauranteUseCaseTest
    {
        [Fact]
        public async Task PromedioDeValoraciones()
        {
            var repoValoracion = new Mock<IOpinionRestauranteRepository>();
            var repoRestaurante = new Mock<IRestauranteRepository>();

            var restauranteId = Guid.NewGuid();
            var valoraciones = new List<OpinionRestaurante>
            {
                new OpinionRestaurante(Guid.NewGuid(), restauranteId, 3),
                new OpinionRestaurante(Guid.NewGuid(), restauranteId, 4),
                new OpinionRestaurante(Guid.NewGuid(), restauranteId, 5)
            };

            repoValoracion.Setup(r => r.ObtenerPorRestauranteAsync(restauranteId, It.IsAny<CancellationToken>()))
                               .ReturnsAsync(valoraciones);

            var useCase = new ActualizarValoracionRestauranteUseCase(repoRestaurante.Object, repoValoracion.Object);

            await useCase.HandleAsync(restauranteId, CancellationToken.None);

            repoRestaurante.Verify(
                        r => r.ActualizarValoracionAsync(restauranteId, 4.0, It.IsAny<CancellationToken>()),
                        Times.Once);
        }
    }
}
