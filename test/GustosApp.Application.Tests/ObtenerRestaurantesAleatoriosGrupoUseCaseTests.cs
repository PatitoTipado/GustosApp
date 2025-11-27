using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GustosApp.Application.Model;
using GustosApp.Application.UseCases.RestauranteUseCases;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using Moq;
using Xunit;

namespace GustosApp.Application.Tests
{
    public class ObtenerRestaurantesAleatoriosGrupoUseCaseTests
    {
        private readonly Mock<IGustosGrupoRepository> _gustosGrupoRepositoryMock;
        private readonly Mock<IRestauranteRepository> _restauranteRepositoryMock;
        private readonly Mock<IGrupoRepository> _grupoRepositoryMock;
        private readonly ObtenerRestaurantesAleatoriosGrupoUseCase _sut;

        public ObtenerRestaurantesAleatoriosGrupoUseCaseTests()
        {
            _gustosGrupoRepositoryMock = new Mock<IGustosGrupoRepository>();
            _restauranteRepositoryMock = new Mock<IRestauranteRepository>();
            _grupoRepositoryMock = new Mock<IGrupoRepository>();

            _sut = new ObtenerRestaurantesAleatoriosGrupoUseCase(
                _gustosGrupoRepositoryMock.Object,
                _restauranteRepositoryMock.Object,
                _grupoRepositoryMock.Object);
        }

        private static ObtenerRestaurantesAleatoriosRequestModel CreateRequest(
            int cantidad = 3,
            double? lat = null,
            double? lng = null,
            int? radioMetros = null)
        {
            return new ObtenerRestaurantesAleatoriosRequestModel
            {
                Cantidad = cantidad,
                Latitud = lat,
                Longitud = lng,
                RadioMetros = radioMetros
            };
        }

        private static Restaurante CreateRestaurante(
    Guid? id = null,
    decimal lat = 0,               
    decimal lng = 0,              
    string nombre = "Restaurante",
    string categoria = "Categoria")
{
    return new Restaurante
    {
        Id = id ?? Guid.NewGuid(),
        Nombre = nombre,
        Direccion = "Alguna dirección",
        Latitud = (double)lat,             
        Longitud = (double)lng,            

        Rating = 0,                
        CantidadResenas = 0,
        Valoracion = 0,

        Categoria = categoria,
        ImagenUrl = "https://example.com/img.jpg",
        WebUrl = "https://example.com",
        PlaceId = "place_123",

        GustosQueSirve = new List<Gusto>
        {
            new Gusto { Nombre = "Pizza" },
            new Gusto { Nombre = "Pasta" }
        },
        RestriccionesQueRespeta = new List<Restriccion>
        {
            new Restriccion { Nombre = "Vegano" }
        }
    };
}


        private static Grupo CreateGrupo()
        {
            return (Grupo)Activator.CreateInstance(typeof(Grupo), nonPublic: true)!;
        }


        [Fact]
        public async Task HandleAsync_GrupoNoExiste_LanzaArgumentException()
        {
            
            var grupoId = Guid.NewGuid();
            var request = CreateRequest();

            _grupoRepositoryMock
                .Setup(g => g.GetByIdAsync(grupoId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Grupo?)null);

            var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
                _sut.HandleAsync(grupoId, request, CancellationToken.None));

            Assert.StartsWith("El grupo no existe", ex.Message);
            Assert.Equal("grupoId", ex.ParamName);

            _gustosGrupoRepositoryMock.Verify(
                g => g.ObtenerGustosIdsDelGrupo(It.IsAny<Guid>()),
                Times.Never);

