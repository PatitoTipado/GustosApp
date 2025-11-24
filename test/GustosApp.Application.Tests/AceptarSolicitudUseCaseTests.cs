using GustosApp.Application.UseCases.AmistadUseCases;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model.@enum;
using GustosApp.Domain.Model;
using Moq;
using FluentAssertions;

namespace GustosApp.Application.Tests 
{

    public class AceptarSolicitudUseCaseTests
    {
        private readonly Mock<ISolicitudAmistadRepository> _solicitudRepoMock;
        private readonly Mock<IUsuarioRepository> _usuarioRepoMock;
        private readonly Mock<IMiembroGrupoRepository> _miembroRepoMock;
        private readonly AceptarSolicitudUseCase _useCase;

        public AceptarSolicitudUseCaseTests()
        {
            _solicitudRepoMock = new Mock<ISolicitudAmistadRepository>();
            _usuarioRepoMock = new Mock<IUsuarioRepository>();
            _miembroRepoMock = new Mock<IMiembroGrupoRepository>();

            _useCase = new AceptarSolicitudUseCase(
                _solicitudRepoMock.Object,
                _usuarioRepoMock.Object,
                _miembroRepoMock.Object
            );
        }

        // Helpers
        private Usuario FakeUsuario(Guid id, string firebaseUid)
        {
            return new Usuario
            {
                Id = id,
                FirebaseUid = firebaseUid
            };
        }

        private SolicitudAmistad FakeSolicitud(Guid remitenteId, Guid destinatarioId, EstadoSolicitud estado = EstadoSolicitud.Pendiente)
        {
            var solicitud = new SolicitudAmistad(remitenteId, destinatarioId);

            typeof(SolicitudAmistad)
                .GetProperty(nameof(SolicitudAmistad.Estado))!
                .SetValue(solicitud, estado);

            return solicitud;
        }

        [Fact]
        public async Task HandleAsync_DeberiaFallar_SiUsuarioNoExiste()
        {
            // Arrange
            _usuarioRepoMock
                .Setup(r => r.GetByFirebaseUidAsync("uid", It.IsAny<CancellationToken>()))
                .ReturnsAsync((Usuario?)null);

            // Act
            Func<Task> act = async () => await _useCase.HandleAsync("uid", Guid.NewGuid());

            // Assert
            await act.Should().ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("Usuario no encontrado");
        }

        [Fact]
        public async Task HandleAsync_DeberiaFallar_SiSolicitudNoExiste()
        {
            // Arrange
            var usuario = FakeUsuario(Guid.NewGuid(), "uid");

            _usuarioRepoMock
                .Setup(r => r.GetByFirebaseUidAsync("uid", It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            _solicitudRepoMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((SolicitudAmistad?)null);

            // Act
            Func<Task> act = async () => await _useCase.HandleAsync("uid", Guid.NewGuid());

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("Solicitud no encontrada");
        }

        [Fact]
        public async Task HandleAsync_DeberiaFallar_SiUsuarioNoEsDestinatario()
        {
            // Arrange
            var usuario = FakeUsuario(Guid.NewGuid(), "uid");
            var solicitud = FakeSolicitud(Guid.NewGuid(), Guid.NewGuid()); // distinto destinatario

            _usuarioRepoMock
                .Setup(r => r.GetByFirebaseUidAsync("uid", It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            _solicitudRepoMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(solicitud);

            // Act
            Func<Task> act = async () => await _useCase.HandleAsync("uid", Guid.NewGuid());

            // Assert
            await act.Should().ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("Solo el destinatario puede aceptar la solicitud");
        }

        [Fact]
        public async Task HandleAsync_DeberiaAceptarSolicitudPendiente()
        {
            // Arrange
            var destinatarioId = Guid.NewGuid();
            var usuario = FakeUsuario(destinatarioId, "uid");
            var solicitud = FakeSolicitud(Guid.NewGuid(), destinatarioId);

            _usuarioRepoMock
                .Setup(r => r.GetByFirebaseUidAsync("uid", It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            _solicitudRepoMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(solicitud);

            _solicitudRepoMock
                .Setup(r => r.UpdateAsync(solicitud, It.IsAny<CancellationToken>()))
                .ReturnsAsync(solicitud);

            // Act
            var result = await _useCase.HandleAsync("uid", Guid.NewGuid());

            // Assert
            result.Estado.Should().Be(EstadoSolicitud.Aceptada);
            result.FechaRespuesta.Should().NotBeNull();

            _solicitudRepoMock.Verify(r => r.UpdateAsync(solicitud, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory]
        [InlineData(EstadoSolicitud.Aceptada)]
        [InlineData(EstadoSolicitud.Rechazada)]
        [InlineData(EstadoSolicitud.Expirada)]
        public async Task HandleAsync_DeberiaFallar_SiSolicitudNoEstaPendiente(EstadoSolicitud estado)
        {
            // Arrange
            var destId = Guid.NewGuid();
            var usuario = FakeUsuario(destId, "uid");

            var solicitud = FakeSolicitud(Guid.NewGuid(), destId, estado);

            _usuarioRepoMock
                .Setup(r => r.GetByFirebaseUidAsync("uid", It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            _solicitudRepoMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(solicitud);

            // Act
            Func<Task> act = async () => await _useCase.HandleAsync("uid", Guid.NewGuid());

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Solo se pueden aceptar solicitudes pendientes");

            _solicitudRepoMock.Verify(r =>
                r.UpdateAsync(It.IsAny<SolicitudAmistad>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task HandleAsync_DeberiaRetornarSolicitudActualizada()
        {
            // Arrange
            var destId = Guid.NewGuid();
            var usuario = FakeUsuario(destId, "uid");
            var solicitud = FakeSolicitud(Guid.NewGuid(), destId);

            _usuarioRepoMock
                .Setup(r => r.GetByFirebaseUidAsync("uid", It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            _solicitudRepoMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(solicitud);

            _solicitudRepoMock
                .Setup(r => r.UpdateAsync(solicitud, It.IsAny<CancellationToken>()))
                .ReturnsAsync(solicitud);

            // Act
            var result = await _useCase.HandleAsync("uid", Guid.NewGuid());

            // Assert
            result.Should().Be(solicitud);
        }
    }
}
