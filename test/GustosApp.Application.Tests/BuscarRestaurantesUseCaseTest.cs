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
        public async Task BuscarRestaurantes()
        {
            var repoMock = new Mock<IRestauranteRepository>();
            var texto = "Pizza";

            repoMock.Setup(r => r.BuscarPorTextoAsync(texto, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new List<Restaurante>
                        {
                new Restaurante { Nombre = "Pizzeria Juan", Categoria = "Pizzería", Valoracion = 4.5m }
                        });

            var casoDeUso = new BuscarRestaurantesUseCase(repoMock.Object);

            var resultado = await casoDeUso.HandleAsync(texto, CancellationToken.None);

            Assert.NotNull(resultado);
            Assert.Single(resultado);
            Assert.Equal("Pizzeria Juan", resultado[0].Nombre);

            repoMock.Verify(r => r.BuscarPorTextoAsync(texto, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task SiNoSeEscribeNadaDevolverListaVacia()
        {
            var repoMock = new Mock<IRestauranteRepository>();
            var texto = "";

            repoMock.Setup(r => r.BuscarPorTextoAsync(texto, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new List<Restaurante>());

            var casoDeUso = new BuscarRestaurantesUseCase(repoMock.Object);

            var resultado = await casoDeUso.HandleAsync(texto, CancellationToken.None);

            Assert.Empty(resultado);
           
        }

        [Fact]
        public async Task BuscarPorTextoAsync_OrdenaPorCoincidenciaCuandoRatingIgual()
        {
            var textoBusqueda = "cafe";
            var repoMock = new Mock<IRestauranteRepository>();

            var restaurantesSimulados = new List<Restaurante>
            {
                new Restaurante { Nombre = "Comedor Rapido", NombreNormalizado = "comedor rapido", Categoria = "Cafeteria", Rating = 4.0 },
                new Restaurante { Nombre = "Cafe Central", NombreNormalizado = "cafe central", Categoria = "Bistro", Rating = 4.0 },
                new Restaurante { Nombre = "Coffe Shop", NombreNormalizado = "coffee shop", Categoria = "Cafe", Rating = 4.0 }
            };

            repoMock.Setup(r => r.BuscarPorTextoAsync(textoBusqueda, It.IsAny<CancellationToken>()))
                .ReturnsAsync(() =>
                restaurantesSimulados
                .Select(r => new
                {
                    Restaurante = r,
                    Prioridad = r.Nombre != null && r.Nombre.ToLower().Contains(textoBusqueda) ? 4 :
                    (r.NombreNormalizado != null && r.NombreNormalizado.ToLower().Contains(textoBusqueda) ? 3 :
                    (r.Categoria != null && r.Categoria.ToLower() == textoBusqueda ? 2 :
                    (r.Categoria != null && r.Categoria.ToLower().Contains(textoBusqueda) ? 1 : 0)))
                })
                .OrderByDescending(x => x.Restaurante.Rating)
                .ThenByDescending(x => x.Prioridad)
                .Select(x => x.Restaurante)
                .ToList()
                );
            
            var casoDeUso = new BuscarRestaurantesUseCase(repoMock.Object);
            var resultado = await casoDeUso.HandleAsync(textoBusqueda, CancellationToken.None);

            Assert.NotNull(resultado);
            Assert.Equal(3, resultado.Count);

            Assert.Equal("Cafe Central", resultado[0].Nombre); 
            Assert.Equal("Coffe Shop", resultado[1].Nombre);   
            Assert.Equal("Comedor Rapido", resultado[2].Nombre); 
        }


        [Fact]
        public async Task BuscarPorTextoAsync_PriorizaResultados_PorRating()
        {
            var textoBusqueda = "cafe"; 
            var repoMock = new Mock<IRestauranteRepository>();

            var restaurantesSimulados = new List<Restaurante>
            {
                new Restaurante { Nombre = "Comedor Rápido", Categoria = "Cafetería", Rating = 4.1 },
                new Restaurante { Nombre = "Cafe Central", Categoria = "Bistro", Rating = 4.8 }
            };

            repoMock.Setup(r => r.BuscarPorTextoAsync(textoBusqueda, It.IsAny<CancellationToken>()))
                .ReturnsAsync(restaurantesSimulados.Where(r =>
                    r.Nombre.ToLower().Contains(textoBusqueda) ||
                    r.Categoria.ToLower().Contains(textoBusqueda))
                    .OrderByDescending(r => r.Rating) 
                    .ToList());

            var casoDeUso = new BuscarRestaurantesUseCase(repoMock.Object);

            var resultado = await casoDeUso.HandleAsync(textoBusqueda, CancellationToken.None);

            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Count);

            Assert.Equal("Cafe Central", resultado[0].Nombre); 
            Assert.Equal(4.8, resultado[0].Rating); 

            Assert.Equal("Comedor Rápido", resultado[1].Nombre); 
            Assert.Equal(4.1, resultado[1].Rating); 
        }

       



    }
}
