using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using GustosApp.Application.UseCases.NotificacionUseCases;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using Moq;

namespace GustosApp.Application.Tests
{
    public class ObtenerNotificacionUsuarioUseCaseTests
    {
        private readonly Mock<INotificacionRepository> _repo;
        private readonly ObtenerNotificacionUsuarioUseCase _useCase;

        public ObtenerNotificacionUsuarioUseCaseTests()
        {
            _repo = new Mock<INotificacionRepository>();
            _useCase = new ObtenerNotificacionUsuarioUseCase(_repo.Object);
        }

        [Fact]
        public async Task HandleAsync_DeberiaRetornarNotificacion()
        {
            var id = Guid.NewGuid();
            var notif = new Notificacion { Id = id, Mensaje = "Hola" };

            _repo.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(notif);

            var result = await _useCase.HandleAsync(id, CancellationToken.None);

            result.Should().NotBeNull();
            result!.Id.Should().Be(id);
            result.Mensaje.Should().Be("Hola");
        }

        [Fact]
        public async Task HandleAsync_SiNoExiste_DeberiaRetornarNull()
        {
            var id = Guid.NewGuid();

            _repo.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Notificacion?)null);

            var result = await _useCase.HandleAsync(id, CancellationToken.None);

            result.Should().BeNull();
        }

        [Fact]
        public async Task HandleAsync_DeberiaLlamarRepositorioUnaVez()
        {
            var id = Guid.NewGuid();
            _repo.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Notificacion?)null);

            await _useCase.HandleAsync(id, CancellationToken.None);

            _repo.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}