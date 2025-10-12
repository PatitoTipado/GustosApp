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

public class GuardarCondicionesUseCaseTests
{
    private readonly Mock<IUsuarioRepository> _usuarioRepoMock;
    private readonly Mock<ICondicionMedicaRepository> _condicionRepoMock;
    private readonly GuardarCondicionesUseCase _useCase;

    public GuardarCondicionesUseCaseTests()
    {
        _usuarioRepoMock = new Mock<IUsuarioRepository>();
        _condicionRepoMock = new Mock<ICondicionMedicaRepository>();
        _useCase = new GuardarCondicionesUseCase(_condicionRepoMock.Object, _usuarioRepoMock.Object);
    }

    [Fact]
    public async Task HandleAsync_Should_Save_Condiciones_And_Remove_Incompatibles_Gustos()
    {
        // Arrange
        var uid = "firebase_123";
        var ids = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        var ct = CancellationToken.None;

        var usuario = new Usuario(uid, "user@mail.com", "Juan", "Pérez", "USR1")
        {
            Gustos = new List<Gusto> { new Gusto { Id = Guid.NewGuid(), Nombre = "Pizza" } }
        };

        _usuarioRepoMock.Setup(r => r.GetByFirebaseUidAsync(uid, ct))
            .ReturnsAsync(usuario);

        var condiciones = new List<CondicionMedica>
        {
            new CondicionMedica { Id = ids[0], Nombre = "Diabetes" },
            new CondicionMedica { Id = ids[1], Nombre = "Hipertensión" }
        };

        _condicionRepoMock.Setup(r => r.GetByIdsAsync(ids, ct))
            .ReturnsAsync(condiciones);

        _usuarioRepoMock.Setup(r => r.SaveChangesAsync(ct))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _useCase.HandleAsync(uid, ids, skip: false, ct);

        // Assert
        _usuarioRepoMock.Verify(r => r.SaveChangesAsync(ct), Times.Once);
        Assert.Equal(RegistroPaso.Gustos, usuario.PasoActual);
        Assert.Equal(2, usuario.CondicionesMedicas.Count);
        Assert.Equal("Condiciones médicas actualizadas correctamente.", result.mensaje);
    }

    [Fact]
    public async Task HandleAsync_Should_Skip_When_Skip_True()
    {
        // Arrange
        var uid = "firebase_123";
        var ct = CancellationToken.None;

        // Act
        var result = await _useCase.HandleAsync(uid, new List<Guid>(), skip: true, ct);

        // Assert
        Assert.Equal("Paso omitido", result.mensaje);
        _usuarioRepoMock.Verify(r => r.SaveChangesAsync(ct), Times.Never);
    }

    [Fact]
    public async Task HandleAsync_Should_Throw_When_User_Not_Found()
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
