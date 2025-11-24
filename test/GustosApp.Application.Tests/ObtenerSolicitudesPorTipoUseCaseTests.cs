using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GustosApp.Application.UseCases.RestauranteUseCases.SolicitudRestauranteUseCases;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model.@enum;
using GustosApp.Domain.Model;
using Moq;
using FluentAssertions;

namespace GustosApp.Application.Tests
{
    public class ObtenerSolicitudesPorTipoUseCaseTests
    {
        private readonly Mock<ISolicitudRestauranteRepository> _repo;
        private readonly ObtenerSolicitudesPorTipoUseCase _useCase;

        public ObtenerSolicitudesPorTipoUseCaseTests()
        {
            _repo = new Mock<ISolicitudRestauranteRepository>();
            _useCase = new ObtenerSolicitudesPorTipoUseCase(_repo.Object);
        }

        // Helper
        private SolicitudRestaurante FakeSolicitud(Guid id)
            => new SolicitudRestaurante
            {
                Id = id,
                FechaCreacion = DateTime.UtcNow
            };

      

        [Fact]
        public async Task HandleAsync_DeberiaRetornarPendientes_CuandoFiltroEsPendiente()
        {
            // Arrange
            var lista = new List<SolicitudRestaurante>
        {
            FakeSolicitud(Guid.NewGuid()),
            FakeSolicitud(Guid.NewGuid())
        };

            _repo.Setup(r => r.GetByEstadoAsync(EstadoSolicitudRestaurante.Pendiente, It.IsAny<CancellationToken>()))
                 .ReturnsAsync(lista);

            // Act
            var result = await _useCase.HandleAsync(EstadoSolicitudRestaurante.Pendiente, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(lista);

            _repo.Verify(r => r.GetByEstadoAsync(EstadoSolicitudRestaurante.Pendiente, It.IsAny<CancellationToken>()), Times.Once);
            _repo.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task HandleAsync_DeberiaRetornarAprobadas_CuandoFiltroEsAprobada()
        {
            var lista = new List<SolicitudRestaurante> { FakeSolicitud(Guid.NewGuid()) };

            _repo.Setup(r => r.GetByEstadoAsync(EstadoSolicitudRestaurante.Aprobada, It.IsAny<CancellationToken>()))
                 .ReturnsAsync(lista);

            var result = await _useCase.HandleAsync(EstadoSolicitudRestaurante.Aprobada, default);

            result.Should().BeEquivalentTo(lista);

            _repo.Verify(r => r.GetByEstadoAsync(EstadoSolicitudRestaurante.Aprobada, It.IsAny<CancellationToken>()), Times.Once);
            _repo.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task HandleAsync_DeberiaRetornarRechazadas_CuandoFiltroEsRechazada()
        {
            var lista = new List<SolicitudRestaurante> { FakeSolicitud(Guid.NewGuid()) };

            _repo.Setup(r => r.GetByEstadoAsync(EstadoSolicitudRestaurante.Rechazada, It.IsAny<CancellationToken>()))
                 .ReturnsAsync(lista);

            var result = await _useCase.HandleAsync(EstadoSolicitudRestaurante.Rechazada, default);

            result.Should().Equal(lista);

            _repo.Verify(r => r.GetByEstadoAsync(EstadoSolicitudRestaurante.Rechazada, It.IsAny<CancellationToken>()), Times.Once);
            _repo.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task HandleAsync_DeberiaRetornarTodas_CuandoFiltroEsTodas()
        {
            var lista = new List<SolicitudRestaurante> { FakeSolicitud(Guid.NewGuid()) };

            _repo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                 .ReturnsAsync(lista);

            var result = await _useCase.HandleAsync(EstadoSolicitudRestaurante.Todas, default);

            result.Should().BeEquivalentTo(lista);

            _repo.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);

            _repo.Verify(r => r.GetByEstadoAsync(It.IsAny<EstadoSolicitudRestaurante>(), It.IsAny<CancellationToken>()),
                         Times.Never);
        }

        [Fact]
        public async Task HandleAsync_DeberiaUsarGetAllAsync_CuandoFiltroEsDesconocido()
        {
            // Arrange
            var lista = new List<SolicitudRestaurante> { FakeSolicitud(Guid.NewGuid()) };

            _repo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                 .ReturnsAsync(lista);

            // Simulamos un valor que no esté en el enum (e.g. 999)
            var filtroInvalido = (EstadoSolicitudRestaurante)999;

            // Act
            var result = await _useCase.HandleAsync(filtroInvalido, default);

            // Assert
            result.Should().BeEquivalentTo(lista);

            _repo.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);

            _repo.Verify(r => r.GetByEstadoAsync(It.IsAny<EstadoSolicitudRestaurante>(), It.IsAny<CancellationToken>()),
                         Times.Never);
        }
    }
    }
