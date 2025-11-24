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
    public class ObtenerSolicitudesPendientesUseCaseTests
    {
        private readonly Mock<IUsuarioRepository> _usuarioRepo;
        private readonly Mock<ISolicitudAmistadRepository> _solicitudRepo;
        private readonly ObtenerSolicitudesPendientesUseCase _useCase;

        public ObtenerSolicitudesPendientesUseCaseTests()
        {
            _usuarioRepo = new Mock<IUsuarioRepository>();
            _solicitudRepo = new Mock<ISolicitudAmistadRepository>();

            _useCase = new ObtenerSolicitudesPendientesUseCase(
                _solicitudRepo.Object,
                _usuarioRepo.Object
            );
        }

        // ------------------------
        // Helpers
        // ------------------------

        private Usuario FakeUsuario(Guid id, string firebase = "uid", string username = "test")
            => new Usuario
            {
                Id = id,
                FirebaseUid = firebase,
                IdUsuario = username
            };

        private SolicitudAmistad FakeSolicitud(Guid remitenteId, Guid destinatarioId)
        {
            var s = new SolicitudAmistad(remitenteId, destinatarioId);

            typeof(SolicitudAmistad)
                .GetProperty(nameof(SolicitudAmistad.Estado))!
                .SetValue(s, EstadoSolicitud.Pendiente);

            s.Remitente = FakeUsuario(remitenteId, "r", "rem");
            s.Destinatario = FakeUsuario(destinatarioId, "d", "dest");

            return s;
        }

        [Fact]
        public async Task HandleAsync_DeberiaFallar_SiUsuarioNoExiste()
        {
            _usuarioRepo.Setup(r => r.GetByFirebaseUidAsync("uid", It.IsAny<CancellationToken>()))
                .ReturnsAsync((Usuario?)null);

            Func<Task> act = () =>
                _useCase.HandleAsync("uid");

            await act.Should().ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("Usuario no encontrado");

            _solicitudRepo.Verify(
                r => r.GetSolicitudesPendientesByUsuarioIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        [Fact]
        public async Task HandleAsync_DeberiaRetornarListaVacia_SiNoHayPendientes()
        {
            var user = FakeUsuario(Guid.NewGuid());

            _usuarioRepo.Setup(r => r.GetByFirebaseUidAsync("uid", CancellationToken.None))
                .ReturnsAsync(user);

            _solicitudRepo
                .Setup(r => r.GetSolicitudesPendientesByUsuarioIdAsync(user.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<SolicitudAmistad>());

            var result = await _useCase.HandleAsync("uid");

            result.Should().NotBeNull();
            result.Should().BeEmpty();

            _solicitudRepo.Verify(
                r => r.GetSolicitudesPendientesByUsuarioIdAsync(user.Id, It.IsAny<CancellationToken>()),
                Times.Once
            );
        }

        [Fact]
        public async Task HandleAsync_DeberiaRetornarSolicitudesPendientesCorrectamente()
        {
            var userId = Guid.NewGuid();
            var user = FakeUsuario(userId);

            var solicitud1 = FakeSolicitud(Guid.NewGuid(), userId);
            var solicitud2 = FakeSolicitud(Guid.NewGuid(), userId);

            var lista = new List<SolicitudAmistad> { solicitud1, solicitud2 };

            _usuarioRepo.Setup(r => r.GetByFirebaseUidAsync("uid", CancellationToken.None))
                .ReturnsAsync(user);

            _solicitudRepo.Setup(r =>
                r.GetSolicitudesPendientesByUsuarioIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(lista);

            var result = await _useCase.HandleAsync("uid");

            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Should().Contain(solicitud1);
            result.Should().Contain(solicitud2);

            // Navegaciones
            foreach (var sol in result)
            {
                sol.Remitente.Should().NotBeNull();
                sol.Destinatario.Should().NotBeNull();
            }

            _solicitudRepo.Verify(
                r => r.GetSolicitudesPendientesByUsuarioIdAsync(userId, It.IsAny<CancellationToken>()),
                Times.Once
            );
        }

        [Fact]
        public async Task HandleAsync_DeberiaPropagarExcepcionDelRepositorio()
        {
            var user = FakeUsuario(Guid.NewGuid());

            _usuarioRepo.Setup(r => r.GetByFirebaseUidAsync("uid", CancellationToken.None))
                .ReturnsAsync(user);

            _solicitudRepo.Setup(r =>
                r.GetSolicitudesPendientesByUsuarioIdAsync(user.Id, It.IsAny<CancellationToken>())
            ).ThrowsAsync(new InvalidOperationException("DB Error"));

            Func<Task> act = () => _useCase.HandleAsync("uid");

            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("DB Error");
        }
    }

}