using GustosApp.Application.Interfaces;
using GustosApp.Application.UseCases;
using GustosApp.Domain;
using GustosApp.Domain.Model;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace GustosApp.Application.Tests
{
    public class CrearNotificacionUseCaseTest
    {
        [Fact]
        public async Task CrearNotificacion()
        {
            var repoMock = new Mock<INotificacionRepository>();
            var useCase = new CrearNotificacionUseCase(repoMock.Object);

            var usuarioDestino = Guid.NewGuid();
            var titulo = "Test";
            var mensaje = "Mensaje de prueba";

            await useCase.HandleAsync(usuarioDestino, titulo, mensaje, TipoNotificacion.InvitacionGrupo, CancellationToken.None);

            repoMock.Verify(r => r.crearAsync(It.IsAny<Notificacion>(),default),Times.Once);
        }

       
    }

    
}
