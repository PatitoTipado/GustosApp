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
    public class BuscarRestaurantesUseCaseTest
    {
        [Fact]
        public async Task HandleAsync_LlamaAlRepositorioConTextoCorrecto()
        {
            var restauranteMock = new Mock<IRestauranteRepository>();
            var casoDeUso = new BuscarRestaurantesUseCase(restauranteMock.Object);

            var textoBusqueda = "pi";

            restauranteMock
                .Setup(r => r.BuscarPorPrefijo(textoBusqueda, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Restaurante> { new Restaurante { Nombre = "Pizzeria Juan" }});

            var resultado = await casoDeUso.HandleAsync(textoBusqueda, CancellationToken.None);

            Assert.Single(resultado);
            Assert.Equal("Pizzeria Juan", resultado[0].Nombre);

            restauranteMock.Verify(r => r.BuscarPorPrefijo(textoBusqueda, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_SinResultados_RetornaListaVacia()
        {
            var repoMock = new Mock<IRestauranteRepository>();
            var useCase = new BuscarRestaurantesUseCase(repoMock.Object);

            repoMock.Setup(r => r.BuscarPorPrefijo("x", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Restaurante>());

            var result = await useCase.HandleAsync("x", CancellationToken.None);

            Assert.Empty(result);
        }




    }
}
