using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GustosApp.Application.UseCases.GrupoUseCases.InvitacionGrupoUseCases;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using Moq;
using Xunit;

namespace GustosApp.Application.Tests
{
    public class ObtenerInvitacionesUsuarioUseCaseTests
    {
        private readonly Mock<IInvitacionGrupoRepository> _invitacionRepositoryMock;
        private readonly Mock<IUsuarioRepository> _usuarioRepositoryMock;
        private readonly ObtenerInvitacionesUsuarioUseCase _sut;

        public ObtenerInvitacionesUsuarioUseCaseTests()
        {
            _invitacionRepositoryMock = new Mock<IInvitacionGrupoRepository>();
            _usuarioRepositoryMock = new Mock<IUsuarioRepository>();

            _sut = new ObtenerInvitacionesUsuarioUseCase(
                _invitacionRepositoryMock.Object,
                _usuarioRepositoryMock.Object);
        }

        private static Usuario CreateUsuario(Guid? id = null, string firebaseUid = "firebase-uid")
        {
            var type = typeof(Usuario);
            var usuario = (Usuario)Activator.CreateInstance(type, nonPublic: true)!;

            type.GetProperty("Id")?.SetValue(usuario, id ?? Guid.NewGuid());
            type.GetProperty("FirebaseUid")?.SetValue(usuario, firebaseUid);

            return usuario;
        }

        private static InvitacionGrupo CreateInvitacion(Guid? id = null)
        {
            var type = typeof(InvitacionGrupo);
            var invitacion = (InvitacionGrupo)Activator.CreateInstance(type, nonPublic: true)!;

            if (id.HasValue)
                type.GetProperty("Id")?.SetValue(invitacion, id.Value);

            return invitacion;
        }

        // Verifica que cuando el usuario no existe se lanza UnauthorizedAccessException con el mensaje correcto.
        [Fact]
        public async Task HandleAsync_UsuarioNoEncontrado_LanzaUnauthorizedAccessException()
        {
            var firebaseUid = "uid-inexistente";
            var ct = CancellationToken.None;

            _usuarioRepositoryMock
                .Setup(r => r.GetByFirebaseUidAsync(firebaseUid, ct))
                .ReturnsAsync((Usuario?)null);

            var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _sut.HandleAsync(firebaseUid, ct));

            Assert.Equal("Usuario no encontrado", ex.Message);

            _invitacionRepositoryMock.Verify(
                r => r.GetInvitacionesPendientesByUsuarioIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        // Verifica que cuando el usuario existe se devuelven las invitaciones pendientes obtenidas del repositorio.
        [Fact]
        public async Task HandleAsync_UsuarioValido_DevuelveInvitacionesPendientesDelRepositorio()
        {
            var firebaseUid = "uid-valido";
            var ct = CancellationToken.None;

            var usuarioId = Guid.NewGuid();
            var usuario = CreateUsuario(id: usuarioId, firebaseUid: firebaseUid);

            var invitacion1 = CreateInvitacion(Guid.NewGuid());
            var invitacion2 = CreateInvitacion(Guid.NewGuid());
            var invitacionesRepo = new List<InvitacionGrupo> { invitacion1, invitacion2 };

            _usuarioRepositoryMock
                .Setup(r => r.GetByFirebaseUidAsync(firebaseUid, ct))
                .ReturnsAsync(usuario);

            _invitacionRepositoryMock
                .Setup(r => r.GetInvitacionesPendientesByUsuarioIdAsync(usuarioId, ct))
                .ReturnsAsync(invitacionesRepo);

            var resultado = await _sut.HandleAsync(firebaseUid, ct);

            var lista = resultado.ToList();
            Assert.Equal(2, lista.Count);
            Assert.Same(invitacion1, lista[0]);
            Assert.Same(invitacion2, lista[1]);

            _usuarioRepositoryMock.Verify(
                r => r.GetByFirebaseUidAsync(firebaseUid, ct),
                Times.Once);

            _invitacionRepositoryMock.Verify(
                r => r.GetInvitacionesPendientesByUsuarioIdAsync(usuarioId, ct),
                Times.Once);
        }
    }
}
