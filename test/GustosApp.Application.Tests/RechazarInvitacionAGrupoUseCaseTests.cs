using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GustosApp.Application.UseCases.GrupoUseCases.InvitacionGrupoUseCases;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using Moq;

namespace GustosApp.Application.Tests
{
    public class RechazarInvitacionAGrupoUseCaseTests
    {
        private readonly Mock<IUsuarioRepository> _usuarioRepoMock;
        private readonly Mock<INotificacionRepository> _notificacionRepoMock;
        private readonly Mock<IInvitacionGrupoRepository> _invitacionRepoMock;

        private readonly RechazarInvitacionAGrupoUseCase _sut;

        public RechazarInvitacionAGrupoUseCaseTests()
        {
            _usuarioRepoMock = new Mock<IUsuarioRepository>();
            _notificacionRepoMock = new Mock<INotificacionRepository>();
            _invitacionRepoMock = new Mock<IInvitacionGrupoRepository>();

            _sut = new RechazarInvitacionAGrupoUseCase(
                _usuarioRepoMock.Object,
                _notificacionRepoMock.Object,
                _invitacionRepoMock.Object
            );
        }


        [Fact]
        public async Task HandleAsync_UsuarioNoEncontrado_LanzaUnauthorized()
        {
            string uid = "uid123";
            Guid notifId = Guid.NewGuid();

            _usuarioRepoMock
                .Setup(r => r.GetByFirebaseUidAsync(uid, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Usuario?)null);

            Func<Task> act = () => _sut.HandleAsync(uid, notifId, CancellationToken.None);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(act);
        }

