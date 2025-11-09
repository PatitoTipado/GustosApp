using GustosApp.Application.UseCases.UsuarioUseCases.RestriccionesUseCases;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using Moq;

namespace GustosApp.Application.Tests
{
    public class GuardarRestriccionesUseCaseTests
    {
        private readonly Mock<IUsuarioRepository> _usuarioRepoMock;
        private readonly Mock<IRestriccionRepository> _restriccionRepoMock;
        private readonly GuardarRestriccionesUseCase _useCase;

        public GuardarRestriccionesUseCaseTests()
        {
            _usuarioRepoMock = new Mock<IUsuarioRepository>();
            _restriccionRepoMock = new Mock<IRestriccionRepository>();
            _useCase = new GuardarRestriccionesUseCase(_usuarioRepoMock.Object, _restriccionRepoMock.Object);
        }

        [Fact]
        public async Task HandleAsync_DeberiaGuardarRestriccionesYRemoverGustosIncompatibles()
        {
            // Arrange
            var uid = "firebase_123";
            var ids = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
            var ct = CancellationToken.None;

            var usuario = new Usuario(uid, "test@mail.com", "Juan", "Pérez", "USR1")
            {
                Gustos = new List<Gusto>
            {
                new Gusto { Id = Guid.NewGuid(), Nombre = "Pizza" },
                new Gusto { Id = Guid.NewGuid(), Nombre = "Sushi" }
            }
            };

            _usuarioRepoMock.Setup(r => r.GetByFirebaseUidAsync(uid, ct))
                .ReturnsAsync(usuario);

            var restricciones = new List<Restriccion>
        {
            new Restriccion { Id = ids[0], Nombre = "Gluten" },
            new Restriccion { Id = ids[1], Nombre = "Lactosa" }
        };

            _restriccionRepoMock.Setup(r => r.GetRestriccionesByIdsAsync(ids, ct))
                .ReturnsAsync(restricciones);

            _usuarioRepoMock.Setup(r => r.SaveChangesAsync(ct))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _useCase.HandleAsync(uid, ids, skip: false, ct);

            // Assert
            _usuarioRepoMock.Verify(r => r.SaveChangesAsync(ct), Times.Once);
            Assert.NotEmpty(usuario.Restricciones);
            Assert.Equal(2, usuario.Restricciones.Count);
        }

        [Fact]
        public async Task HandleAsync_DeberiaSkipearCuandoSkipTrue()
        {
            // Arrange
            var uid = "firebase_123";
            var ct = CancellationToken.None;

            // Act
            var result = await _useCase.HandleAsync(uid, new List<Guid>(), skip: true, ct);

            // Assert
            Assert.Empty(result);
            _usuarioRepoMock.Verify(r => r.SaveChangesAsync(ct), Times.Never);
        }

        [Fact]
        public async Task HandleAsync_DeberiaTirarThrowCuandoNoEncuentraAlUsuario()
        {
            // Arrange
            var uid = "firebase_999";
            var ct = CancellationToken.None;

            _usuarioRepoMock.Setup(r => r.GetByFirebaseUidAsync(uid, ct))
                .ReturnsAsync((Usuario?)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _useCase.HandleAsync(uid, new List<Guid> { Guid.NewGuid() }, false, ct));
        }
    }
    }
