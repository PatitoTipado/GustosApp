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
    public class ObtenerGustosFiltradosUseCaseTests
    {
        private readonly Mock<IUsuarioRepository> _mockUsuarioRepo;
        private readonly Mock<IGustoRepository> _mockGustoRepo;
        private readonly ObtenerGustosFiltradosUseCase _useCase;

        public ObtenerGustosFiltradosUseCaseTests()
        {
            _mockUsuarioRepo = new Mock<IUsuarioRepository>();
            _mockGustoRepo = new Mock<IGustoRepository>();
            _useCase = new ObtenerGustosFiltradosUseCase(_mockUsuarioRepo.Object, _mockGustoRepo.Object);
        }

        [Fact]
        public async Task HandleAsync_FiltraGustosConTagsProhibidos_Correctamente()
        {
            // Arrange
            var uid = "firebase-uid-test";
        
            var tagHarina = new Tag { Id = Guid.NewGuid(), Nombre = "harina" };
            var tagTomate = new Tag { Id = Guid.NewGuid(), Nombre = "tomate" };

            // Restricción: sin harina
            var restriccionSinHarina = new Restriccion
            {
                Id = Guid.NewGuid(),
                Nombre = "Sin harina",
                TagsProhibidos = new List<Tag> { tagHarina }
            };

            // Usuario con esa restricción
            var usuario = new Usuario(uid, "test@mail.com", "Juan", "Perez", "u123")
            {
                Restricciones = new List<Restriccion> { restriccionSinHarina },
                CondicionesMedicas = new List<CondicionMedica>()
            };

            // Gustos (Pizza tiene harina, Ensalada no)
            var gustoPizza = new Gusto
            {
                Id = Guid.NewGuid(),
                Nombre = "Pizza",
                Tags = new List<Tag> { tagHarina, tagTomate },
                ImagenUrl = "pizza.jpg"
            };

            var gustoEnsalada = new Gusto
            {
                Id = Guid.NewGuid(),
                Nombre = "Ensalada",
                Tags = new List<Tag> { tagTomate },
                ImagenUrl = "ensalada.jpg"
            };

            var gustos = new List<Gusto> { gustoPizza, gustoEnsalada };

            // Configurar mocks
            _mockUsuarioRepo.Setup(r => r.GetByFirebaseUidAsync(uid, It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            _mockGustoRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(gustos);

            // Act
            var resultado = await _useCase.HandleAsync(uid, CancellationToken.None);

            // Assert
            Assert.Single(resultado); // Solo debería quedar 1
            Assert.Equal("Ensalada", resultado.First().Nombre); // Ensalada pasa el filtro
        }

        [Fact]
        public async Task HandleAsync_SinRestriccionesYCondiciones_DevuelveTodosLosGustos()
        {
            // Arrange
            var uid = "uid-sin-restricciones";
            var usuario = new Usuario(uid, "user@mail.com", "Ana", "Lopez", "u456");

            var gusto1 = new Gusto { Id = Guid.NewGuid(), Nombre = "Pizza", Tags = new List<Tag>() };
            var gusto2 = new Gusto { Id = Guid.NewGuid(), Nombre = "Sushi", Tags = new List<Tag>() };

            var gustos = new List<Gusto> { gusto1, gusto2 };

            _mockUsuarioRepo.Setup(r => r.GetByFirebaseUidAsync(uid, It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            _mockGustoRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(gustos);

            // Act
            var resultado = await _useCase.HandleAsync(uid, CancellationToken.None);

            // Assert
            Assert.Equal(2, resultado.Count);
            Assert.Contains(resultado, g => g.Nombre == "Pizza");
            Assert.Contains(resultado, g => g.Nombre == "Sushi");
         
        }

        [Fact]
        public async Task HandleAsync_UsuarioNoExiste_LanzaExcepcion()
        {
            // Arrange
            var uid = "no-existe";
            _mockUsuarioRepo.Setup(r => r.GetByFirebaseUidAsync(uid, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Usuario)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _useCase.HandleAsync(uid, CancellationToken.None));
        }
    }
}