        [Fact]
        public async Task HandleAsync_NotificacionNoEncontrada_LanzaArgumentException()
        {
            string uid = "uid123";
            Guid notifId = Guid.NewGuid();

            var usuario = new Usuario { Id = Guid.NewGuid() };

            _usuarioRepoMock
                .Setup(r => r.GetByFirebaseUidAsync(uid, It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            _notificacionRepoMock
                .Setup(r => r.GetByIdAsync(notifId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Notificacion?)null);

            Func<Task> act = () => _sut.HandleAsync(uid, notifId, CancellationToken.None);

            await Assert.ThrowsAsync<ArgumentException>(act);
        }

      
        [Fact]
        public async Task HandleAsync_NotificacionSinInvitacionId_LanzaArgumentException()
        {
            string uid = "uid123";
            Guid notifId = Guid.NewGuid();

            var usuario = new Usuario { Id = Guid.NewGuid() };
            var noti = new Notificacion { InvitacionId = null };

            _usuarioRepoMock.Setup(r => r.GetByFirebaseUidAsync(uid, It.IsAny<CancellationToken>()))
                            .ReturnsAsync(usuario);

            _notificacionRepoMock.Setup(r => r.GetByIdAsync(notifId, It.IsAny<CancellationToken>()))
                                 .ReturnsAsync(noti);

            Func<Task> act = () => _sut.HandleAsync(uid, notifId, CancellationToken.None);

            await Assert.ThrowsAsync<ArgumentException>(act);
        }

  
        [Fact]
        public async Task HandleAsync_InvitacionNoEncontrada_LanzaArgumentException()
        {
            string uid = "uid123";
            Guid notifId = Guid.NewGuid();
            Guid invitacionId = Guid.NewGuid();

            var usuario = new Usuario { Id = Guid.NewGuid() };
            var noti = new Notificacion { InvitacionId = invitacionId };

            _usuarioRepoMock.Setup(r => r.GetByFirebaseUidAsync(uid, It.IsAny<CancellationToken>()))
                            .ReturnsAsync(usuario);

            _notificacionRepoMock.Setup(r => r.GetByIdAsync(notifId, It.IsAny<CancellationToken>()))
                                 .ReturnsAsync(noti);

            _invitacionRepoMock.Setup(r => r.GetByIdAsync(invitacionId, It.IsAny<CancellationToken>()))
                               .ReturnsAsync((InvitacionGrupo?)null);

            Func<Task> act = () => _sut.HandleAsync(uid, notifId, CancellationToken.None);

            await Assert.ThrowsAsync<ArgumentException>(act);
        }


        [Fact]
        public async Task HandleAsync_InvitacionParaOtroUsuario_LanzaUnauthorized()
        {
            string uid = "uid123";
            Guid notifId = Guid.NewGuid();
            Guid invitacionId = Guid.NewGuid();

            var usuario = new Usuario { Id = Guid.NewGuid() };
            var noti = new Notificacion { InvitacionId = invitacionId };

            var invitacion = new InvitacionGrupo(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), null);
            // UsuarioInvitadoId es otro
            invitacion = new InvitacionGrupo(Guid.NewGuid(), Guid.NewGuid(), usuario.Id, null);

            _usuarioRepoMock.Setup(r => r.GetByFirebaseUidAsync(uid, It.IsAny<CancellationToken>()))
                            .ReturnsAsync(usuario);

            _notificacionRepoMock.Setup(r => r.GetByIdAsync(notifId, It.IsAny<CancellationToken>()))
                                 .ReturnsAsync(noti);

            _invitacionRepoMock.Setup(r => r.GetByIdAsync(invitacionId, It.IsAny<CancellationToken>()))
                               .ReturnsAsync(invitacion);

            Func<Task> act = () => _sut.HandleAsync(uid, notifId, CancellationToken.None);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(act);
        }

 
    
        [Fact]
        public async Task HandleAsync_InvitacionNoPendiente_LanzaArgumentException()
        {
            string uid = "uid123";
            Guid notifId = Guid.NewGuid();
            Guid invitacionId = Guid.NewGuid();

            var usuario = new Usuario { Id = Guid.NewGuid() };
            var noti = new Notificacion { InvitacionId = invitacionId };

            var invitacion = new InvitacionGrupo(Guid.NewGuid(), usuario.Id, Guid.NewGuid(), null);
            invitacion.Aceptar(); // ya no está pendiente

            _usuarioRepoMock.Setup(r => r.GetByFirebaseUidAsync(uid, It.IsAny<CancellationToken>()))
                            .ReturnsAsync(usuario);

            _notificacionRepoMock.Setup(r => r.GetByIdAsync(notifId, It.IsAny<CancellationToken>()))
                                 .ReturnsAsync(noti);

            _invitacionRepoMock.Setup(r => r.GetByIdAsync(invitacionId, It.IsAny<CancellationToken>()))
                               .ReturnsAsync(invitacion);

            Func<Task> act = () => _sut.HandleAsync(uid, notifId, CancellationToken.None);

            await Assert.ThrowsAsync<ArgumentException>(act);
        }

 
        [Fact]
        public async Task HandleAsync_InvitacionExpirada_LanzaArgumentException()
        {
            string uid = "uid123";
            Guid notifId = Guid.NewGuid();
            Guid invitacionId = Guid.NewGuid();

            var usuario = new Usuario { Id = Guid.NewGuid() };
            var noti = new Notificacion { InvitacionId = invitacionId };

            var invitacion = new InvitacionGrupo(Guid.NewGuid(), usuario.Id, Guid.NewGuid(), null);
            invitacion.FechaExpiracion = DateTime.UtcNow.AddMinutes(-5); // EXPIRED

            _usuarioRepoMock.Setup(r => r.GetByFirebaseUidAsync(uid, It.IsAny<CancellationToken>()))
                            .ReturnsAsync(usuario);

            _notificacionRepoMock.Setup(r => r.GetByIdAsync(notifId, It.IsAny<CancellationToken>()))
                                 .ReturnsAsync(noti);

            _invitacionRepoMock.Setup(r => r.GetByIdAsync(invitacionId, It.IsAny<CancellationToken>()))
                               .ReturnsAsync(invitacion);

            Func<Task> act = () => _sut.HandleAsync(uid, notifId, CancellationToken.None);

            await Assert.ThrowsAsync<ArgumentException>(act);
        }

   
        [Fact]
        public async Task HandleAsync_CasoFeliz_RechazaInvitacionYEliminaNotificacion()
        {
            string uid = "uid123";
            Guid notifId = Guid.NewGuid();
            Guid invitacionId = Guid.NewGuid();

            var usuario = new Usuario { Id = Guid.NewGuid(), FirebaseUid = uid };
            var notificacion = new Notificacion { InvitacionId = invitacionId };

            var invitacion = new InvitacionGrupo(Guid.NewGuid(), usuario.Id, Guid.NewGuid(), null)
            {
                FechaExpiracion = DateTime.UtcNow.AddMinutes(5) // NO expira
            };

            _usuarioRepoMock.Setup(r => r.GetByFirebaseUidAsync(uid, It.IsAny<CancellationToken>()))
                            .ReturnsAsync(usuario);

            _notificacionRepoMock.Setup(r => r.GetByIdAsync(notifId, It.IsAny<CancellationToken>()))
                                 .ReturnsAsync(notificacion);

            _invitacionRepoMock.Setup(r => r.GetByIdAsync(invitacionId, It.IsAny<CancellationToken>()))
                               .ReturnsAsync(invitacion);

            _invitacionRepoMock.Setup(r => r.UpdateAsync(invitacion, It.IsAny<CancellationToken>()))
                              .ReturnsAsync((InvitacionGrupo?)null);


            _notificacionRepoMock.Setup(r => r.EliminarAsync(notifId, It.IsAny<CancellationToken>()))
                                 .Returns(Task.CompletedTask);

            // Act
            await _sut.HandleAsync(uid, notifId, CancellationToken.None);

            // Assert
            Assert.Equal(EstadoInvitacion.Rechazada, invitacion.Estado);

            _notificacionRepoMock.Verify(r => r.EliminarAsync(notifId, It.IsAny<CancellationToken>()), Times.Once);
            _invitacionRepoMock.Verify(r => r.UpdateAsync(invitacion, It.IsAny<CancellationToken>()), Times.Once);
        }
    }

}