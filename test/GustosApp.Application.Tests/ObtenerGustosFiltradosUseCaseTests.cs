using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using GustosApp.API.DTO;
using Moq;
using Xunit;
using GustosApp.Application.UseCases.UsuarioUseCases.GustoUseCases;

namespace GustosApp.Application.Tests.UseCases
{
    public class ObtenerGustosFiltradosUseCaseTests
    {
        private readonly Mock<IUsuarioRepository> _usuarioRepoMock;
        private readonly Mock<IGustoRepository> _gustoRepoMock;
        private readonly ObtenerGustosFiltradosUseCase _useCase;

        public ObtenerGustosFiltradosUseCaseTests()
        {
            _usuarioRepoMock = new Mock<IUsuarioRepository>();
            _gustoRepoMock = new Mock<IGustoRepository>();
            _useCase = new ObtenerGustosFiltradosUseCase(_usuarioRepoMock.Object, _gustoRepoMock.Object);
        }

        [Fact]
        public async Task HandleAsync_DeberiaRetornarSoloGustosCompatibles()
        {
            // Arrange
            var uid = "firebase_123";
            var tagProhibido = new Tag { Nombre = "gluten" };

            var restriccion = new Restriccion
            {
                Nombre = "Sin Gluten",
                TagsProhibidos = new List<Tag> { tagProhibido }
            };

            var gustoCompatible = new Gusto
            {
                Id = Guid.NewGuid(),
                Nombre = "Sushi",
                ImagenUrl = "img/sushi.jpg",
                Tags = new List<Tag> { new Tag { Nombre = "pescado" } }
            };

            var gustoIncompatible = new Gusto
            {
                Id = Guid.NewGuid(),
                Nombre = "Pizza",
                ImagenUrl = "img/pizza.jpg",
                Tags = new List<Tag> { new Tag { Nombre = "gluten" } }
            };

            var usuario = new UsuarioFake(uid, "mail@mail.com", "Juan", "Pérez", "USR1")
            {
                Restricciones = new List<Restriccion> { restriccion },
                CondicionesMedicas = new List<CondicionMedica>()
            };

            _usuarioRepoMock.Setup(r => r.GetByFirebaseUidAsync(uid, It.IsAny<CancellationToken>()))
                            .ReturnsAsync(usuario);

            _gustoRepoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                          .ReturnsAsync(new List<Gusto> { gustoCompatible, gustoIncompatible });

            // Act
            var result = await _useCase.HandleAsync(uid, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.GustosFiltrados);
            Assert.Equal("Sushi", result.GustosFiltrados.First().Nombre);

            Assert.DoesNotContain(result.GustosFiltrados, g => g.Nombre == "Pizza");

            _usuarioRepoMock.Verify(r => r.GetByFirebaseUidAsync(uid, It.IsAny<CancellationToken>()), Times.Once);
            _gustoRepoMock.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_DeberiaLanzarExcepcionSiUsuarioNoExiste()
        {
            // Arrange
            var uid = "firebase_999";
            _usuarioRepoMock.Setup(r => r.GetByFirebaseUidAsync(uid, It.IsAny<CancellationToken>()))
                            .ReturnsAsync((Usuario?)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _useCase.HandleAsync(uid, CancellationToken.None));
        }
    }

    //  Subclase fake reutilizable
    public class UsuarioFake : Usuario
    {
        public UsuarioFake(string firebaseUid, string email, string nombre, string apellido, string idUsuario)
            : base(firebaseUid, email, nombre, apellido, idUsuario)
        {
            Gustos = new List<Gusto>();
            Restricciones = new List<Restriccion>();
            CondicionesMedicas = new List<CondicionMedica>();
        }

        public override List<string> ValidarCompatibilidad() => new List<string>();
    }
}