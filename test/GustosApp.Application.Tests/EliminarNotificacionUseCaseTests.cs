using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using GustosApp.Application.UseCases.NotificacionUseCases;
using GustosApp.Domain.Interfaces;
using Moq;

namespace GustosApp.Application.Tests
{
    public class EliminarNotificacionUseCaseTests
    {
        private readonly Mock<INotificacionRepository> _repo;
        private readonly EliminarNotificacionUseCase _useCase;

        public EliminarNotificacionUseCaseTests()
        {
            _repo = new Mock<INotificacionRepository>();
            _useCase = new EliminarNotificacionUseCase(_repo.Object);
        }

       
        [Fact]
        public async Task HandleAsync_IdVacio_DeberiaLanzarArgumentException()
        {
            // Arrange
            var id = Guid.Empty;

            // Act
            var act = async () => await _useCase.HandleAsync(id, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<ArgumentException>()
                .WithMessage("*ID de notificación no puede ser vacío*");
        }

       
        [Fact]
        public async Task HandleAsync_Valido_DeberiaEliminarNotificacion()
        {
            // Arrange
            var id = Guid.NewGuid();

            _repo.Setup(r => r.EliminarAsync(id, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _useCase.HandleAsync(id, CancellationToken.None);

            // Assert
            _repo.Verify(r => r.EliminarAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        }

    
        [Fact]
        public async Task HandleAsync_ErrorRepositorio_DeberiaPropagarExcepcion()
        {
            // Arrange
            var id = Guid.NewGuid();

            _repo.Setup(r => r.EliminarAsync(id, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("DB ERROR"));

            // Act
            var act = async () => await _useCase.HandleAsync(id, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("DB ERROR");

        }

    }
    }
