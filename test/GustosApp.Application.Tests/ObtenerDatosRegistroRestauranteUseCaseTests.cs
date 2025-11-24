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
    public class ObtenerDatosRegistroRestauranteUseCaseTests
    {
        private readonly Mock<IGustoRepository> _gustoRepositoryMock;
        private readonly Mock<IRestriccionRepository> _restriccionRepositoryMock;
        private readonly ObtenerDatosRegistroRestauranteUseCase _sut;

        public ObtenerDatosRegistroRestauranteUseCaseTests()
        {
            _gustoRepositoryMock = new Mock<IGustoRepository>();
            _restriccionRepositoryMock = new Mock<IRestriccionRepository>();

            _sut = new ObtenerDatosRegistroRestauranteUseCase(
                _gustoRepositoryMock.Object,
                _restriccionRepositoryMock.Object);
        }

        [Fact]
        public async Task HandleAsync_DevuelveGustosYRestriccionesDeLosRepositorios()
        {
            var ct = CancellationToken.None;

            var gustosEsperados = new List<Gusto>
            {
                new Gusto { Id = Guid.NewGuid(), Nombre = "Pizza" },
                new Gusto { Id = Guid.NewGuid(), Nombre = "Hamburguesa" }
            };

            var restriccionesEsperadas = new List<Restriccion>
            {
                new Restriccion { Id = Guid.NewGuid(), Nombre = "Celiaquía" },
                new Restriccion { Id = Guid.NewGuid(), Nombre = "Vegano" }
            };

            _gustoRepositoryMock
                .Setup(g => g.GetAllAsync(ct))
                .ReturnsAsync(gustosEsperados);

            _restriccionRepositoryMock
                .Setup(r => r.GetAllAsync(ct))
                .ReturnsAsync(restriccionesEsperadas);

            var (gustos, restricciones) = await _sut.HandleAsync(ct);

            Assert.Same(gustosEsperados, gustos);
            Assert.Same(restriccionesEsperadas, restricciones);

            _gustoRepositoryMock.Verify(g => g.GetAllAsync(ct), Times.Once);
            _restriccionRepositoryMock.Verify(r => r.GetAllAsync(ct), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_SoportaListasVacias()
        {
            var ct = CancellationToken.None;

            _gustoRepositoryMock
                .Setup(g => g.GetAllAsync(ct))
                .ReturnsAsync(new List<Gusto>());

            _restriccionRepositoryMock
                .Setup(r => r.GetAllAsync(ct))
                .ReturnsAsync(new List<Restriccion>());

            var (gustos, restricciones) = await _sut.HandleAsync(ct);

            Assert.NotNull(gustos);
            Assert.NotNull(restricciones);
            Assert.Empty(gustos);
            Assert.Empty(restricciones);

            _gustoRepositoryMock.Verify(g => g.GetAllAsync(ct), Times.Once);
            _restriccionRepositoryMock.Verify(r => r.GetAllAsync(ct), Times.Once);
        }
    }
}
