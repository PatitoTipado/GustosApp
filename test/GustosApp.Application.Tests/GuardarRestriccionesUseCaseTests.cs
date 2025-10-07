using GustosApp.Application.UseCases;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using Moq;

namespace GustosApp.Application.Tests
{
    public class GuardarRestriccionesUseCaseTests
    {

        private readonly Mock<IUsuarioRepository> _userRepoMock;
        private readonly Mock<IRestriccionRepository> _restricRepoMock;
        private readonly GuardarRestriccionesUseCase _useCase;

        public GuardarRestriccionesUseCaseTests()
        {
            _userRepoMock = new Mock<IUsuarioRepository>();
            _restricRepoMock = new Mock<IRestriccionRepository>();

            _useCase = new GuardarRestriccionesUseCase(
                _userRepoMock.Object,
                _restricRepoMock.Object
            );
        }

        [Fact]
        public async Task HandleAsync_AgregaRestricciones_SiNoExisten()
        {
            // Arrange
            var uid = "abc123";
            var user = new Usuario(uid, "user@test.com", "Gonza", "Flores", "001", null);
            var restric1 = new Restriccion { Id = Guid.NewGuid(), Nombre = "Sin Harina" };
            var restric2 = new Restriccion { Id = Guid.NewGuid(), Nombre = "Sin Azúcar" };
            var ids = new List<Guid> { restric1.Id, restric2.Id };

            _userRepoMock.Setup(r => r.GetByFirebaseUidAsync(uid, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            _restricRepoMock.Setup(r => r.GetRestriccionesByIdsAsync(ids, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Restriccion> { restric1, restric2 });

            // Act
            await _useCase.HandleAsync(uid, ids, skip: false, CancellationToken.None);

            // Assert
            Assert.Equal(2, user.Restricciones.Count);
            Assert.Contains(user.Restricciones, r => r.Nombre == "Sin Harina");
            _userRepoMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_NoHaceNada_SiSkipEsTrue()
        {
            // Arrange
            var uid = "abc123";

            // Act
            await _useCase.HandleAsync(uid, new List<Guid>(), skip: true, CancellationToken.None);

            // Assert
            _userRepoMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task HandleAsync_NoDuplicaRestricciones()
        {
            // Arrange
            var uid = "abc123";
            var restric = new Restriccion { Id = Guid.NewGuid(), Nombre = "Sin Harina" };
            var user = new Usuario(uid, "user@test.com", "Gonza", "Flores", "001", null);
            user.Restricciones.Add(restric);

            _userRepoMock.Setup(r => r.GetByFirebaseUidAsync(uid, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            _restricRepoMock.Setup(r => r.GetRestriccionesByIdsAsync(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Restriccion> { restric });

            // Act
            await _useCase.HandleAsync(uid, new List<Guid> { restric.Id }, skip: false, CancellationToken.None);

            // Assert
            Assert.Single(user.Restricciones);
            _userRepoMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
