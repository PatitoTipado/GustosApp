using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using GustosApp.Application.UseCases.AmistadUseCases;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model.@enum;
using GustosApp.Domain.Model;
using Moq;

namespace GustosApp.Application.Tests
{
    public class RechazarSolicitudUseCaseTests
    {
        private readonly Mock<IUsuarioRepository> _usuarioRepo;
        private readonly Mock<ISolicitudAmistadRepository> _solicitudRepo;
        private readonly RechazarSolicitudUseCase _useCase;

        public RechazarSolicitudUseCaseTests()
        {
            _usuarioRepo = new Mock<IUsuarioRepository>();
            _solicitudRepo = new Mock<ISolicitudAmistadRepository>();

            _useCase = new RechazarSolicitudUseCase(
                _solicitudRepo.Object,
                _usuarioRepo.Object
            );
        }

        // Helpers
        private Usuario FakeUsuario(Guid id, string firebase = "uid", string username = "test")
        {
            return new Usuario
            {
                Id = id,
                FirebaseUid = firebase,
                IdUsuario = username
            };
        }

        private SolicitudAmistad FakeSolicitud(Guid remitente, Guid destinatario, EstadoSolicitud estado)
        {
            var s = new SolicitudAmistad(remitente, destinatario);

            typeof(SolicitudAmistad)
                .GetProperty(nameof(SolicitudAmistad.Estado))!
                .SetValue(s, estado);

            return s;
        }

        
        [Fact]
        public async Task HandleAsync_DeberiaFallar_SiUsuarioNoExiste()
        {
            _usuarioRepo.Setup(r => r.GetByFirebaseUidAsync("uid", It.IsAny<CancellationToken>()))
                .ReturnsAsync((Usuario?)null);

            Func<Task> act = () =>
                _useCase.HandleAsync("uid", Guid.NewGuid());

            await act.Should().ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("Usuario no encontrado");
        }

        [Fact]
        public async Task HandleAsync_DeberiaFallar_SiSolicitudNoExiste()
        {
            var user = FakeUsuario(Guid.NewGuid());

            _usuarioRepo.Setup(r => r.GetByFirebaseUidAsync("uid", CancellationToken.None)).ReturnsAsync(user);
            _solicitudRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((SolicitudAmistad?)null);

            Func<Task> act = () =>
                _useCase.HandleAsync("uid", Guid.NewGuid());

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("Solicitud no encontrada");
        }

        [Fact]
        public async Task HandleAsync_DeberiaFallar_SiUsuarioNoEsDestinatario()
        {
            var user = FakeUsuario(Guid.NewGuid());
            var solicitud = FakeSolicitud(Guid.NewGuid(), Guid.NewGuid(), EstadoSolicitud.Pendiente); // destinatario incorrecto

            _usuarioRepo.Setup(r => r.GetByFirebaseUidAsync("uid", CancellationToken.None)).ReturnsAsync(user);
            _solicitudRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), CancellationToken.None)).ReturnsAsync(solicitud);

            Func<Task> act = () =>
                _useCase.HandleAsync("uid", Guid.NewGuid());

            await act.Should().ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("Solo el destinatario puede rechazar la solicitud");

            _solicitudRepo.Verify(r =>
                r.UpdateAsync(It.IsAny<SolicitudAmistad>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

       

        [Theory]
        [InlineData(EstadoSolicitud.Aceptada)]
        [InlineData(EstadoSolicitud.Rechazada)]
        [InlineData(EstadoSolicitud.Expirada)]
        public async Task HandleAsync_DeberiaFallar_SiSolicitudNoEstaPendiente(EstadoSolicitud estado)
        {
            var userId = Guid.NewGuid();

            var user = FakeUsuario(userId);
            var solicitud = FakeSolicitud(Guid.NewGuid(), userId, estado);

            _usuarioRepo.Setup(r => r.GetByFirebaseUidAsync("uid", CancellationToken.None)).ReturnsAsync(user);
            _solicitudRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), CancellationToken.None)).ReturnsAsync(solicitud);

            Func<Task> act = () =>
                _useCase.HandleAsync("uid", Guid.NewGuid());

            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Solo se pueden rechazar solicitudes pendientes");

            _solicitudRepo.Verify(r =>
                r.UpdateAsync(It.IsAny<SolicitudAmistad>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

      

        [Fact]
        public async Task HandleAsync_DeberiaRechazarSolicitudCorrectamente()
        {
            var userId = Guid.NewGuid();
            var remitenteId = Guid.NewGuid();

            var user = FakeUsuario(userId);
            var solicitud = FakeSolicitud(remitenteId, userId, EstadoSolicitud.Pendiente);

            _usuarioRepo.Setup(r => r.GetByFirebaseUidAsync("uid", CancellationToken.None)).ReturnsAsync(user);
            _solicitudRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), CancellationToken.None)).ReturnsAsync(solicitud);
            _solicitudRepo.Setup(r => r.UpdateAsync(solicitud, It.IsAny<CancellationToken>()))
                .ReturnsAsync(solicitud);

            var result = await _useCase.HandleAsync("uid", Guid.NewGuid());

            result.Should().NotBeNull();
            result.Estado.Should().Be(EstadoSolicitud.Rechazada);
            result.FechaRespuesta.Should().NotBeNull();

            _solicitudRepo.Verify(r =>
                r.UpdateAsync(solicitud, It.IsAny<CancellationToken>()),
                Times.Once);
        }


        [Fact]
        public async Task HandleAsync_DeberiaFallar_SiUpdateAsyncFalla()
        {
            var userId = Guid.NewGuid();
            var remitenteId = Guid.NewGuid();

            var user = FakeUsuario(userId);
            var solicitud = FakeSolicitud(remitenteId, userId, EstadoSolicitud.Pendiente);

            _usuarioRepo.Setup(r => r.GetByFirebaseUidAsync("uid", CancellationToken.None)).ReturnsAsync(user);
            _solicitudRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), CancellationToken.None)).ReturnsAsync(solicitud);

            _solicitudRepo.Setup(r =>
                r.UpdateAsync(solicitud, It.IsAny<CancellationToken>())
            ).ThrowsAsync(new InvalidOperationException("Error al guardar"));

            Func<Task> act = () =>
                _useCase.HandleAsync("uid", Guid.NewGuid());

            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Error al guardar");

            solicitud.Estado.Should().Be(EstadoSolicitud.Rechazada);
        }
    }

}