            _restauranteRepositoryMock.Verify(
                r => r.ObtenerRestaurantesPorGustosGrupo(
                    It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

      
        [Fact]
        public async Task HandleAsync_SinGustosEnGrupo_DevuelveListaVaciaYNoLlamaRestaurantes()
        {
            var grupoId = Guid.NewGuid();
            var request = CreateRequest();

            _grupoRepositoryMock
                .Setup(g => g.GetByIdAsync(grupoId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateGrupo());

            _gustosGrupoRepositoryMock
                .Setup(g => g.ObtenerGustosIdsDelGrupo(grupoId))
                .ReturnsAsync(new List<Guid>());

            var result = await _sut.HandleAsync(grupoId, request, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Empty(result);

            _restauranteRepositoryMock.Verify(
                r => r.ObtenerRestaurantesPorGustosGrupo(
                    It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

       
        [Fact]
        public async Task HandleAsync_SinRestaurantesParaGustos_DevuelveListaVacia()
        {
            var grupoId = Guid.NewGuid();
            var request = CreateRequest();

            var gustosIds = new List<Guid> { Guid.NewGuid() };

            _grupoRepositoryMock
                .Setup(g => g.GetByIdAsync(grupoId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateGrupo());

            _gustosGrupoRepositoryMock
                .Setup(g => g.ObtenerGustosIdsDelGrupo(grupoId))
                .ReturnsAsync(gustosIds);

            _restauranteRepositoryMock
                .Setup(r => r.ObtenerRestaurantesPorGustosGrupo(gustosIds, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Restaurante>());

            var result = await _sut.HandleAsync(grupoId, request, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Empty(result);

            _restauranteRepositoryMock.Verify(
                r => r.ObtenerRestaurantesPorGustosGrupo(
                    It.Is<List<Guid>>(l => l.SequenceEqual(gustosIds)),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        
        [Fact]
        public async Task HandleAsync_SinFiltroGeografico_DevuelveCantidadSolicitadaOMenor()
        {
            var grupoId = Guid.NewGuid();
            var ct = CancellationToken.None;
            var request = CreateRequest(cantidad: 2); 

            var gustosIds = new List<Guid> { Guid.NewGuid() };

            var r1 = CreateRestaurante(nombre: "R1");
            var r2 = CreateRestaurante(nombre: "R2");
            var r3 = CreateRestaurante(nombre: "R3");

            var restaurantes = new List<Restaurante> { r1, r2, r3 };

            _grupoRepositoryMock
                .Setup(g => g.GetByIdAsync(grupoId, ct))
                .ReturnsAsync(CreateGrupo());

            _gustosGrupoRepositoryMock
                .Setup(g => g.ObtenerGustosIdsDelGrupo(grupoId))
                .ReturnsAsync(gustosIds);

            _restauranteRepositoryMock
                .Setup(r => r.ObtenerRestaurantesPorGustosGrupo(gustosIds, ct))
                .ReturnsAsync(restaurantes);

            var result = await _sut.HandleAsync(grupoId, request, ct);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count); 

            var idsOriginales = restaurantes.Select(x => x.Id).ToHashSet();
            Assert.All(result, dto => Assert.Contains(dto.Id, idsOriginales));

            Assert.All(result, dto =>
            {
                Assert.NotNull(dto.Gustos);
                Assert.Contains("Pizza", dto.Gustos);
                Assert.NotNull(dto.Restricciones);
                Assert.Contains("Vegano", dto.Restricciones);
            });
        }

    
        [Fact]
        public async Task HandleAsync_ConFiltroGeografico_DevuelveSoloRestaurantesDentroDelRadio()
        {
            var grupoId = Guid.NewGuid();
            var ct = CancellationToken.None;

            var request = CreateRequest(
                cantidad: 5,
                lat: 0,
                lng: 0,
                radioMetros: 1000);

            var gustosIds = new List<Guid> { Guid.NewGuid() };

            var rCerca = CreateRestaurante(
                id: Guid.NewGuid(),
                lat: 0,
                lng: 0,
                nombre: "Cerca");

            var rLejos = CreateRestaurante(
                id: Guid.NewGuid(),
                lat: 0,
                lng: 1,
                nombre: "Lejos");

            var restaurantes = new List<Restaurante> { rCerca, rLejos };

            _grupoRepositoryMock
                .Setup(g => g.GetByIdAsync(grupoId, ct))
                .ReturnsAsync(CreateGrupo());

            _gustosGrupoRepositoryMock
                .Setup(g => g.ObtenerGustosIdsDelGrupo(grupoId))
                .ReturnsAsync(gustosIds);

            _restauranteRepositoryMock
                .Setup(r => r.ObtenerRestaurantesPorGustosGrupo(gustosIds, ct))
                .ReturnsAsync(restaurantes);

            var result = await _sut.HandleAsync(grupoId, request, ct);

            Assert.Single(result);
            Assert.Equal(rCerca.Id, result[0].Id);
        }
    }
}

