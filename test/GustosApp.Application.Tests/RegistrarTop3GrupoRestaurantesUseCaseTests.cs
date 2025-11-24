using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GustosApp.Application.UseCases.RestauranteUseCases;
using GustosApp.Domain.Interfaces;
using Moq;
using Xunit;
using System.Linq;

namespace GustosApp.Application.Tests
{
    public class RegistrarTop3GrupoRestaurantesUseCaseTests
    {
        private readonly Mock<IRestauranteEstadisticasRepository> _estadisticasRepositoryMock;
        private readonly RegistrarTop3GrupoRestaurantesUseCase _sut;

        public RegistrarTop3GrupoRestaurantesUseCaseTests()
        {
            _estadisticasRepositoryMock = new Mock<IRestauranteEstadisticasRepository>();
            _sut = new RegistrarTop3GrupoRestaurantesUseCase(_estadisticasRepositoryMock.Object);
        }

        [Fact]
        public async Task HandleAsync_RestauranteIdsNull_LanzaArgumentNullException()
        {
            IEnumerable<Guid>? ids = null;

            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _sut.HandleAsync(ids!, CancellationToken.None));
        }

        [Fact]
        public async Task HandleAsync_IdsValidos_LlamaIncrementarTop3GrupoAsyncConLosMismosIds()
        {
            var ct = new CancellationTokenSource().Token;

            var ids = new List<Guid>
            {
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid()
            };

            _estadisticasRepositoryMock
                .Setup(r => r.IncrementarTop3GrupoAsync(ids, ct))
                .Returns(Task.CompletedTask)
                .Verifiable();

            await _sut.HandleAsync(ids, ct);

            _estadisticasRepositoryMock.Verify(
    r => r.IncrementarTop3GrupoAsync(
        It.Is<IEnumerable<Guid>>(p =>
            p != null &&
            p.Count() == ids.Count &&
            !p.Except(ids).Any()),
        ct),
    Times.Once);
        }
    }
}
