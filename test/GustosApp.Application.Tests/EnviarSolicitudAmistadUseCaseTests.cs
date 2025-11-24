using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using GustosApp.Application.Interfaces;
using GustosApp.Application.UseCases.AmistadUseCases;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model.@enum;
using GustosApp.Domain.Model;
using Moq;

namespace GustosApp.Application.Tests
{
    public class EnviarSolicitudAmistadUseCaseTests
    {
        private readonly Mock<IUsuarioRepository> _usuarioRepo;
        private readonly Mock<ISolicitudAmistadRepository> _solicitudRepo;
        private readonly Mock<ISolicitudAmistadRealtimeService> _realtimeRepo;
        private readonly EnviarSolicitudAmistadUseCase _useCase;

        public EnviarSolicitudAmistadUseCaseTests()
        {
            _usuarioRepo = new Mock<IUsuarioRepository>();
            _solicitudRepo = new Mock<ISolicitudAmistadRepository>();
            _realtimeRepo = new Mock<ISolicitudAmistadRealtimeService>();

            _useCase = new EnviarSolicitudAmistadUseCase(
                _usuarioRepo.Object,
                _solicitudRepo.Object,
                _realtimeRepo.Object
            );
        }

        // Helpers
        private Usuario FakeUsuario(Guid id, string firebaseUid, string username)
        {
            return new Usuario
            {
                Id = id,
                FirebaseUid = firebaseUid,
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
        public async Task HandleAsync_DeberiaFallar_SiRemitenteNoExiste()
        {
            // Arrange
            _usuarioRepo
                .Setup(r => r.GetByFirebaseUidAsync("uid", It.IsAny<CancellationToken>()))
                .ReturnsAsync((Usuario?)null);

            // Act
            Func<Task> act = async () =>
                await _useCase.HandleAsync("uid", "destino", null);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("Usuario no encontrado");
        }

        [Fact]
        public async Task HandleAsync_DeberiaFallar_SiDestinatarioNoExiste()
        {
            // Arrange
            var remitente = FakeUsuario(Guid.NewGuid(), "uid", "juan");

            _usuarioRepo
                .Setup(r => r.GetByFirebaseUidAsync("uid", It.IsAny<CancellationToken>()))
                .ReturnsAsync(remitente);

            _usuarioRepo
                .Setup(r => r.GetByUsernameAsync("destino", It.IsAny<CancellationToken>()))
                .ReturnsAsync((Usuario?)null);

            // Act
            Func<Task> act = async () =>
                await _useCase.HandleAsync("uid", "destino", null);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("No se encontró un usuario con ese username");
        }

        [Fact]
        public async Task HandleAsync_NoDeberiaPermitirEnviarseSolicitudASiMismo()
        {
            // Arrange
            var user = FakeUsuario(Guid.NewGuid(), "uid", "juan");

            _usuarioRepo
                .Setup(r => r.GetByFirebaseUidAsync("uid", It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            _usuarioRepo
                .Setup(r => r.GetByUsernameAsync("juan", It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            // Act
            Func<Task> act = async () =>
                await _useCase.HandleAsync("uid", "juan", null);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("No puedes enviarte una solicitud a ti mismo");
        }

        [Fact]
        public async Task HandleAsync_DeberiaFallar_SiExisteAmistadAceptada()
        {
            // Arrange
            var remitente = FakeUsuario(Guid.NewGuid(), "uid", "juan");
            var destinatario = FakeUsuario(Guid.NewGuid(), "uid2", "pedro");

            _usuarioRepo.Setup(r => r.GetByFirebaseUidAsync("uid", It.IsAny<CancellationToken>()))
                .ReturnsAsync(remitente);
            _usuarioRepo.Setup(r => r.GetByUsernameAsync("pedro", It.IsAny<CancellationToken>()))
                .ReturnsAsync(destinatario);

            var amistadActiva = FakeSolicitud(remitente.Id, destinatario.Id, EstadoSolicitud.Aceptada);

            _solicitudRepo
                .Setup(r => r.GetAmistadEntreUsuariosAsync(remitente.Id, destinatario.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(amistadActiva);

            // Act
            Func<Task> act = async () =>
                await _useCase.HandleAsync("uid", "pedro", null);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("Ya existe una amistad activa con este usuario");
        }

        [Fact]
        public async Task HandleAsync_DeberiaFallar_SiExistePendiente_AHaciaB()
        {
            // Arrange
            var remitente = FakeUsuario(Guid.NewGuid(), "uid", "juan");
            var destinatario = FakeUsuario(Guid.NewGuid(), "uid2", "pedro");

            _usuarioRepo.Setup(r => r.GetByFirebaseUidAsync("uid", It.IsAny<CancellationToken>()))
                .ReturnsAsync(remitente);
            _usuarioRepo.Setup(r => r.GetByUsernameAsync("pedro", It.IsAny<CancellationToken>()))
                .ReturnsAsync(destinatario);

            _solicitudRepo.Setup(r =>
                r.GetAmistadEntreUsuariosAsync(remitente.Id, destinatario.Id, It.IsAny<CancellationToken>())
            ).ReturnsAsync((SolicitudAmistad?)null);

            _solicitudRepo.Setup(r =>
                r.ExisteSolicitudPendienteAsync(remitente.Id, destinatario.Id, It.IsAny<CancellationToken>())
            ).ReturnsAsync(true);

            // Act
            Func<Task> act = () =>
                _useCase.HandleAsync("uid", "pedro", null);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("Ya existe una solicitud pendiente entre estos usuarios");
        }

        [Fact]
        public async Task HandleAsync_DeberiaFallar_SiExistePendiente_BHaciaA()
        {
            // Arrange
            var remitente = FakeUsuario(Guid.NewGuid(), "uid", "juan");
            var destinatario = FakeUsuario(Guid.NewGuid(), "uid2", "pedro");

            _usuarioRepo.Setup(r => r.GetByFirebaseUidAsync("uid", It.IsAny<CancellationToken>()))
                .ReturnsAsync(remitente);
            _usuarioRepo.Setup(r => r.GetByUsernameAsync("pedro", It.IsAny<CancellationToken>()))
                .ReturnsAsync(destinatario);

            _solicitudRepo.Setup(r =>
                r.GetAmistadEntreUsuariosAsync(remitente.Id, destinatario.Id, It.IsAny<CancellationToken>())
            ).ReturnsAsync((SolicitudAmistad?)null);

            _solicitudRepo
                .Setup(r => r.ExisteSolicitudPendienteAsync(remitente.Id, destinatario.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _solicitudRepo
                .Setup(r => r.ExisteSolicitudPendienteAsync(destinatario.Id, remitente.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            Func<Task> act = () =>
                _useCase.HandleAsync("uid", "pedro", null);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("Ya existe una solicitud pendiente entre estos usuarios");
        }

        [Fact]
        public async Task HandleAsync_DeberiaCrearSolicitudCorrectamente()
        {
            // Arrange
            var remitente = FakeUsuario(Guid.NewGuid(), "uid", "juan");
            var destinatario = FakeUsuario(Guid.NewGuid(), "uid2", "pedro");

           


            _usuarioRepo.Setup(r => r.GetByFirebaseUidAsync("uid", CancellationToken.None)).ReturnsAsync(remitente);
            _usuarioRepo.Setup(r => r.GetByUsernameAsync("pedro", CancellationToken.None)).ReturnsAsync(destinatario);

            _solicitudRepo.Setup(r => r.GetAmistadEntreUsuariosAsync(remitente.Id, destinatario.Id, CancellationToken.None))
                .ReturnsAsync((SolicitudAmistad?)null);

            _solicitudRepo.Setup(r => r.ExisteSolicitudPendienteAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            SolicitudAmistad? solicitudCreada = null;

            _solicitudRepo.Setup(r => r.CreateAsync(It.IsAny<SolicitudAmistad>(), It.IsAny<CancellationToken>()))
                .Callback<SolicitudAmistad, CancellationToken>((s, _) => solicitudCreada = s)
                .ReturnsAsync((SolicitudAmistad s, CancellationToken _) => s);

            var completa = FakeSolicitud(remitente.Id, destinatario.Id, EstadoSolicitud.Pendiente);
            completa.Remitente = remitente;
            completa.Destinatario = destinatario;

            _solicitudRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(completa);

            // Act
            var result = await _useCase.HandleAsync("uid", "pedro", "hola");

            // Assert
            solicitudCreada.Should().NotBeNull();
            result.Should().NotBeNull();
            result.Remitente.Should().Be(remitente);
            result.Destinatario.Should().Be(destinatario);

            _solicitudRepo.Verify(r => r.CreateAsync(It.IsAny<SolicitudAmistad>(), It.IsAny<CancellationToken>()), Times.Once);
            _solicitudRepo.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
            _realtimeRepo.Verify(r => r.EnviarSolicitudAsync(destinatario.FirebaseUid, completa, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_DeberiaFallar_SiCreateAsyncFalla()
        {
            // Arrange
            var remitente = FakeUsuario(Guid.NewGuid(), "uid", "juan");
            var destinatario = FakeUsuario(Guid.NewGuid(), "uid2", "pedro");

            _usuarioRepo.Setup(r => r.GetByFirebaseUidAsync("uid",CancellationToken.None)).ReturnsAsync(remitente);
            _usuarioRepo.Setup(r => r.GetByUsernameAsync("pedro", CancellationToken.None)).ReturnsAsync(destinatario);

            _solicitudRepo.Setup(r => r.GetAmistadEntreUsuariosAsync(remitente.Id, destinatario.Id,CancellationToken.None))
                .ReturnsAsync((SolicitudAmistad?)null);

            _solicitudRepo.Setup(r =>
                r.ExisteSolicitudPendienteAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>())
            ).ReturnsAsync(false);

            _solicitudRepo
                .Setup(r => r.CreateAsync(It.IsAny<SolicitudAmistad>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Error al crear solicitud"));

            // Act
            Func<Task> act = async () => await _useCase.HandleAsync("uid", "pedro", null);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Error al crear solicitud");
        }
    }
    }
