
namespace GustosApp.Application.Tests
{
    using GustosApp.Application.Interfaces;
    using GustosApp.Application.UseCases.RestauranteUseCases;
    using GustosApp.Domain.Common;
    using GustosApp.Domain.Interfaces;
    using GustosApp.Domain.Model;
    using GustosApp.Infraestructure.Repositories;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Xunit;

    public class SugerirGustosSobreUnRadioUseCaseTests
    {
        private readonly Mock<IEmbeddingService> _mockEmbedding;
        private readonly Mock<ILogger<SugerirGustosSobreUnRadioUseCase>> _mockLogger;
        private readonly SugerirGustosSobreUnRadioUseCase _sut;
        private readonly Mock<IRestauranteRepository> _mockRepository;

        public SugerirGustosSobreUnRadioUseCaseTests()
        {
            _mockEmbedding = new Mock<IEmbeddingService>();
            _mockLogger = new Mock<ILogger<SugerirGustosSobreUnRadioUseCase>>();
            _mockRepository = new Mock<IRestauranteRepository>();

            _sut = new SugerirGustosSobreUnRadioUseCase(
                _mockEmbedding.Object,
                _mockLogger.Object,
                _mockRepository.Object
            );
        }

        private float[] Emb(params float[] v) => v;

        // ------------------------------------------------------------
        //  FIXTURES
        // ------------------------------------------------------------
        private UsuarioPreferencias UsuarioPizza()
            => new UsuarioPreferencias
            {
                Gustos = new List<string> { "pizza" },
                Restricciones = new List<string>()
            };

        private Restaurante RestaurantePizza(Guid id = new Guid())
            => new Restaurante
            {
                Id = id,
                Nombre = "Pizza Place",
                GustosQueSirve = new List<Gusto> { new Gusto { Nombre = "pizza" } },
                RestriccionesQueRespeta = new List<Restriccion>()
            };

        // ------------------------------------------------------------
        // 1. Usuario sin gustos → retorna vacío
        // ------------------------------------------------------------
        [Fact]
        public async Task Handle_SinGustos_ReturnsEmpty()
        {
            var usuario = new UsuarioPreferencias { Gustos = new List<string>() };
            var result = await _sut.Handle(usuario, new List<Restaurante>());
            Assert.Empty(result);
        }

        // ------------------------------------------------------------
        // 2. Sin restaurantes → vacío
        // ------------------------------------------------------------
        [Fact]
        public async Task Handle_SinRestaurantes_ReturnsEmpty()
        {
            var usuario = UsuarioPizza();
            var result = await _sut.Handle(usuario, new List<Restaurante>());
            Assert.Empty(result);
        }


        // ------------------------------------------------------------
        // 4. Penalización por gustos faltantes → queda fuera
        // ------------------------------------------------------------
        [Fact]
        public async Task Handle_GustosFaltantes_Excluye()
        {
            var usuario = new UsuarioPreferencias
            {
                Gustos = new List<string> { "pizza", "sushi" },
                Restricciones = new List<string>()
            };

            var rest = new Restaurante
            {
                Id = new Guid(),
                GustosQueSirve = new List<Gusto> { new Gusto { Nombre = "pizza" } },
                RestriccionesQueRespeta = new List<Restriccion>()
            };

            _mockEmbedding.Setup(m => m.GetEmbedding(It.IsAny<string>()))
                .Returns(Emb(0, 0)); // score bajísimo

            var result = await _sut.Handle(usuario, new List<Restaurante> { rest });

            Assert.Empty(result); // La penalización empuja el score por debajo del umbral
        }

        // ------------------------------------------------------------
        // 5. Restaurante sin texto → ignorado
        // ------------------------------------------------------------
        [Fact]
        public async Task Handle_RestSinDescripcion_SeIgnora()
        {
            var usuario = UsuarioPizza();

            var rest = new Restaurante
            {
                Id = new Guid(),
                GustosQueSirve = new List<Gusto>(),
                RestriccionesQueRespeta = new List<Restriccion>()
            };

            var result = await _sut.Handle(usuario, new List<Restaurante> { rest });

            Assert.Empty(result);
        }

    }

}