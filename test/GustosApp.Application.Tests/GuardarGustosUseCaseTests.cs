using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GustosApp.Application.UseCases;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using Moq;

namespace GustosApp.Application.Tests
{
    public class GuardarGustosUseCaseTests
    {
        private readonly Mock<IUsuarioRepository> _mockUsuarioRepo;
        private readonly Mock<IGustoRepository> _mockGustoRepo;
        private readonly GuardarGustosUseCase _useCase;

        public GuardarGustosUseCaseTests()
        {
            _mockUsuarioRepo = new Mock<IUsuarioRepository>();
            _mockGustoRepo = new Mock<IGustoRepository>();
            _useCase = new GuardarGustosUseCase(_mockUsuarioRepo.Object, _mockGustoRepo.Object);
        }

        [Fact]
        public async Task HandleAsync_DeberiaLanzarErrorSiMenosDeTresGustos()
        {
            // Arrange
            var uid = "user-123";
            var ids = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() }; // solo 2 gustos

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _useCase.HandleAsync(uid, ids, CancellationToken.None));
        }

        [Fact]
        public async Task HandleAsync_DeberiaLanzarErrorSiUsuarioNoExiste()
        {
            // Arrange
            var uid = "inexistente";
            var ids = new List<Guid> { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };

            _mockUsuarioRepo.Setup(r => r.GetByFirebaseUidAsync(uid, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Usuario)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _useCase.HandleAsync(uid, ids, CancellationToken.None));
        }

        [Fact]
        public async Task HandleAsync_DeberiaLanzarErrorSiNoHayGustosValidos()
        {
            // Arrange
            var uid = "uid-valido";
            var usuario = new Usuario(uid, "user@test.com", "Ana", "Lopez", "id123");

            _mockUsuarioRepo.Setup(r => r.GetByFirebaseUidAsync(uid, It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            _mockGustoRepo.Setup(r => r.GetByIdsAsync(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Gusto>()); // vacío

            var ids = new List<Guid> { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _useCase.HandleAsync(uid, ids, CancellationToken.None));
        }

        [Fact]
        public async Task HandleAsync_DeberiaGuardarGustosYActualizarPaso()
        {
            // Arrange
            var uid = "user-abc";
            var usuario = new Usuario(uid, "user@test.com", "Pedro", "Ramirez", "u001");

            var gustoPizza = new Gusto { Id = Guid.NewGuid(), Nombre = "Pizza" };
            var gustoSushi = new Gusto { Id = Guid.NewGuid(), Nombre = "Sushi" };
            var gustoHamburguesa = new Gusto { Id = Guid.NewGuid(), Nombre = "Hamburguesa" };

            var gustos = new List<Gusto> { gustoPizza, gustoSushi, gustoHamburguesa };

            _mockUsuarioRepo.Setup(r => r.GetByFirebaseUidAsync(uid, It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            _mockGustoRepo.Setup(r => r.GetByIdsAsync(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(gustos);

            _mockUsuarioRepo.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var conflictos = await _useCase.HandleAsync(uid, gustos.Select(g => g.Id).ToList(), CancellationToken.None);

            // Assert
            Assert.Empty(conflictos); // No hay conflictos
            Assert.Equal(3, usuario.Gustos.Count);
            Assert.Equal(RegistroPaso.Verificacion, usuario.PasoActual);
            _mockUsuarioRepo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_DeberiaDevolverConflictosSiLosHay()
        {
            // Arrange
            var uid = "user-xyz";
            var gustoCarne = new Gusto
            {
                Id = Guid.NewGuid(),
                Nombre = "Carne",
                Tags = new List<Tag> { new Tag { Nombre = "carne" } }
            };
            var gustoEnsalada = new Gusto
            {
                Id = Guid.NewGuid(),
                Nombre = "Ensalada",
                Tags = new List<Tag> { new Tag { Nombre = "vegetales" } }
            };

            var usuario = new Usuario(uid, "mail@test.com", "Juan", "Perez", "id001");
            var restriccionSinCarne = new Restriccion
            {
                Id = Guid.NewGuid(),
                Nombre = "Sin carne",
                TagsProhibidos = new List<Tag> { new Tag { Nombre = "carne" } }
            };
            usuario.Restricciones.Add(restriccionSinCarne);

            _mockUsuarioRepo.Setup(r => r.GetByFirebaseUidAsync(uid, It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            _mockGustoRepo.Setup(r => r.GetByIdsAsync(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Gusto> { gustoCarne, gustoEnsalada });

            _mockUsuarioRepo.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var conflictos = await _useCase.HandleAsync(uid, new List<Guid> { gustoCarne.Id, gustoEnsalada.Id, Guid.NewGuid() }, CancellationToken.None);

            // Assert
            Assert.Single(conflictos); // solo "Carne" es conflictivo
            Assert.DoesNotContain(usuario.Gustos, g => g.Nombre == "Carne");
            Assert.Contains(usuario.Gustos, g => g.Nombre == "Ensalada");
        }
    }
}
