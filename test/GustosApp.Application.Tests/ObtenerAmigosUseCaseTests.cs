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
    public class ObtenerAmigosUseCaseTests
    {
        private readonly Mock<IUsuarioRepository> _usuarioRepo;
        private readonly Mock<ISolicitudAmistadRepository> _solicitudRepo;
        private readonly ObtenerAmigosUseCase _useCase;

        public ObtenerAmigosUseCaseTests()
        {
            _usuarioRepo = new Mock<IUsuarioRepository>();
            _solicitudRepo = new Mock<ISolicitudAmistadRepository>();

            _useCase = new ObtenerAmigosUseCase(
                _solicitudRepo.Object,
                _usuarioRepo.Object
            );
        }

        // Helpers
        private Usuario FakeUsuario(Guid id, string uid = "uid", string username = "user")
            => new Usuario
            {
                Id = id,
                FirebaseUid = uid,
                IdUsuario = username
            };

        private SolicitudAmistad FakeSolicitudAceptada(Guid remitenteId, Guid destinatarioId)
        {
            var s = new SolicitudAmistad(remitenteId, destinatarioId);

            typeof(SolicitudAmistad)
                .GetProperty(nameof(SolicitudAmistad.Estado))!
                .SetValue(s, EstadoSolicitud.Aceptada);

            s.Remitente = FakeUsuario(remitenteId, "R", "Rem");
            s.Destinatario = FakeUsuario(destinatarioId, "D", "Dest");

            return s;
        }


     

        [Fact]
        public async Task HandleAsync_DeberiaFallar_SiUsuarioNoExiste()
        {
            _usuarioRepo.Setup(r => r.GetByFirebaseUidAsync("uid", It.IsAny<CancellationToken>()))
                .ReturnsAsync((Usuario?)null);

            Func<Task> act = () => _useCase.HandleAsync("uid");

            await act.Should().ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("Usuario no encontrado");

            _solicitudRepo.Verify(r =>
                r.GetAmigosByUsuarioIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task HandleAsync_DeberiaRetornarListaVacia_SiNoHayAmigos()
        {
            var user = FakeUsuario(Guid.NewGuid());

            _usuarioRepo.Setup(r => r.GetByFirebaseUidAsync("uid",CancellationToken.None)).ReturnsAsync(user);
            _solicitudRepo.Setup(r => r.GetAmigosByUsuarioIdAsync(user.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Usuario>());

            var result = await _useCase.HandleAsync("uid");

            result.Should().NotBeNull();
            result.Should().BeEmpty();

            _solicitudRepo.Verify(r =>
                r.GetAmigosByUsuarioIdAsync(user.Id, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task HandleAsync_DeberiaRetornarAmigoSiUsuarioEsRemitente()
        {
            var userId = Guid.NewGuid();
            var amigoId = Guid.NewGuid();

            var user = FakeUsuario(userId);
            var amigo = FakeUsuario(amigoId);

            _usuarioRepo.Setup(r => r.GetByFirebaseUidAsync("uid", CancellationToken.None)).ReturnsAsync(user);
            _solicitudRepo.Setup(r => r.GetAmigosByUsuarioIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Usuario> { amigo });

            var result = await _useCase.HandleAsync("uid");

            result.Should().HaveCount(1);
            result.Should().Contain(amigo);
        }

        [Fact]
        public async Task HandleAsync_DeberiaRetornarAmigoSiUsuarioEsDestinatario()
        {
            var userId = Guid.NewGuid();
            var amigo = FakeUsuario(Guid.NewGuid());

            var user = FakeUsuario(userId);

            _usuarioRepo.Setup(r => r.GetByFirebaseUidAsync("uid", CancellationToken.None)).ReturnsAsync(user);
            _solicitudRepo.Setup(r => r.GetAmigosByUsuarioIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Usuario> { amigo });

            var result = await _useCase.HandleAsync("uid");

            result.Should().ContainSingle(a => a.Id == amigo.Id);
        }

        [Fact]
        public async Task HandleAsync_DeberiaRetornarMultiplesAmigos()
        {
            var userId = Guid.NewGuid();

            var user = FakeUsuario(userId);
            var amigo1 = FakeUsuario(Guid.NewGuid());
            var amigo2 = FakeUsuario(Guid.NewGuid());

            var lista = new List<Usuario> { amigo1, amigo2 };

            _usuarioRepo.Setup(r => r.GetByFirebaseUidAsync("uid", CancellationToken.None)).ReturnsAsync(user);
            _solicitudRepo.Setup(r => r.GetAmigosByUsuarioIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(lista);

            var result = await _useCase.HandleAsync("uid");

            result.Should().HaveCount(2);
            result.Should().Contain(lista);
        }

        [Fact]
        public async Task HandleAsync_DeberiaPropagarErrorDelRepositorio()
        {
            var user = FakeUsuario(Guid.NewGuid());

            _usuarioRepo.Setup(r => r.GetByFirebaseUidAsync("uid", CancellationToken.None)).ReturnsAsync(user);

            _solicitudRepo.Setup(r =>
                r.GetAmigosByUsuarioIdAsync(user.Id, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("DB Error"));

            Func<Task> act = () => _useCase.HandleAsync("uid");

            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("DB Error");
        }
    }
}
