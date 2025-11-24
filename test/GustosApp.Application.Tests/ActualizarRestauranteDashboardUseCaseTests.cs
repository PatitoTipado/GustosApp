using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GustosApp.Application.UseCases.RestauranteUseCases;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using Moq;
using Xunit;

namespace GustosApp.Application.Tests
{
    public class ActualizarRestauranteDashboardUseCaseTests
    {
        private readonly Mock<IRestauranteRepository> _restauranteRepositoryMock;
        private readonly Mock<IGustoRepository> _gustoRepositoryMock;
        private readonly Mock<IRestriccionRepository> _restriccionRepositoryMock;
        private readonly ActualizarRestauranteDashboardUseCase _sut;

        public ActualizarRestauranteDashboardUseCaseTests()
        {
            _restauranteRepositoryMock = new Mock<IRestauranteRepository>();
            _gustoRepositoryMock = new Mock<IGustoRepository>();
            _restriccionRepositoryMock = new Mock<IRestriccionRepository>();

            _sut = new ActualizarRestauranteDashboardUseCase(
                _restauranteRepositoryMock.Object,
                _gustoRepositoryMock.Object,
                _restriccionRepositoryMock.Object);
        }

        private Restaurante CreateRestaurante(Guid? id = null)
        {
            var now = DateTime.UtcNow.AddDays(-1);

            return new Restaurante
            {
                Id = id ?? Guid.NewGuid(),
                Direccion = "Dirección original",
                Latitud = -34.0,
                Longitud = -58.0,
                HorariosJson = "{\"original\":\"schedule\"}",
                WebUrl = "https://original.example.com",
                GustosQueSirve = new List<Gusto>(),
                RestriccionesQueRespeta = new List<Restriccion>(),
                ActualizadoUtc = now,
                UltimaActualizacion = now
            };
        }


        [Fact]
        public async Task HandleAsync_RestauranteNoExiste_LanzaInvalidOperationException()
        {
   
            var restauranteId = Guid.NewGuid();

            _restauranteRepositoryMock
                .Setup(r => r.GetByIdAsync(restauranteId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Restaurante?)null);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _sut.HandleAsync(
                    restauranteId,
                    direccion: "Nueva dirección",
                    latitud: -35.0,
                    longitud: -59.0,
                    horariosJson: "{}",
                    webUrl: "https://nuevo.example.com",
                    gustosQueSirveIds: null,
                    restriccionesQueRespetaIds: null,
                    ct: CancellationToken.None));

