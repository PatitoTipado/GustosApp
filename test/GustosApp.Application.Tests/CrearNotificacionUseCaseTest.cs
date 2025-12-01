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
        private readonly Mock<INotificacionRepository> _repo;
        private readonly CrearNotificacionUseCase _sut;

        public CrearNotificacionUseCaseTest()
        {
            _repo = new Mock<INotificacionRepository>();
            _sut = new CrearNotificacionUseCase(_repo.Object);
        }

        private void AssertRepoCalled(Guid usuarioDestino, TipoNotificacion tipo, string mensajeEsperado)
        {
            _repo.Verify(r => r.crearAsync(
                It.Is<Notificacion>(n =>
                    n.UsuarioDestinoId == usuarioDestino &&
                    n.Tipo == tipo &&
                    n.Mensaje == mensajeEsperado &&
                    n.Leida == false &&
                    n.Titulo == tipo.ToString()
                ),
                It.IsAny<CancellationToken>()),
            Times.Once);

            _repo.Invocations.Clear();
        }

        [Fact]
        public async Task CrearNotificacion_SolicitudAmistad()
        {
            var usuario = Guid.NewGuid();

            await _sut.HandleAsync(usuario, TipoNotificacion.SolicitudAmistad, "Juan", "NoImporta", CancellationToken.None);

            AssertRepoCalled(usuario,
                TipoNotificacion.SolicitudAmistad,
                "Juan te ha enviado una solicitud de amistad.");
        }

        [Fact]
        public async Task CrearNotificacion_InvitacionGrupo()
        {
            var usuario = Guid.NewGuid();

            await _sut.HandleAsync(usuario, TipoNotificacion.InvitacionGrupo, "Laura", "GrupoX", CancellationToken.None);

            AssertRepoCalled(usuario,
                TipoNotificacion.InvitacionGrupo,
                "Laura te ha invitado a unirte al grupo GrupoX.");
        }

        [Fact]
        public async Task CrearNotificacion_RecordatorioEvento()
        {
            var usuario = Guid.NewGuid();

            await _sut.HandleAsync(usuario, TipoNotificacion.RecordatorioEvento, "", "", CancellationToken.None);

            AssertRepoCalled(usuario,
                TipoNotificacion.RecordatorioEvento,
                "Recordatorio: Tienes un evento pendiente.");
        }

        [Fact]
        public async Task CrearNotificacion_MensajeNuevo()
        {
            var usuario = Guid.NewGuid();

            await _sut.HandleAsync(usuario, TipoNotificacion.MensajeNuevo, "Sofia", "", CancellationToken.None);

            AssertRepoCalled(usuario,
                TipoNotificacion.MensajeNuevo,
                "Nuevo mensaje de Sofia.");
        }

        [Fact]
        public async Task CrearNotificacion_Default()
        {
            var usuario = Guid.NewGuid();

            var tipo = (TipoNotificacion)999;

            await _sut.HandleAsync(usuario, tipo, "", "", CancellationToken.None);

            AssertRepoCalled(usuario,
                tipo,
                "Tienes una nueva notificación.");
        }
    }
    }
