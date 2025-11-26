using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GustosApp.Application.Interfaces;
using GustosApp.Application.UseCases.UsuarioUseCases;



using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using Moq;
using Xunit;

public class FinalizarRegistroUseCaseTests
{
    private readonly Mock<IUsuarioRepository> _userMock;
    private readonly Mock<ICacheService> _cacheMock;
    private readonly FinalizarRegistroUseCase _useCase;
    private readonly CancellationToken _ct = CancellationToken.None;

    public FinalizarRegistroUseCaseTests()
    {
        _userMock = new Mock<IUsuarioRepository>();
        _cacheMock = new Mock<ICacheService>();

        _useCase = new FinalizarRegistroUseCase(
            _userMock.Object,
            _cacheMock.Object
        );
    }

    private Usuario CrearUsuarioBase(string uid)
    {
        return new Usuario
        {
            FirebaseUid = uid,
            Email = "test@user.com",
            Nombre = "Test",
            Apellido = "User",
            IdUsuario = "usr1",
            Gustos = new List<Gusto>(),
            Restricciones = new List<Restriccion>(),
            CondicionesMedicas = new List<CondicionMedica>(),
            RegistroInicialCompleto = false
        };
    }

    [Fact]
    public async Task HandleAsync_MenosDeTresGustos_LanzaInvalidOperationException()
    {
        // Arrange
        var uid = "uid-test";
        var usuario = CrearUsuarioBase(uid);

        usuario.Gustos.Add(new Gusto { Id = Guid.NewGuid(), Nombre = "Pizza" });

        _userMock
            .Setup(r => r.GetByFirebaseUidAsync(uid, _ct))
            .ReturnsAsync(usuario);

        // Act + Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _useCase.HandleAsync(uid, _ct));
    }

    [Fact]
    public async Task HandleAsync_GustosIncompatibles_LanzaInvalidOperationException()
    {
        // Arrange
        var uid = "uid-test";

        var tagAzucar = new Tag { Id = Guid.NewGuid(), Nombre = "Azucar" };

        var gustoPizza = new Gusto
        {
            Id = Guid.NewGuid(),
            Nombre = "Pizza",
            Tags = new List<Tag> { tagAzucar }
        };

        var usuario = CrearUsuarioBase(uid);
        usuario.Gustos.Add(gustoPizza);
        usuario.Gustos.Add(new Gusto { Id = Guid.NewGuid(), Nombre = "Pasta" });
        usuario.Gustos.Add(new Gusto { Id = Guid.NewGuid(), Nombre = "Ensalada" });

        var restriccion = new Restriccion
        {
            Id = Guid.NewGuid(),
            Nombre = "Diabetes",
            TagsProhibidos = new List<Tag> { tagAzucar }
        };

        usuario.Restricciones.Add(restriccion);

        _userMock
            .Setup(r => r.GetByFirebaseUidAsync(uid, _ct))
            .ReturnsAsync(usuario);

        // Act + Assert
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _useCase.HandleAsync(uid, _ct));

        Assert.Contains("incompatibles", ex.Message, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Pizza", ex.Message);
    }

    [Fact]
    public async Task HandleAsync_Valido_MarcaRegistroCompleto_YLimpiaCacheTemporal()
    {
        // Arrange
        var uid = "uid-test";

        var gusto1 = new Gusto { Id = Guid.NewGuid(), Nombre = "Pizza" };
        var gusto2 = new Gusto { Id = Guid.NewGuid(), Nombre = "Pasta" };
        var gusto3 = new Gusto { Id = Guid.NewGuid(), Nombre = "Ensalada" };

        var usuario = CrearUsuarioBase(uid);
 
        usuario.Gustos.Add(gusto1);
        usuario.Gustos.Add(gusto2);
        usuario.Gustos.Add(gusto3);

        // Sin restricciones ni condiciones que generen conflicto
        _userMock
            .Setup(r => r.GetByFirebaseUidAsync(uid, _ct))
            .ReturnsAsync(usuario);

        // Act
        await _useCase.HandleAsync(uid, _ct);

        // Assert
        Assert.True(usuario.RegistroInicialCompleto);
        _userMock.Verify(r => r.SaveChangesAsync(_ct), Times.Once);

        _cacheMock.Verify(c => c.DeleteAsync($"registro:{uid}:gustosTemp"), Times.Once);
        _cacheMock.Verify(c => c.DeleteAsync($"registro:{uid}:restriccionesTemp"), Times.Once);
        _cacheMock.Verify(c => c.DeleteAsync($"registro:{uid}:condicionesTemp"), Times.Once);

        _cacheMock.Verify(c => c.SetAsync(
            $"registro:{uid}:inicialCompleto",
            true,
            It.IsAny<TimeSpan>()),
            Times.Once
        );
    }

    [Fact]
    public async Task HandleAsync_RegistroYaCompleto_NoRompe_YVuelveAGuardar()
    {
        // Arrange
        var uid = "uid-test";

        var gusto1 = new Gusto { Id = Guid.NewGuid(), Nombre = "Pizza" };
        var gusto2 = new Gusto { Id = Guid.NewGuid(), Nombre = "Pasta" };
        var gusto3 = new Gusto { Id = Guid.NewGuid(), Nombre = "Ensalada" };

        var usuario = CrearUsuarioBase(uid);
      
        usuario.Gustos.Add(gusto1);
        usuario.Gustos.Add(gusto2);
        usuario.Gustos.Add(gusto3);
        usuario.RegistroInicialCompleto = true;

        _userMock
            .Setup(r => r.GetByFirebaseUidAsync(uid, _ct))
            .ReturnsAsync(usuario);

        // Act
        await _useCase.HandleAsync(uid, _ct);

        // Assert: sigue en true, se guarda y se limpia/el cache igual
        Assert.True(usuario.RegistroInicialCompleto);
        _userMock.Verify(r => r.SaveChangesAsync(_ct), Times.Once);
        _cacheMock.Verify(c => c.DeleteAsync(It.IsAny<string>()), Times.Exactly(3));
    }
}
