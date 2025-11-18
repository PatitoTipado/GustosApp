using GustosApp.Application.Interfaces;
using GustosApp.Application.UseCases;
using GustosApp.Application.UseCases.NotificacionUseCases;
using GustosApp.Domain;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using GustosApp.Domain.Model.@enum;
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
            var nombreUsuario = "Gaston";
            var nombreGrupo = "GrupoTest";

            await useCase.HandleAsync(
                usuarioDestino,
                TipoNotificacion.InvitacionGrupo,
                nombreUsuario,
                nombreGrupo,
                CancellationToken.None
                );

            repoMock.Verify(r => r.crearAsync(
                It.Is<Notificacion>(n =>
                n.UsuarioDestinoId == usuarioDestino &&
                n.Tipo == TipoNotificacion.InvitacionGrupo),It.IsAny<CancellationToken>()), Times.Once);

             repoMock.Verify(r => r.crearAsync(It.IsAny<Notificacion>(),default),Times.Once);
        }


        }


    }
