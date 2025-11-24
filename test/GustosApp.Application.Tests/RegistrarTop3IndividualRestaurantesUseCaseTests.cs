using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GustosApp.Application.UseCases.RestauranteUseCases;
using GustosApp.Domain.Interfaces;
using Moq;
using Xunit;

namespace GustosApp.Application.Tests
{
    public class RegistrarTop3IndividualRestaurantesUseCaseTests
    {
        private readonly Mock<IRestauranteEstadisticasRepository> _estadisticasRepositoryMock;
        private readonly RegistrarTop3IndividualRestaurantesUseCase _sut;

        public RegistrarTop3IndividualRestaurantesUseCaseTests()
        {
            _estadisticasRepositoryMock = new Mock<IRestauranteEstadisticasRepository>();
            _sut = new RegistrarTop3IndividualRestaurantesUseCase(_estadisticasRepositoryMock.Object);
        }

        [Fact]
        public async Task HandleAsync_RestauranteIdsNull_LanzaArgumentNullException()
        {
            IEnumerable<Guid>? ids = null;

            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _sut.HandleAsync(ids!, CancellationToken.None));
        }

        [Fact]
        public async Task HandleAsync_IdsValidos_LlamaIncrementarTop3IndividualAsyncConLosMismosIds()
        {
            var ct = new CancellationTokenSource().Token;

            var ids = new List<Guid>
            {
                Guid.NewGuid(),
                Guid.NewGuid()
            };

            _estadisticasRepositoryMock
                .Setup(r => r.IncrementarTop3IndividualAsync(It.IsAny<IEnumerable<Guid>>(), ct))
                .Returns(Task.CompletedTask);

            await _sut.HandleAsync(ids, ct);

            _estadisticasRepositoryMock.Verify(
                r => r.IncrementarTop3IndividualAsync(
                    It.Is<IEnumerable<Guid>>(p =>
                        p != null && new HashSet<Guid>(p).SetEquals(ids)),
                    ct),
                Times.Once);
        }
    }
}
