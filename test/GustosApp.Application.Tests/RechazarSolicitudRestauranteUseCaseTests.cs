using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using GustosApp.Application.Interfaces;
using GustosApp.Application.UseCases.RestauranteUseCases.SolicitudRestauranteUseCases;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model.@enum;
using GustosApp.Domain.Model;
using Moq;

namespace GustosApp.Application.Tests
{
    public class RechazarSolicitudRestauranteUseCaseTests
    {
        private readonly Mock<ISolicitudRestauranteRepository> _solicitudes;
        private readonly Mock<IUsuarioRepository> _usuarios;
        private readonly Mock<IFileStorageService> _firebase;
        private readonly Mock<IFirebaseAuthService> _auth;
        private readonly Mock<IEmailService> _email;
        private readonly Mock<IEmailTemplateService> _templates;

        private readonly RechazarSolicitudRestauranteUseCase _useCase;

        public RechazarSolicitudRestauranteUseCaseTests()
        {
            _solicitudes = new Mock<ISolicitudRestauranteRepository>();
            _usuarios = new Mock<IUsuarioRepository>();
            _firebase = new Mock<IFileStorageService>();
            _auth = new Mock<IFirebaseAuthService>();
            _email = new Mock<IEmailService>();
            _templates = new Mock<IEmailTemplateService>();

            _useCase = new RechazarSolicitudRestauranteUseCase(
                _solicitudes.Object,
                _usuarios.Object,
                _firebase.Object,
                _auth.Object,
                _email.Object,
                _templates.Object
            );
        }

        // Helpers
        private Usuario FakeUsuario(Guid id)
            => new Usuario
            {
                Id = id,
                FirebaseUid = "uid123",
                Email = "test@test.com",
                Nombre = "Gonza",
                Rol = RolUsuario.PendienteRestaurante
            };

        private SolicitudRestaurante FakeSolicitud(Guid id, Usuario user)
        {
            return new SolicitudRestaurante
            {
                Id = id,
                UsuarioId = user.Id,
                Usuario = user,
                Nombre = "MiResto",
                Direccion = "Calle 123",
                Estado = EstadoSolicitudRestaurante.Pendiente,
                Imagenes = new List<SolicitudRestauranteImagen>
            {
                new SolicitudRestauranteImagen { Url = "url1" },
                new SolicitudRestauranteImagen { Url = "url2" }
            }
            };
        }

       
        [Fact]
        public async Task HandleAsync_SolicitudNoExiste_DeberiaLanzarException()
        {
            // Arrange
            var id = Guid.NewGuid();
            _solicitudes.Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((SolicitudRestaurante?)null);

            // Act
            Func<Task> act = () => _useCase.HandleAsync(id, "motivo", default);

            // Assert
            await act.Should().ThrowAsync<Exception>()
                .WithMessage("Solicitud no encontrada");
        }

        
        [Fact]
        public async Task HandleAsync_SolicitudNoPendiente_DeberiaLanzarInvalidOperationException()
        {
            var id = Guid.NewGuid();
            var user = FakeUsuario(Guid.NewGuid());
            var solicitud = FakeSolicitud(id, user);
            solicitud.Estado = EstadoSolicitudRestaurante.Aprobada;

            _solicitudes.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(solicitud);

            Func<Task> act = () => _useCase.HandleAsync(id, "motivo", default);

            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Solo se pueden rechazar solicitudes pendientes.");
        }

       

        [Fact]
        public async Task HandleAsync_MotivoVacio_DeberiaLanzarArgumentException()
        {
            var id = Guid.NewGuid();
            var user = FakeUsuario(Guid.NewGuid());
            var solicitud = FakeSolicitud(id, user);

            _solicitudes.Setup(r => r.GetByIdAsync(id, default))
                .ReturnsAsync(solicitud);

            Func<Task> act = () => _useCase.HandleAsync(id, "   ", default);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("Debe especificar un motivo de rechazo.");
        }

    

        [Fact]
        public async Task HandleAsync_DeberiaRechazarSolicitud_YRealizarAccionesCompletas()
        {
            // Arrange
            var id = Guid.NewGuid();
            var user = FakeUsuario(Guid.NewGuid());
            var solicitud = FakeSolicitud(id, user);

            _solicitudes.Setup(r => r.GetByIdAsync(id, default))
                .ReturnsAsync(solicitud);

            _usuarios.Setup(u => u.UpdateAsync(user, default))
                .Returns(Task.CompletedTask);

            _usuarios.Setup(u => u.SaveChangesAsync(default))
                .Returns(Task.CompletedTask);

            _templates.Setup(t => t.Render("SolicitudRechazada.html", It.IsAny<Dictionary<string, string>>()))
                .Returns("HTML_BODY");

            _email.Setup(e =>
                e.EnviarEmailAsync(user.Email, "Tu solicitud fue rechazada", "HTML_BODY"
                , CancellationToken.None))
                .Returns(Task.CompletedTask);

            _auth.Setup(a =>
                a.SetUserRoleAsync(user.FirebaseUid, RolUsuario.Usuario.ToString()))
                .Returns(Task.CompletedTask);

            // Act
            await _useCase.HandleAsync(id, "Motivo de rechazo", default);

            // Assert → Estado
            solicitud.Estado.Should().Be(EstadoSolicitudRestaurante.Rechazada);
            solicitud.MotivoRechazo.Should().Be("Motivo de rechazo");

            // Rol restaurado
            solicitud.Usuario.Rol.Should().Be(RolUsuario.Usuario);

            // Firebase role assignment
            _auth.Verify(a =>
                a.SetUserRoleAsync(user.FirebaseUid, RolUsuario.Usuario.ToString()),
                Times.Once);

            // Eliminación de imágenes
            _firebase.Verify(f => f.DeleteFileAsync("url1"), Times.Once);
            _firebase.Verify(f => f.DeleteFileAsync("url2"), Times.Once);

            // Email
            _email.Verify(e =>
                e.EnviarEmailAsync(user.Email, "Tu solicitud fue rechazada", "HTML_BODY", 
                CancellationToken.None),
                Times.Once);

            // Update usuario
            _usuarios.Verify(u => u.UpdateAsync(user, default), Times.Once);
            _usuarios.Verify(u => u.SaveChangesAsync(default), Times.Once);

            // Update solicitud
            _solicitudes.Verify(s => s.UpdateAsync(solicitud, default), Times.Once);
        }
    }

    }
