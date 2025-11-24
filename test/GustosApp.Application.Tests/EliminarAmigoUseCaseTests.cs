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
    public class EliminarAmigoUseCaseTests
    {
        private readonly Mock<IUsuarioRepository> _usuarioRepo;
        private readonly Mock<ISolicitudAmistadRepository> _solicitudRepo;
        private readonly EliminarAmigoUseCase _useCase;

        public EliminarAmigoUseCaseTests()
        {
            _usuarioRepo = new Mock<IUsuarioRepository>();
            _solicitudRepo = new Mock<ISolicitudAmistadRepository>();

            _useCase = new EliminarAmigoUseCase(
                _solicitudRepo.Object,
                _usuarioRepo.Object
            );
        }

        private Usuario FakeUsuario(Guid id, string firebase, string username = "user")
        {
            return new Usuario
            {
                Id = id,
                FirebaseUid = firebase,
                IdUsuario = username
            };
        }

        private SolicitudAmistad FakeSolicitud(Guid a, Guid b)
        {
            var s = new SolicitudAmistad(a, b);

            typeof(SolicitudAmistad)
                .GetProperty(nameof(SolicitudAmistad.Estado))!
                .SetValue(s, EstadoSolicitud.Aceptada);

            return s;
        }

        [Fact]
        public async Task HandleAsync_DeberiaFallar_SiUsuarioNoExiste()
        {
            _usuarioRepo
                .Setup(r => r.GetByFirebaseUidAsync("uid", It.IsAny<CancellationToken>()))
                .ReturnsAsync((Usuario?)null);

            Func<Task> act = async () =>
                await _useCase.HandleAsync("uid", Guid.NewGuid());

            await act.Should().ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("Usuario no encontrado");
        }

        [Fact]
        public async Task HandleAsync_NoDeberiaPermitirAutoEliminacion()
        {
            var id = Guid.NewGuid();
            var user = FakeUsuario(id, "uid");

            _usuarioRepo.Setup(r => r.GetByFirebaseUidAsync("uid", CancellationToken.None)).ReturnsAsync(user);

            Func<Task> act = async () =>
                await _useCase.HandleAsync("uid", id);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("No podés eliminarte a vos mismo como amigo.");
        }

        [Fact]
        public async Task HandleAsync_DeberiaFallar_SiNoExisteAmistadActiva()
        {
            var user = FakeUsuario(Guid.NewGuid(), "uid");

            _usuarioRepo.Setup(r => r.GetByFirebaseUidAsync("uid", CancellationToken.None))
                .ReturnsAsync(user);

            _solicitudRepo
                .Setup(r => r.GetAmistadEntreUsuariosAsync(user.Id, It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((SolicitudAmistad?)null);

            Func<Task> act = () =>
                _useCase.HandleAsync("uid", Guid.NewGuid());

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("No se encontró una amistad activa con el usuario especificado");
        }

        [Fact]
        public async Task HandleAsync_DeberiaEliminarAmistadCorrectamente()
        {
            var userId = Guid.NewGuid();
            var amigoId = Guid.NewGuid();

            var user = FakeUsuario(userId, "uid");
            var amistad = FakeSolicitud(userId, amigoId);

            _usuarioRepo.Setup(r => r.GetByFirebaseUidAsync("uid", CancellationToken.None))
                .ReturnsAsync(user);

            _solicitudRepo.Setup(r =>
                r.GetAmistadEntreUsuariosAsync(userId, amigoId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(amistad);

            _solicitudRepo.Setup(r =>
                r.DeleteAsync(amistad.Id, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var result = await _useCase.HandleAsync("uid", amigoId);

            result.Should().BeTrue();

            _solicitudRepo.Verify(r =>
                r.DeleteAsync(amistad.Id, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task HandleAsync_DeberiaFallar_SiDeleteAsyncFalla()
        {
            var userId = Guid.NewGuid();
            var amigoId = Guid.NewGuid();

            var user = FakeUsuario(userId, "uid");
            var amistad = FakeSolicitud(userId, amigoId);

            _usuarioRepo.Setup(r => r.GetByFirebaseUidAsync("uid",CancellationToken.None))
                .ReturnsAsync(user);

            _solicitudRepo.Setup(r =>
                r.GetAmistadEntreUsuariosAsync(userId, amigoId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(amistad);

            _solicitudRepo.Setup(r => r.DeleteAsync(amistad.Id, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Error eliminando amistad"));

            Func<Task> act = async () =>
                await _useCase.HandleAsync("uid", amigoId);

            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Error eliminando amistad");
        }
    }
    }
