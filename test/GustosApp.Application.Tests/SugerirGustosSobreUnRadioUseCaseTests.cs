
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
        // 3. Restaurante coincide perfectamente → aparece en resultado
        // ------------------------------------------------------------
        [Fact]
        public async Task Handle_RestauranteCompatible_Aparece()
        {
            var usuario = UsuarioPizza();
            var rest = RestaurantePizza();

            _mockEmbedding.Setup(m => m.GetEmbedding("pizza"))
                .Returns(Emb(1, 0));

            _mockEmbedding.Setup(m => m.GetEmbedding("pizza"))
                .Returns(Emb(1, 0));

            var result = await _sut.Handle(usuario, new List<Restaurante> { rest });

            Assert.Single(result);
            Assert.Equal(rest.Id, result[0].Id);
            Assert.True(rest.Score > 0.5);
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

        // ------------------------------------------------------------
        // 6. Ordenado por score
        // ------------------------------------------------------------
        [Fact]
        public async Task Handle_OrdenadoPorScore()
        {
            var usuario = UsuarioPizza();

            var r1 = RestaurantePizza(id: new Guid());
            var r2 = RestaurantePizza(id: new Guid());

            _mockEmbedding.Setup(m => m.GetEmbedding("pizza"))
                .Returns(Emb(1, 0));

            _mockEmbedding.SetupSequence(m => m.GetEmbedding("pizza"))
                .Returns(Emb(1, 0))   // user
                .Returns(Emb(1, 0));  // rest1

            _mockEmbedding.Setup(m => m.GetEmbedding("pizza pizza"))
                .Returns(Emb(0.1f, 0.1f)); // rest2 (similaridad baja)

            var result = await _sut.Handle(usuario, new List<Restaurante> { r2, r1 });

            Assert.Equal(new Guid(), result[0].Id);
        }

        // ------------------------------------------------------------
        // 7. Respeta maxResults
        // ------------------------------------------------------------
        [Fact]
        public async Task Handle_MaxResultsRespetado()
        {
            var usuario = UsuarioPizza();

            var restaurants = Enumerable.Range(1, 20)
                .Select(id => RestaurantePizza(Guid.NewGuid()))
                .ToList();

            _mockEmbedding.Setup(m => m.GetEmbedding(It.IsAny<string>()))
                .Returns(Emb(1, 0));

            var result = await _sut.Handle(usuario, restaurants, maxResults: 5);

            Assert.Equal(5, result.Count);
        }
    }

}