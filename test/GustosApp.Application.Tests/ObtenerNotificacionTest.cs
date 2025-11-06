using GustosApp.Application.Interfaces;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GustosApp.Application.Tests.mocks
{
    public class ObtenerNotificacionTest
    {
        [Fact]
        public async Task ObtenerNotificacion()
        {
            var repoMock = new Mock<INotificacionRepository>();
            repoMock
            .Setup(r => r.ObtenerNotificacionPorUsuarioAsync(It.IsAny<Guid>(),It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Notificacion> { new Notificacion() });

            var useCase = new ObtenerNotificacionesUsuarioUseCase(repoMock.Object);
            var result = await useCase.HandleAsync(Guid.NewGuid(), default);

            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task ObtenerNotificacion_SinResultados()
        {
            var repoMock = new Mock<INotificacionRepository>();
            repoMock
                .Setup(r => r.ObtenerNotificacionPorUsuarioAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Notificacion>()); // sin resultados

            var useCase = new ObtenerNotificacionesUsuarioUseCase(repoMock.Object);

            var result = await useCase.HandleAsync(Guid.NewGuid(), default);

            Assert.NotNull(result);
            Assert.Empty(result);
        }

    }
}
