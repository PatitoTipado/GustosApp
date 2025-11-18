using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GustosApp.Application.UseCases;
using GustosApp.Application.DTO;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using Moq;
using Xunit;
using Microsoft.VisualBasic;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using GustosApp.Application.Interfaces;
using GustosApp.Application.UseCases.RestauranteUseCases;
using Microsoft.Extensions.Logging;

namespace GustosApp.Application.Tests
{

    public class SugerirGustosUseCaseTests
    {

        /*S

        [Fact]
        public async Task Handle_DeberiaDevolverRestaurantesSimilares_SegunGustos()
        {
            // Arrange
            var mockRepo = new Mock<IRestauranteRepository>();
            var mockEmb = new Mock<IEmbeddingService>();
            var logger = new Mock<ILogger<SugerirGustosUseCase>>();

            // Mock: 3 restaurantes con gustos distintos
            var rest1 = new Restaurante { Id = Guid.NewGuid(), Nombre = "Pizza Loca", GustosQueSirve = new List<Gusto> { new Gusto { Nombre = "Pizza" } } };
            var rest2 = new Restaurante { Id = Guid.NewGuid(), Nombre = "Sushi Zen", GustosQueSirve = new List<Gusto> { new Gusto { Nombre = "Sushi" } } };
            var rest3 = new Restaurante { Id = Guid.NewGuid(), Nombre = "Carnes Don Pepe", GustosQueSirve = new List<Gusto> { new Gusto { Nombre = "Carne" } } };

            mockRepo.Setup(r => r.buscarRestauranteParaUsuariosConGustosYRestricciones(
                It.IsAny<List<string>>(), It.IsAny<List<string>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Restaurante> { rest1, rest2, rest3 });

            // Mock embeddings (valores simples para controlar resultados)
            mockEmb.Setup(e => e.GetEmbedding(It.Is<string>(s => s.Contains("Pizza"))))
                .Returns(new float[] { 1, 0 });
            mockEmb.Setup(e => e.GetEmbedding(It.Is<string>(s => s.Contains("Sushi"))))
                .Returns(new float[] { 0, 1 });
            mockEmb.Setup(e => e.GetEmbedding(It.Is<string>(s => s.Contains("Carne"))))
                .Returns(new float[] { 0.5f, 0.5f });
            mockEmb.Setup(e => e.GetEmbedding(It.Is<string>(s => s.Contains("Pizza Sushi"))))
                .Returns(new float[] { 0.7f, 0.7f });

            var useCase = new SugerirGustosUseCase(mockRepo.Object, mockEmb.Object, logger.Object);

            var preferencias = new UsuarioPreferencias
            {
                Gustos = new List<string> { "Pizza", "Sushi" },
                Restricciones = new List<string>(),
                CondicionesMedicas = new List<string>()
            };

            // Act
            var resultados = await useCase.Handle(preferencias, 5, CancellationToken.None);

            // Assert
            Assert.NotEmpty(resultados);
            Assert.Contains(resultados, r => r.Nombre.Contains("Pizza"));
            Assert.True(resultados.All(r => r.Score >= 0.3)); // supera el umbral
            Assert.Equal(resultados.OrderByDescending(r => r.Score).Select(r => r.Id), resultados.Select(r => r.Id)); // está ordenado
        }

        [Fact]
        public async Task Handle_DeberiaIgnorarRestaurantesDuplicados()
        {
            // Arrange
            var mockRepo = new Mock<IRestauranteRepository>();
            var mockEmb = new Mock<IEmbeddingService>();
            var logger = new Mock<ILogger<SugerirGustosUseCase>>();

            var rest = new Restaurante
            {
                Id = Guid.NewGuid(),
                Nombre = "Pizza Planet",
                GustosQueSirve = new List<Gusto> { new Gusto { Nombre = "Pizza" }, new Gusto { Nombre = "Muzzarella" } }
            };

            mockRepo.Setup(r => r.buscarRestauranteParaUsuariosConGustosYRestricciones(
                It.IsAny<List<string>>(), It.IsAny<List<string>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Restaurante> { rest, rest }); // duplicado intencional

            mockEmb.Setup(e => e.GetEmbedding(It.IsAny<string>())).Returns(new float[] { 1, 0 });

            var useCase = new SugerirGustosUseCase(mockRepo.Object, mockEmb.Object, logger.Object);

            var preferencias = new UsuarioPreferencias
            {
                Gustos = new List<string> { "Pizza" },
                Restricciones = new List<string>(),
                CondicionesMedicas = new List<string>()
            };

            // Act
            var resultados = await useCase.Handle(preferencias, 10, CancellationToken.None);

            // Assert
            Assert.Single(resultados); // ✅ sólo uno
            Assert.Equal("Pizza Planet", resultados.First().Nombre);
        }

        [Fact]
        public async Task Handle_DeberiaManejarErroresDeEmbeddingSinFallar()
        {
            // Arrange
            var mockRepo = new Mock<IRestauranteRepository>();
            var mockEmb = new Mock<IEmbeddingService>();
            var logger = new Mock<ILogger<SugerirGustosUseCase>>();

            mockRepo.Setup(r => r.buscarRestauranteParaUsuariosConGustosYRestricciones(
                It.IsAny<List<string>>(), It.IsAny<List<string>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Restaurante>
                {
                new Restaurante { Id = Guid.NewGuid(), Nombre = "Restaurante X", GustosQueSirve = new List<Gusto> { new Gusto { Nombre = "Pizza" } } }
                });

            // Forzamos una excepción al generar embeddings
            mockEmb.Setup(e => e.GetEmbedding(It.IsAny<string>())).Throws(new Exception("Error de IA"));

            var useCase = new SugerirGustosUseCase(mockRepo.Object, mockEmb.Object, logger.Object);

            var preferencias = new UsuarioPreferencias
            {
                Gustos = new List<string> { "Pizza" },
                Restricciones = new List<string>(),
                CondicionesMedicas = new List<string>()
            };

            // Act
            var resultados = await useCase.Handle(preferencias, 5, CancellationToken.None);

            // Assert
            Assert.NotNull(resultados);
            Assert.Empty(resultados); // sin resultados válidos, pero no lanza excepción
        }
    }
    */
    }
}