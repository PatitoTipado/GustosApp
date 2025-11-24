using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GustosApp.Application.UseCases.RestauranteUseCases;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace GustosApp.Application.Tests
{
    public class BuscarRestaurantesCercanosUseCaseTests
    {
        private readonly Mock<IRestauranteRepository> _repoMock;
        private readonly IConfiguration _config;
        private readonly TestHttpMessageHandler _httpHandler;
        private readonly HttpClient _httpClient;
        private readonly BuscarRestaurantesCercanosUseCase _sut;

        public BuscarRestaurantesCercanosUseCaseTests()
        {
            _repoMock = new Mock<IRestauranteRepository>();

            var dict = new Dictionary<string, string?>
            {
                { "GooglePlaces:ApiKey", "fake-api-key" }
            };
            _config = new ConfigurationBuilder()
                .AddInMemoryCollection(dict!)
                .Build();

            _httpHandler = new TestHttpMessageHandler();
            _httpClient = new HttpClient(_httpHandler);

            _sut = new BuscarRestaurantesCercanosUseCase(
                _repoMock.Object,
                _config,
                _httpClient);
        }

        private static Restaurante CreateRestaurante(Guid? id = null, double rating = 0, int userRatings = 0)
        {
            return new Restaurante
            {
                Id = id ?? Guid.NewGuid(),
                Nombre = "Restaurante",
                Direccion = "Calle falsa 123",
                Latitud = 0,
                Longitud = 0,
                Rating = rating,
                CantidadResenas = userRatings,
                Categoria = "restaurant",
                ImagenUrl = null,
                UltimaActualizacion = DateTime.UtcNow
            };
        }

        private class TestHttpMessageHandler : HttpMessageHandler
        {
            public int SendCount { get; private set; }
            public Func<HttpRequestMessage, CancellationToken, HttpResponseMessage>? OnSend { get; set; }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                SendCount++;
                if (OnSend != null)
                {
                    return Task.FromResult(OnSend(request, cancellationToken));
                }

                var resp = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{}", Encoding.UTF8, "application/json")
                };
                return Task.FromResult(resp);
            }
        }

        [Fact]
        public async Task HandleAsync_ConCacheYSinServesCsv_UsaSoloCacheNoLlamaGoogle()
        {
            var lat = -34.6;
            var lng = -58.4;
            var radio = 1000;
            var ct = CancellationToken.None;

            var r1 = CreateRestaurante(rating: 4.5, userRatings: 50);
            var r2 = CreateRestaurante(rating: 3.0, userRatings: 5);
            var cachedList = new List<Restaurante> { r1, r2 };

            _repoMock
                .Setup(r => r.GetNearbyAsync(lat, lng, radio, TimeSpan.FromHours(24), ct))
                .ReturnsAsync(cachedList);

            var result = await _sut.HandleAsync(
                lat,
                lng,
                radio,
                typesCsv: null,
                priceLevelsCsv: null,
                openNow: null,
                minRating: 4.0,
                minUserRatings: 10,
                servesCsv: null,
                ct: ct);

        
            Assert.Single(result);
            Assert.Equal(r1.Id, result[0].Id);

            Assert.Equal(0, _httpHandler.SendCount);

            _repoMock.Verify(r =>
                    r.GetNearbyAsync(lat, lng, radio, TimeSpan.Zero, ct),
                Times.Never);

            _repoMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

   
        [Fact]
        public async Task HandleAsync_SinCache_LlamaGoogleYLuegoRefrescaDesdeRepo()
        {
            var lat = -34.6;
            var lng = -58.4;
            var radio = 1000;
            var ct = CancellationToken.None;

            _repoMock
                .Setup(r => r.GetNearbyAsync(lat, lng, radio, TimeSpan.FromHours(24), ct))
                .ReturnsAsync(new List<Restaurante>());

            var finalList = new List<Restaurante>
            {
                CreateRestaurante(),
                CreateRestaurante()
            };

            _repoMock
                .Setup(r => r.GetNearbyAsync(lat, lng, radio, TimeSpan.Zero, ct))
                .ReturnsAsync(finalList);

            _repoMock
                .Setup(r => r.SaveChangesAsync(ct))
                .Returns(Task.CompletedTask);

            _httpHandler.OnSend = (request, token) =>
            {
                Assert.Equal("https://places.googleapis.com/v1/places:searchNearby", request.RequestUri!.ToString());
                Assert.Equal(HttpMethod.Post, request.Method);
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{}", Encoding.UTF8, "application/json")
                };
            };

            // Act
            var result = await _sut.HandleAsync(
                lat,
                lng,
                radio,
                typesCsv: null,
                priceLevelsCsv: null,
                openNow: null,
                minRating: null,
                minUserRatings: 0,
                servesCsv: null, 
                ct: ct);

            Assert.Equal(2, result.Count);

            Assert.Equal(1, _httpHandler.SendCount);

            _repoMock.Verify(r =>
                    r.GetNearbyAsync(lat, lng, radio, TimeSpan.FromHours(24), ct),
                Times.Once);

            _repoMock.Verify(r =>
                    r.GetNearbyAsync(lat, lng, radio, TimeSpan.Zero, ct),
                Times.Once);

            _repoMock.Verify(r => r.SaveChangesAsync(ct), Times.Once);
        }
    }
}
