using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GustosApp.Application.Interfaces;
using GustosApp.Application.UseCases.UsuarioUseCases.RestriccionesUseCases;

using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using Moq;
using Xunit;

public class GuardarRestriccionesUseCaseTests
{
    private readonly Mock<IUsuarioRepository> _userMock;
    private readonly Mock<IRestriccionRepository> _restriccionMock;
    private readonly Mock<ICacheService> _cacheMock;
    private readonly GuardarRestriccionesUseCase _useCase;
    private readonly CancellationToken _ct = CancellationToken.None;

    public GuardarRestriccionesUseCaseTests()
    {
        _userMock = new Mock<IUsuarioRepository>();
        _restriccionMock = new Mock<IRestriccionRepository>();
        _cacheMock = new Mock<ICacheService>();

        _useCase = new GuardarRestriccionesUseCase(
            _userMock.Object,
            _restriccionMock.Object,
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
            CondicionesMedicas = new List<CondicionMedica>()
        };
    }

    [Fact]
    public async Task HandleAsync_SkipTrue_LimpiaRestricciones_YGuardaCacheEnRegistro()
    {
        // Arrange
        var uid = "uid-test";
        var usuario = CrearUsuarioBase(uid);

        usuario.Restricciones.Add(new Restriccion { Id = Guid.NewGuid(), Nombre = "Sin gluten" });

        _userMock
            .Setup(r => r.GetByFirebaseUidAsync(uid, _ct))
            .ReturnsAsync(usuario);

        // Act
        var result = await _useCase.HandleAsync(
            uid,
            new List<Guid> { Guid.NewGuid() }, // no importa, porque skip = true
            skip: true,
            modo: ModoPreferencias.Registro,
            ct: _ct
        );

        // Assert
        Assert.Empty(usuario.Restricciones);          // se limpiaron
        _userMock.Verify(r => r.SaveChangesAsync(_ct), Times.Once);

        // cache de registro actualizado con lista vacía
        _cacheMock.Verify(c => c.SetAsync(
            $"registro:{uid}:restricciones",
            It.Is<List<Guid>>(l => l.Count == 0),
            It.IsAny<TimeSpan>()),
            Times.Once
        );
    }

    [Fact]
    public async Task HandleAsync_IdsInvalidos_LanzaArgumentException()
    {
        // Arrange
        var uid = "uid-test";
        var usuario = CrearUsuarioBase(uid);

        var ids = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };

        _userMock
            .Setup(r => r.GetByFirebaseUidAsync(uid, _ct))
            .ReturnsAsync(usuario);

        // Devuelvo menos entidades de las pedidas → mismatch
        _restriccionMock
            .Setup(r => r.GetRestriccionesByIdsAsync(ids, _ct))
            .ReturnsAsync(new List<Restriccion>
            {
                new Restriccion { Id = ids[0], Nombre = "Sin gluten" }
            });

        // Act + Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _useCase.HandleAsync(uid, ids, skip: false, modo: ModoPreferencias.Registro, ct: _ct));
    }

    [Fact]
    public async Task HandleAsync_SinCambios_ModoRegistro_SoloCache_NoSaveChanges()
    {
        // Arrange
        var uid = "uid-test";
        var r1Id = Guid.NewGuid();
        var r2Id = Guid.NewGuid();

        var usuario = CrearUsuarioBase(uid);
        usuario.Restricciones = new List<Restriccion>
        {
            new Restriccion { Id = r1Id, Nombre = "Sin gluten" },
            new Restriccion { Id = r2Id, Nombre = "Sin lactosa" }
        };

        var ids = new List<Guid> { r1Id, r2Id }; // exactamente las mismas

        _userMock
            .Setup(r => r.GetByFirebaseUidAsync(uid, _ct))
            .ReturnsAsync(usuario);

        _restriccionMock
            .Setup(r => r.GetRestriccionesByIdsAsync(ids, _ct))
            .ReturnsAsync(usuario.Restricciones.ToList());

        // Act
        var result = await _useCase.HandleAsync(
            uid,
            ids,
            skip: false,
            modo: ModoPreferencias.Registro,
            ct: _ct
        );

        // Assert: sin cambios → lista vacía
        Assert.Empty(result);

        // No se llamó a SaveChanges
        _userMock.Verify(r => r.SaveChangesAsync(_ct), Times.Never);

        // Pero sí se cacheó en registro
        _cacheMock.Verify(c => c.SetAsync(
            $"registro:{uid}:restricciones",
            It.Is<List<Guid>>(l => l.Count == 2 && l.Contains(r1Id) && l.Contains(r2Id)),
            It.IsAny<TimeSpan>()),
            Times.Once
        );
    }

    [Fact]
    public async Task HandleAsync_AgregaYQuita_Correctamente()
    {
        // Arrange
        var uid = "uid-test";
        var r1Id = Guid.NewGuid(); // ya está
        var r2Id = Guid.NewGuid(); // se quita
        var r3Id = Guid.NewGuid(); // se agrega

        var usuario = CrearUsuarioBase(uid);

        usuario.Restricciones = new List<Restriccion>
        {
            new Restriccion { Id = r1Id, Nombre = "Sin gluten" },
            new Restriccion { Id = r2Id, Nombre = "Sin lactosa" }
        };

        var ids = new List<Guid> { r1Id, r3Id }; // r2 sale, r3 entra

        var r1 = usuario.Restricciones.First(x => x.Id == r1Id);
        var r3 = new Restriccion { Id = r3Id, Nombre = "Sin sal" };

        _userMock
            .Setup(r => r.GetByFirebaseUidAsync(uid, _ct))
            .ReturnsAsync(usuario);

        _restriccionMock
            .Setup(r => r.GetRestriccionesByIdsAsync(ids, _ct))
            .ReturnsAsync(new List<Restriccion> { r1, r3 });

        // Act
        var result = await _useCase.HandleAsync(
            uid,
            ids,
            skip: false,
            modo: ModoPreferencias.Registro,
            ct: _ct
        );

        // Assert
        _userMock.Verify(r => r.SaveChangesAsync(_ct), Times.Once);

        Assert.Contains(usuario.Restricciones, r => r.Id == r1Id);
        Assert.Contains(usuario.Restricciones, r => r.Id == r3Id);
        Assert.DoesNotContain(usuario.Restricciones, r => r.Id == r2Id);
    }

    [Fact]
    public async Task HandleAsync_Cambios_DevuelveGustosIncompatibles()
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

        var restriccionDiabetes = new Restriccion
        {
            Id = Guid.NewGuid(),
            Nombre = "Diabetes",
            TagsProhibidos = new List<Tag> { tagAzucar }
        };

        var ids = new List<Guid> { restriccionDiabetes.Id };

        _userMock
            .Setup(r => r.GetByFirebaseUidAsync(uid, _ct))
            .ReturnsAsync(usuario);

        _restriccionMock
            .Setup(r => r.GetRestriccionesByIdsAsync(ids, _ct))
            .ReturnsAsync(new List<Restriccion> { restriccionDiabetes });

        // Act
        var result = await _useCase.HandleAsync(
            uid,
            ids,
            skip: false,
            modo: ModoPreferencias.Registro,
            ct: _ct
        );

        // Assert: Pizza debe ser incompatible y removida
        Assert.Contains("Pizza", result);
        Assert.DoesNotContain(usuario.Gustos, g => g.Nombre == "Pizza");
    }
}
