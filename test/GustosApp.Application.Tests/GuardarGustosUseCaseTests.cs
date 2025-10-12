using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;
using GustosApp.Application.UseCases;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;


    public class UsuarioFake : Usuario
    {
        public UsuarioFake(string firebaseUid, string email, string nombre, string apellido, string idUsuario)
            : base(firebaseUid, email, nombre, apellido, idUsuario)
        {
            Gustos = new List<Gusto>();
            Restricciones = new List<Restriccion>();
            CondicionesMedicas = new List<CondicionMedica>();
        }

        public override List<string> ValidarCompatibilidad()
        {
            return new List<string> { "Pizza" };
        }
    }

  
    public class GuardarGustosUseCaseTests
    {
        private readonly Mock<IUsuarioRepository> _usuarioRepoMock;
        private readonly Mock<IGustoRepository> _gustoRepoMock;
        private readonly GuardarGustosUseCase _useCase;

        public GuardarGustosUseCaseTests()
        {
            _usuarioRepoMock = new Mock<IUsuarioRepository>();
            _gustoRepoMock = new Mock<IGustoRepository>();
            _useCase = new GuardarGustosUseCase(_usuarioRepoMock.Object, _gustoRepoMock.Object);
        }

        [Fact]
        public async Task HandleAsync_Should_Save_Gustos_And_Advance_Paso()
        {
            // Arrange
            var uid = "firebase_123";
            var ids = new List<Guid> { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
            var ct = CancellationToken.None;

            var usuario = new Usuario(uid, "test@mail.com", "Juan", "Pérez", "USR1");

            _usuarioRepoMock.Setup(r => r.GetByFirebaseUidAsync(uid, ct))
                .ReturnsAsync(usuario);

            var gustos = ids.Select(id => new Gusto { Id = id, Nombre = $"Gusto {id}" }).ToList();

            _gustoRepoMock.Setup(r => r.GetByIdsAsync(ids, ct))
                .ReturnsAsync(gustos);

            _usuarioRepoMock.Setup(r => r.SaveChangesAsync(ct))
                .Returns(Task.CompletedTask);

            // Act
            var conflictos = await _useCase.HandleAsync(uid, ids, ct);

            // Assert
            _usuarioRepoMock.Verify(r => r.SaveChangesAsync(ct), Times.Once);
            Assert.Equal(RegistroPaso.Verificacion, usuario.PasoActual);
            Assert.Equal(3, usuario.Gustos.Count);
            Assert.Empty(conflictos);
        }

        [Fact]
        public async Task HandleAsync_Should_Throw_When_Less_Than_Three_Gustos()
        {
            // Arrange
            var uid = "firebase_123";
            var ids = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() }; // solo 2 gustos
            var ct = CancellationToken.None;

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
                _useCase.HandleAsync(uid, ids, ct));

            Assert.Contains("al menos 3 gustos", ex.Message);
        }

        [Fact]
        public async Task HandleAsync_Should_Throw_When_User_Not_Found()
        {
            // Arrange
            var uid = "firebase_999";
            var ids = new List<Guid> { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
            var ct = CancellationToken.None;

            _usuarioRepoMock.Setup(r => r.GetByFirebaseUidAsync(uid, ct))
                .ReturnsAsync((Usuario?)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() =>
                _useCase.HandleAsync(uid, ids, ct));
        }

        [Fact]
    public async Task HandleAsync_DeberiaGuardarNuevosGustosYValidarCompatibilidad()
    {
        // Arrange
        var uid = "firebase_123";
        var ids = new List<Guid> { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };

        var gustoIncompatible = new Gusto { Id = ids[0], Nombre = "Pizza" };
        var gustoCompatible1 = new Gusto { Id = ids[1], Nombre = "Sushi" };
        var gustoCompatible2 = new Gusto { Id = ids[2], Nombre = "Tacos" };
        var gustoCompatible3 = new Gusto { Id = ids[3], Nombre = "Pasta" };

        var usuario = new UsuarioFake(uid, "user@mail.com", "Ana", "García", "USR2")
        {
            Gustos = new List<Gusto> { gustoCompatible1 }
        };

        _usuarioRepoMock.Setup(r => r.GetByFirebaseUidAsync(uid, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(usuario);

        _gustoRepoMock.Setup(r => r.GetByIdsAsync((List<Guid>)It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync(new List<Gusto> { gustoIncompatible, gustoCompatible1, gustoCompatible2, gustoCompatible3 });

        _usuarioRepoMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                        .Returns(Task.CompletedTask);

        // Act
        var conflictos = await _useCase.HandleAsync(uid, ids, CancellationToken.None);

        // Assert
        Assert.NotNull(conflictos);
        Assert.Contains("Pizza", conflictos);
        Assert.Equal(RegistroPaso.Verificacion, usuario.PasoActual);

        _usuarioRepoMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
    }

