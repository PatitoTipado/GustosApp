using System;
using System.Threading;
using System.Threading.Tasks;
using GustosApp.Application.UseCases.RestauranteUseCases;
using GustosApp.Domain.Interfaces;
using Moq;
using Xunit;

namespace GustosApp.Application.Tests
{
    public class RegistrarVisitaPerfilRestauranteUseCaseTests
    {
        private readonly Mock<IRestauranteEstadisticasRepository> _estadisticasRepositoryMock;
        private readonly RegistrarVisitaPerfilRestauranteUseCase _sut;

        public RegistrarVisitaPerfilRestauranteUseCaseTests()
        {
            _estadisticasRepositoryMock = new Mock<IRestauranteEstadisticasRepository>();
            _sut = new RegistrarVisitaPerfilRestauranteUseCase(_estadisticasRepositoryMock.Object);
        }

        [Fact]
        public async Task HandleAsync_RestauranteIdVacio_LanzaArgumentException()
        {
            var restauranteId = Guid.Empty;

            var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
                _sut.HandleAsync(restauranteId, CancellationToken.None));

           
            Assert.StartsWith("El restauranteId no puede ser vacío.", ex.Message);
            Assert.Equal("restauranteId", ex.ParamName);

        }

        [Fact]
        public async Task HandleAsync_RestauranteIdValido_LlamaIncrementarVisitaPerfilAsync()
        {
            var restauranteId = Guid.NewGuid();
            var ct = new CancellationTokenSource().Token;

            _estadisticasRepositoryMock
                .Setup(r => r.IncrementarVisitaPerfilAsync(restauranteId, ct))
                .Returns(Task.CompletedTask)
                .Verifiable();

            await _sut.HandleAsync(restauranteId, ct);

            _estadisticasRepositoryMock.Verify(
                r => r.IncrementarVisitaPerfilAsync(restauranteId, ct),
                Times.Once);
        }
    }
}