            Assert.Equal("El restaurante no existe.", ex.Message);
        }

    
        [Fact]
        public async Task HandleAsync_ActualizaCamposBasicos_CuandoSeEnvianValores()
        {
            var restauranteId = Guid.NewGuid();
            var restaurante = CreateRestaurante(restauranteId);

            var oldActualizado = restaurante.ActualizadoUtc;
            var oldUltimaActualizacion = restaurante.UltimaActualizacion;

            _restauranteRepositoryMock
                .Setup(r => r.GetByIdAsync(restauranteId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(restaurante);

            
            var result = await _sut.HandleAsync(
                restauranteId,
                direccion: "Nueva dirección",
                latitud: -35.123,
                longitud: -60.456,
                horariosJson: "{\"nuevo\":\"schedule\"}",
                webUrl: "https://nuevo.example.com",
                gustosQueSirveIds: null,
                restriccionesQueRespetaIds: null,
                ct: CancellationToken.None);

            Assert.Same(restaurante, result);

            Assert.Equal("Nueva dirección", restaurante.Direccion);
            Assert.Equal(-35.123, restaurante.Latitud);
            Assert.Equal(-60.456, restaurante.Longitud);
            Assert.Equal("{\"nuevo\":\"schedule\"}", restaurante.HorariosJson);
            Assert.Equal("https://nuevo.example.com", restaurante.WebUrl);

            Assert.True(restaurante.ActualizadoUtc > oldActualizado);
            Assert.True(restaurante.UltimaActualizacion > oldUltimaActualizacion);

            _gustoRepositoryMock.Verify(
                g => g.GetByIdsAsync(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()),
                Times.Never);

            _restriccionRepositoryMock.Verify(
                r => r.GetRestriccionesByIdsAsync(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()),
                Times.Never);

            _restauranteRepositoryMock.Verify(
                r => r.UpdateAsync(restaurante, It.IsAny<CancellationToken>()),
                Times.Once);

            _restauranteRepositoryMock.Verify(
                r => r.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        }


        [Fact]
        public async Task HandleAsync_WebUrlVacia_LaEstableceEnNull()
        {
            var restauranteId = Guid.NewGuid();
            var restaurante = CreateRestaurante(restauranteId);
            restaurante.WebUrl = "https://original.example.com";

            _restauranteRepositoryMock
                .Setup(r => r.GetByIdAsync(restauranteId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(restaurante);

            await _sut.HandleAsync(
                restauranteId,
                direccion: null,
                latitud: null,
                longitud: null,
                horariosJson: null,
                webUrl: "", 
                gustosQueSirveIds: null,
                restriccionesQueRespetaIds: null,
                ct: CancellationToken.None);

            Assert.Null(restaurante.WebUrl);
        }

        // --------------------------------------------------------------------
        // 4. Cuando se envían gustosQueSirveIds, se consulta repositorio y se
        //    aplican los gustos al restaurante
        // --------------------------------------------------------------------
       /* [Fact]
        public async Task HandleAsync_GustosIds_ActualizaGustosDelRestaurante()
        {
            // Arrange
            var restauranteId = Guid.NewGuid();
            var restaurante = CreateRestaurante(restauranteId);

            restaurante.GustosQueSirve = new List<Gusto>
            {
                new Gusto { Nombre = "Viejo gusto" }
            };

            var gustoId = Guid.NewGuid();
            var gustosIds = new List<Guid> { gustoId };

            var nuevosGustos = new List<Gusto>
            {
                new Gusto { Nombre = "Nuevo gusto" }
            };

            _restauranteRepositoryMock
                .Setup(r => r.GetByIdAsync(restauranteId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(restaurante);

            _gustoRepositoryMock
                .Setup(g => g.GetByIdsAsync(gustosIds, It.IsAny<CancellationToken>()))
                .ReturnsAsync(nuevosGustos);

            // Act
            await _sut.HandleAsync(
                restauranteId,
                direccion: null,
                latitud: null,
                longitud: null,
                horariosJson: null,
                webUrl: null,
                gustosQueSirveIds: gustosIds,
                restriccionesQueRespetaIds: null,
                ct: CancellationToken.None);

            // Assert: se llamó al repo con los IDs correctos
            _gustoRepositoryMock.Verify(
                g => g.GetByIdsAsync(
                    It.Is<List<Guid>>(l => l.Count == 1 && l[0] == gustoId),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            // Y el restaurante ahora refleja los nuevos gustos
            Assert.Single(restaurante.GustosQueSirve);
            Assert.Equal("Nuevo gusto", restaurante.GustosQueSirve[0].Nombre);
        }*/

        // --------------------------------------------------------------------
        // 5. Si gustosQueSirveIds es una lista vacía, se limpian los gustos
        //    y no se llama al repositorio
        // --------------------------------------------------------------------
        [Fact]
        public async Task HandleAsync_GustosListaVacia_LimpiaGustosYNoLlamaRepo()
        {
            var restauranteId = Guid.NewGuid();
            var restaurante = CreateRestaurante(restauranteId);

            restaurante.GustosQueSirve = new List<Gusto>
            {
                new Gusto { Nombre = "Gusto 1" }
            };

            var gustosIds = new List<Guid>(); 

            _restauranteRepositoryMock
                .Setup(r => r.GetByIdAsync(restauranteId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(restaurante);

            await _sut.HandleAsync(
                restauranteId,
                direccion: null,
                latitud: null,
                longitud: null,
                horariosJson: null,
                webUrl: null,
                gustosQueSirveIds: gustosIds,
                restriccionesQueRespetaIds: null,
                ct: CancellationToken.None);

            _gustoRepositoryMock.Verify(
                g => g.GetByIdsAsync(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()),
                Times.Never);

            Assert.Empty(restaurante.GustosQueSirve);
        }

        // --------------------------------------------------------------------
        // 6. Cuando se envían restriccionesQueRespetaIds, se consultan y se
        //    aplican al restaurante
        // --------------------------------------------------------------------
       /* [Fact]
        public async Task HandleAsync_RestriccionesIds_ActualizaRestriccionesDelRestaurante()
        {
            // Arrange
            var restauranteId = Guid.NewGuid();
            var restaurante = CreateRestaurante(restauranteId);

            restaurante.RestriccionesQueRespeta = new List<Restriccion>
            {
                new Restriccion { Nombre = "Vieja restricción" }
            };

            var restriccionId = Guid.NewGuid();
            var restriccionesIds = new List<Guid> { restriccionId };

            var nuevasRestricciones = new List<Restriccion>
            {
                new Restriccion { Nombre = "Nueva restricción" }
            };

            _restauranteRepositoryMock
                .Setup(r => r.GetByIdAsync(restauranteId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(restaurante);

            _restriccionRepositoryMock
                .Setup(r => r.GetRestriccionesByIdsAsync(restriccionesIds, It.IsAny<CancellationToken>()))
                .ReturnsAsync(nuevasRestricciones);

            // Act
            await _sut.HandleAsync(
                restauranteId,
                direccion: null,
                latitud: null,
                longitud: null,
                horariosJson: null,
                webUrl: null,
                gustosQueSirveIds: null,
                restriccionesQueRespetaIds: restriccionesIds,
                ct: CancellationToken.None);

            // Assert
            _restriccionRepositoryMock.Verify(
                r => r.GetRestriccionesByIdsAsync(
                    It.Is<List<Guid>>(l => l.Count == 1 && l[0] == restriccionId),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            Assert.Single(restaurante.RestriccionesQueRespeta);
            Assert.Equal("Nueva restricción", restaurante.RestriccionesQueRespeta[0].Nombre);
        }*/

        // --------------------------------------------------------------------
        // 7. Si restriccionesQueRespetaIds es lista vacía, limpia las
        //    restricciones y no llama al repositorio
        // --------------------------------------------------------------------
        [Fact]
        public async Task HandleAsync_RestriccionesListaVacia_LimpiaRestriccionesYNoLlamaRepo()
        {
            var restauranteId = Guid.NewGuid();
            var restaurante = CreateRestaurante(restauranteId);

            restaurante.RestriccionesQueRespeta = new List<Restriccion>
            {
                new Restriccion { Nombre = "Restricción 1" }
            };

            var restriccionesIds = new List<Guid>(); 

            _restauranteRepositoryMock
                .Setup(r => r.GetByIdAsync(restauranteId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(restaurante);

            await _sut.HandleAsync(
                restauranteId,
                direccion: null,
                latitud: null,
                longitud: null,
                horariosJson: null,
                webUrl: null,
                gustosQueSirveIds: null,
                restriccionesQueRespetaIds: restriccionesIds,
                ct: CancellationToken.None);

            _restriccionRepositoryMock.Verify(
                r => r.GetRestriccionesByIdsAsync(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()),
                Times.Never);

            Assert.Empty(restaurante.RestriccionesQueRespeta);
        }
    }
}
