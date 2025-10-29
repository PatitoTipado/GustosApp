using GustosApp.Application.Interfaces;
using GustosApp.Application.UseCases;
using GustosApp.Domain.Model;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GustosApp.Application.Tests
{
    public class MarcarNotificacionLeidaUseCaseTest
    {
        [Fact]
        public async Task MarcarNotificacionLeida()
        {
            var repoMock = new Mock<INotificacionRepository>();
            var notificacionId = Guid.NewGuid();

           repoMock.Setup(r => r.MarcarComoLeidaAsync(notificacionId, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Verifiable();

            var useCaseLeida = new MarcarNotificacionLeidaUseCase(repoMock.Object);
            await useCaseLeida.HandleAsync(notificacionId,CancellationToken.None);
        }
    }
}
