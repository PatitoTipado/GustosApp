
namespace GustosApp.Application.Tests
{
    using Xunit;
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using GustosApp.Application.Interfaces;
    using GustosApp.Domain.Interfaces;
    using GustosApp.Application.UseCases.RestauranteUseCases;
    using GustosApp.Domain.Common;
    using GustosApp.Domain.Model;

    public class SugerirGustosSobreUnRadioUseCaseTests
    {
        private readonly Mock<IEmbeddingService> _embeddingMock;
        private readonly Mock<IRestauranteRepository> _repoMock;

        private readonly SugerirGustosSobreUnRadioUseCase _sut;

        public SugerirGustosSobreUnRadioUseCaseTests()
        {
            _embeddingMock = new Mock<IEmbeddingService>();
            _repoMock = new Mock<IRestauranteRepository>();

            _sut = new SugerirGustosSobreUnRadioUseCase(
                _embeddingMock.Object,
                _repoMock.Object
            );
        }

        private float[] FakeEmbedding(string text) =>
            Enumerable.Repeat(1f, 5).ToArray();

        // ----------------------------------------
        // 1. Validaciones de entrada
        // ----------------------------------------

        [Fact]
        public async Task Handle_DebeLanzarExcepcion_SiUsuarioNoTieneGustos()
        {
            var usuario = new UsuarioPreferencias { Gustos = new List<string>() };
            var restaurantes = new List<Restaurante>();

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _sut.Handle(usuario, restaurantes)
            );
        }

