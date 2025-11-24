using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using GustosApp.Application.UseCases.RestauranteUseCases.SolicitudRestauranteUseCases;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using Moq;

namespace GustosApp.Application.Tests
{
    public class ObtenerSolicitudRestaurantesPorIdUseCaseTests
    {
        private readonly Mock<ISolicitudRestauranteRepository> _repo;
        private readonly ObtenerSolicitudRestaurantesPorIdUseCase _useCase;

        public ObtenerSolicitudRestaurantesPorIdUseCaseTests()
        {
            _repo = new Mock<ISolicitudRestauranteRepository>();
            _useCase = new ObtenerSolicitudRestaurantesPorIdUseCase(_repo.Object);
        }

        // Helper simple
        private SolicitudRestaurante FakeSolicitud(Guid id)
            => new SolicitudRestaurante
            {
                Id = id,
                FechaCreacion = DateTime.UtcNow,
                Nombre = "Test",
                Direccion = "Calle"
            };

      

        [Fact]
        public async Task HandleAsync_DeberiaRetornarSolicitud_SiExiste()
        {
            // Arrange
            var id = Guid.NewGuid();
            var solicitud = FakeSolicitud(id);

            _repo.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                 .ReturnsAsync(solicitud);

            // Act
            var result = await _useCase.HandleAsync(id, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeSameAs(solicitud);

            _repo.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        }

      
        [Fact]
        public async Task HandleAsync_DeberiaLanzarKeyNotFound_SiNoExiste()
        {
            // Arrange
            var id = Guid.NewGuid();

            _repo.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                 .ReturnsAsync((SolicitudRestaurante?)null);

            // Act
            Func<Task> act = () => _useCase.HandleAsync(id, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage("La solicitud de restaurante no existe.");

            _repo.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        }
    }

}