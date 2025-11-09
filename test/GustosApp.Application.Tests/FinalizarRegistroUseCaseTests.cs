using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GustosApp.Application.UseCases.UsuarioUseCases;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using Moq;

namespace GustosApp.Application.Tests
{
    public class FinalizarRegistroUseCaseTests
    {
        private readonly Mock<IUsuarioRepository> _usuarioRepoMock;
        private readonly FinalizarRegistroUseCase _useCase;


        public FinalizarRegistroUseCaseTests()
        {
            _usuarioRepoMock = new Mock<IUsuarioRepository>();
            _useCase = new FinalizarRegistroUseCase(_usuarioRepoMock.Object);
        }
        [Fact]
        public async Task HandleAsync_DeberiaLanzarExcepcionSiUsuarioNoExiste()
        {
            // Arrange
            var uid = "uid_inexistente";
            var ct = CancellationToken.None;


            _usuarioRepoMock.Setup(r => r.GetByFirebaseUidAsync(uid, ct))
                    .ReturnsAsync((Usuario?)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _useCase.HandleAsync(uid, ct));
        }


        [Fact]
        public async Task HandleAsync_DeberiaFallarSiUsuarioNoTieneGustos()
        {
            // Arrange
            var uid = "firebase_123";
            var usuario = new Usuario(uid, "mail@mail.com", "Juan", "Pérez", "USR1");


            _usuarioRepoMock.Setup(r => r.GetByFirebaseUidAsync(uid, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(usuario);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _useCase.HandleAsync(uid, CancellationToken.None));
            Assert.Contains("Debe seleccionar al menos un gusto", ex.Message);
        }

        [Fact]
        public async Task HandleAsync_DeberiaFinalizarSiTodoEsCompatible()
        {
            // Arrange
            var uid = "firebase_ok";
            var ct = CancellationToken.None;

            // Tags
            var tagFruta = new Tag { Nombre = "Fruta" };
            var tagVegetal = new Tag { Nombre = "Vegetal" };

            // Gustos (todos aptos)
            var gusto1 = new Gusto { Nombre = "Ensalada", Tags = new List<Tag> { tagVegetal } };
            var gusto2 = new Gusto { Nombre = "Ensalada de frutas", Tags = new List<Tag> { tagFruta } };

            var usuario = new Usuario(uid, "ok@mail.com", "Ana", "García", "USR2");

            usuario.Gustos = new List<Gusto> { gusto1, gusto2 };

            _usuarioRepoMock.Setup(r => r.GetByFirebaseUidAsync(uid, ct)).ReturnsAsync(usuario);
            _usuarioRepoMock.Setup(r => r.SaveChangesAsync(ct)).Returns(Task.CompletedTask);

            // Act
            await _useCase.HandleAsync(uid, ct);

            // Assert
            _usuarioRepoMock.Verify(r => r.SaveChangesAsync(ct), Times.Once);
            Assert.Equal(RegistroPaso.Finalizado, usuario.PasoActual);
        }


        [Fact]
        public async Task HandleAsync_DeberiaFallarSiHayGustoIncompatibleConRestricciones()
        {
            // Arrange
            var uid = "firebase_restriccion";
            var ct = CancellationToken.None;

            var tagGluten = new Tag { Nombre = "Gluten" };
            var gustoPizza = new Gusto { Nombre = "Pizza", Tags = new List<Tag> { tagGluten } };

            var restriccion = new Restriccion
            {
                Nombre = "Sin gluten",
                TagsProhibidos = new List<Tag> { tagGluten }
            };

            var usuario = new Usuario(uid, "mail@mail.com", "Pedro", "López", "USR3");
            usuario.Gustos.Add(gustoPizza);
            usuario.Restricciones.Add(restriccion);

            _usuarioRepoMock.Setup(r => r.GetByFirebaseUidAsync(uid, ct)).ReturnsAsync(usuario);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _useCase.HandleAsync(uid, ct));
            Assert.Contains("incompatibles con tus restricciones", ex.Message);
        }

        [Fact]
        public async Task HandleAsync_DeberiaFallarSiHayGustoIncompatibleConCondicionesMedicas()
        {
            // Arrange
            var uid = "firebase_condicion";
            var ct = CancellationToken.None;

            var tagAzucar = new Tag { Nombre = "Azúcar" };
            var gustoHelado = new Gusto { Nombre = "Helado", Tags = new List<Tag> { tagAzucar } };

            var diabetes = new CondicionMedica
            {
                Nombre = "Diabetes",
                TagsCriticos = new List<Tag> { tagAzucar }
            };

            var usuario = new Usuario(uid, "mail@mail.com", "Lucía", "Martínez", "USR4");
            usuario.Gustos.Add(gustoHelado);
            usuario.CondicionesMedicas.Add(diabetes);

            _usuarioRepoMock.Setup(r => r.GetByFirebaseUidAsync(uid, ct)).ReturnsAsync(usuario);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _useCase.HandleAsync(uid, ct));
            Assert.Contains("incompatibles", ex.Message);
            Assert.Contains("Helado", ex.Message);
        }
        [Fact]
        public async Task HandleAsync_DeberiaDetectarMultiplesConflictos()
        {
            // Arrange
            var uid = "firebase_multi";
            var ct = CancellationToken.None;

            var tagGluten = new Tag { Nombre = "Gluten" };
            var tagAzucar = new Tag { Nombre = "Azúcar" };

            var gustoPizza = new Gusto { Nombre = "Pizza", Tags = new List<Tag> { tagGluten } };
            var gustoHelado = new Gusto { Nombre = "Helado", Tags = new List<Tag> { tagAzucar } };

            var sinGluten = new Restriccion { Nombre = "Sin gluten", TagsProhibidos = new List<Tag> { tagGluten } };
            var diabetes = new CondicionMedica { Nombre = "Diabetes", TagsCriticos = new List<Tag> { tagAzucar } };

            var usuario = new Usuario(uid, "mail@mail.com", "María", "Santos", "USR5");

            usuario.Gustos = new List<Gusto> { gustoPizza, gustoHelado };

            usuario.Restricciones.Add(sinGluten);
            usuario.CondicionesMedicas.Add(diabetes);

            _usuarioRepoMock.Setup(r => r.GetByFirebaseUidAsync(uid, ct)).ReturnsAsync(usuario);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _useCase.HandleAsync(uid, ct));
            Assert.Contains("Pizza", ex.Message);
            Assert.Contains("Helado", ex.Message);
        }

    }
}