        [Fact]
        public async Task Handle_DebeLanzarExcepcion_SiNoHayRestaurantes()
        {
            var usuario = new UsuarioPreferencias { Gustos = new List<string> { "pizza" } };
            var restaurantes = new List<Restaurante>();

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _sut.Handle(usuario, restaurantes)
            );
        }

        // ----------------------------------------
        // 2. Flujo principal sin resenas
        // ----------------------------------------

        [Fact]
        public async Task Handle_DebeRetornarResultados_SinResenas()
        {
            // Arrange
            var usuario = new UsuarioPreferencias
            {
                Gustos = new List<string> { "pizza" }
            };

            var rest = new Restaurante
            {
                Id = Guid.NewGuid(),
                GustosQueSirve = new List<Gusto> { new Gusto { Nombre = "pizza" } },
                RestriccionesQueRespeta = new List<Restriccion>()
            };

            var restaurantes = new List<Restaurante> { rest };

            _embeddingMock
                .Setup(x => x.GetEmbedding(It.IsAny<string>()))
                .Returns(FakeEmbedding("x"));

            _repoMock
                .Setup(x => x.obtenerRestauranteConResenias(It.IsAny<List<Guid>>()))
                .ReturnsAsync(restaurantes);

            // Act
            var result = await _sut.Handle(usuario, restaurantes);

            // Assert
            Assert.Single(result);
            Assert.Equal(rest.Id, result.First().Id);
        }

        // ----------------------------------------
        // 3. Flujo con resenas positivas y negativas
        // ----------------------------------------

        [Fact]
        public async Task Handle_AplicaAjustePorResenas()
        {
            // Arrange usuario
            var usuario = new UsuarioPreferencias
            {
                Gustos = new List<string> { "pasta" }
            };

            float[] embedding = new float[] { 1, 1, 1, 1, 1 };

            _embeddingMock
                .Setup(x => x.GetEmbedding(It.IsAny<string>()))
                .Returns(embedding);

            var rest = new Restaurante
            {
                Id = Guid.NewGuid(),
                GustosQueSirve = new List<Gusto> { new Gusto { Nombre = "pasta" } },
                RestriccionesQueRespeta = new List<Restriccion>(),
                Reviews = new List<OpinionRestaurante>
            {
                new OpinionRestaurante { Opinion = "me encanto", Valoracion = 5 },
                new OpinionRestaurante { Opinion = "muy malo", Valoracion = 1 }
            }
            };

            var restaurantes = new List<Restaurante> { rest };

            _repoMock
                .Setup(x => x.obtenerRestauranteConResenias(It.IsAny<List<Guid>>()))
                .ReturnsAsync(restaurantes);

            // Act
            var result = await _sut.Handle(usuario, restaurantes);

            // Assert
            Assert.Single(result);
            Assert.True(result.First().Score > 0); // ajustado
        }

        // ----------------------------------------
        // 4. Si ninguna resena es relevante, no ajusta nada
        // ----------------------------------------

        [Fact]
        public async Task Handle_NoAjustaPuntuacion_SiResenasNoRelevantes()
        {
            var usuario = new UsuarioPreferencias
            {
                Gustos = new List<string> { "asado" }
            };

            float[] embedding = new float[] { 1, 1, 1, 1, 1 };

            _embeddingMock
                .Setup(x => x.GetEmbedding(It.IsAny<string>()))
                .Returns(embedding);

            var rest = new Restaurante
            {
                Id = Guid.NewGuid(),
                GustosQueSirve = new List<Gusto> { new Gusto { Nombre = "asado" } },
                RestriccionesQueRespeta = new List<Restriccion>(),
                Reviews = new List<OpinionRestaurante>
            {
                new OpinionRestaurante { Opinion = "", Valoracion = 5 },
                new OpinionRestaurante { Opinion = "irrelevante", Valoracion = 2 }
            }
            };

            var restaurantes = new List<Restaurante> { rest };

            _repoMock
                .Setup(x => x.obtenerRestauranteConResenias(It.IsAny<List<Guid>>()))
                .ReturnsAsync(restaurantes);

            var result = await _sut.Handle(usuario, restaurantes);

            Assert.Single(result);
        }

        // ----------------------------------------
        // 5. Test de ordenamiento
        // ----------------------------------------

        [Fact]
        public async Task Handle_DevuelveOrdenadoPorMejorScore()
        {
            var usuario = new UsuarioPreferencias { Gustos = new List<string> { "comida" } };

            float[] embedding = FakeEmbedding("x");

            _embeddingMock.Setup(x => x.GetEmbedding(It.IsAny<string>()))
                .Returns(embedding);

            var r1 = new Restaurante
            {
                Id = Guid.NewGuid(),
                GustosQueSirve = new List<Gusto> { new Gusto { Nombre = "comida" } },
                RestriccionesQueRespeta = new List<Restriccion>()
            };

            var r2 = new Restaurante
            {
                Id = Guid.NewGuid(),
                GustosQueSirve = new List<Gusto> { new Gusto { Nombre = "comida" } },
                RestriccionesQueRespeta = new List<Restriccion>()
            };

            var restaurantes = new List<Restaurante> { r1, r2 };

            _repoMock
                .Setup(x => x.obtenerRestauranteConResenias(It.IsAny<List<Guid>>()))
                .ReturnsAsync(restaurantes);

            // Act
            var result = await _sut.Handle(usuario, restaurantes);

            // Assert
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task Handle_IgnoraRestaurantes_SiEmbeddingRestauranteEsNull()
        {
            var usuario = new UsuarioPreferencias
            {
                Gustos = new List<string> { "pasta" }
            };

            // Embedding usuario OK
            _embeddingMock.Setup(x => x.GetEmbedding(It.Is<string>(t => t.Contains("pasta"))))
                .Returns(new float[] { 1, 1, 1 });

            // Embedding del restaurante null
            _embeddingMock.Setup(x => x.GetEmbedding(It.Is<string>(t => !t.Contains("pasta"))))
                .Returns((float[])null);

            var rest = new Restaurante
            {
                Id = Guid.NewGuid(),
                GustosQueSirve = new List<Gusto>(), // texto vacío → embedding null
                RestriccionesQueRespeta = new List<Restriccion>()
            };

            var restaurantes = new List<Restaurante> { rest };

            _repoMock.Setup(x => x.obtenerRestauranteConResenias(It.IsAny<List<Guid>>()))
                .ReturnsAsync(new List<Restaurante>());

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _sut.Handle(usuario, restaurantes));
        }

    }

}