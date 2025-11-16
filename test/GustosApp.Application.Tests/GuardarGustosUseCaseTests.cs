using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GustosApp.Application.Interfaces;
using GustosApp.Application.UseCases.UsuarioUseCases.GustoUseCases;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using Moq;
using Xunit;

public class GuardarGustosUseCaseTests
{
    private readonly Mock<IUsuarioRepository> _userMock;
    private readonly Mock<IGustoRepository> _gustoMock;
    private readonly Mock<ICacheService> _cacheMock;
    private readonly GuardarGustosUseCase _useCase;
    private readonly CancellationToken _ct = CancellationToken.None;

    public GuardarGustosUseCaseTests()
    {
        _userMock = new Mock<IUsuarioRepository>();
        _gustoMock = new Mock<IGustoRepository>();
        _cacheMock = new Mock<ICacheService>();

        _useCase = new GuardarGustosUseCase(
            _userMock.Object,
            _gustoMock.Object,
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
    public async Task HandleAsync_BorrarTodo_LimpiaGustos_YGuardaCacheEnRegistro()
    {
        // Arrange
        var uid = "uid-test";
        var usuario = CrearUsuarioBase(uid);

        usuario.Gustos.Add(new Gusto { Id = Guid.NewGuid(), Nombre = "Pizza" });

        _userMock
            .Setup(r => r.GetByFirebaseUidAsync(uid, _ct))
            .ReturnsAsync(usuario);

        // Act
        var result = await _useCase.HandleAsync(
            uid,
            new List<Guid>(), // sin ids
            ModoPreferencias.Registro,
            _ct
        );

        // Assert
        Assert.Empty(usuario.Gustos);
        _userMock.Verify(r => r.SaveChangesAsync(_ct), Times.Once);

        _cacheMock.Verify(c => c.SetAsync(
            $"registro:{uid}:gustos",
            It.Is<List<Guid>>(l => l.Count == 0),
            It.IsAny<TimeSpan>()),
            Times.Once
        );
    }

    [Fact]
    public async Task HandleAsync_MenosDeTresGustos_LanzaArgumentException()
    {
        // Arrange
        var uid = "uid-test";
        var usuario = CrearUsuarioBase(uid);

        _userMock
            .Setup(r => r.GetByFirebaseUidAsync(uid, _ct))
            .ReturnsAsync(usuario);

        var ids = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };

        // Act + Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _useCase.HandleAsync(uid, ids, ModoPreferencias.Registro, _ct));
    }

    [Fact]
    public async Task HandleAsync_IdsInvalidos_LanzaArgumentException()
    {
        // Arrange
        var uid = "uid-test";
        var usuario = CrearUsuarioBase(uid);

        _userMock
            .Setup(r => r.GetByFirebaseUidAsync(uid, _ct))
            .ReturnsAsync(usuario);

        var ids = new List<Guid> { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };

        // Devuelvo solo 2 gustos en lugar de 3
        _gustoMock
            .Setup(r => r.GetByIdsAsync(ids, _ct))
            .ReturnsAsync(new List<Gusto>
            {
                new Gusto { Id = ids[0], Nombre = "Pizza" },
                new Gusto { Id = ids[1], Nombre = "Pasta" }
            });

        // Act + Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _useCase.HandleAsync(uid, ids, ModoPreferencias.Registro, _ct));
    }

    [Fact]
    public async Task HandleAsync_SinCambios_ModoRegistro_SoloCache()
    {
        // Arrange
        var uid = "uid-test";

        var g1Id = Guid.NewGuid();
        var g2Id = Guid.NewGuid();
        var g3Id = Guid.NewGuid();

        var usuario = CrearUsuarioBase(uid);

        usuario.Gustos = new List<Gusto>
        {
            new Gusto { Id = g1Id, Nombre = "Pizza" },
            new Gusto { Id = g2Id, Nombre = "Pasta" },
            new Gusto { Id = g3Id, Nombre = "Ensalada" }
        };

        var ids = new List<Guid> { g1Id, g2Id, g3Id };

        _userMock
            .Setup(r => r.GetByFirebaseUidAsync(uid, _ct))
            .ReturnsAsync(usuario);

        _gustoMock
            .Setup(r => r.GetByIdsAsync(ids, _ct))
            .ReturnsAsync(usuario.Gustos.ToList());

        // Act
        var result = await _useCase.HandleAsync(
            uid,
            ids,
            ModoPreferencias.Registro,
            _ct
        );

        // Assert
        Assert.Empty(result);
        _userMock.Verify(r => r.SaveChangesAsync(_ct), Times.Never);

        _cacheMock.Verify(c => c.SetAsync(
            $"registro:{uid}:gustos",
            It.Is<List<Guid>>(l => l.Count == 3 && l.Contains(g1Id) && l.Contains(g2Id) && l.Contains(g3Id)),
            It.IsAny<TimeSpan>()),
            Times.Once
        );
    }

    [Fact]
    public async Task HandleAsync_Cambios_AgregaYQuitaCorrectamente()
    {
        // Arrange
        var uid = "uid-test";

        var g1Id = Guid.NewGuid(); // queda
        var g2Id = Guid.NewGuid(); // se quita
        var g3Id = Guid.NewGuid(); // entra (para que haya al menos 3)
        var g4Id = Guid.NewGuid(); // entra

        var usuario = CrearUsuarioBase(uid);

        usuario.Gustos = new List<Gusto>
        {
            new Gusto { Id = g1Id, Nombre = "Pizza" },
            new Gusto { Id = g2Id, Nombre = "Pasta" }
        };

        var ids = new List<Guid> { g1Id, g3Id, g4Id };

        var g1 = usuario.Gustos.First(x => x.Id == g1Id);
        var g3 = new Gusto { Id = g3Id, Nombre = "Ensalada" };
        var g4 = new Gusto { Id = g4Id, Nombre = "Hamburguesa" };

        _userMock
            .Setup(r => r.GetByFirebaseUidAsync(uid, _ct))
            .ReturnsAsync(usuario);

        _gustoMock
            .Setup(r => r.GetByIdsAsync(ids, _ct))
            .ReturnsAsync(new List<Gusto> { g1, g3, g4 });

        // Act
        var result = await _useCase.HandleAsync(
            uid,
            ids,
            ModoPreferencias.Registro,
            _ct
        );

        // Assert
        _userMock.Verify(r => r.SaveChangesAsync(_ct), Times.Once);

        Assert.Contains(usuario.Gustos, g => g.Id == g1Id);
        Assert.Contains(usuario.Gustos, g => g.Id == g3Id);
        Assert.Contains(usuario.Gustos, g => g.Id == g4Id);
        Assert.DoesNotContain(usuario.Gustos, g => g.Id == g2Id);
    }

    [Fact]
    public async Task HandleAsync_DevuelveGustosIncompatibles()
    {
        // Arrange
        var uid = "uid-test";

        var tagAzucar = new Tag { Id = Guid.NewGuid(), Nombre = "Azucar" };

        var g1 = new Gusto
        {
            Id = Guid.NewGuid(),
            Nombre = "Pizza",
            Tags = new List<Tag> { tagAzucar } // incompatible
        };

        var g2 = new Gusto
        {
            Id = Guid.NewGuid(),
            Nombre = "Ensalada",
            Tags = new List<Tag>() // ok
        };

        var g3 = new Gusto
        {
            Id = Guid.NewGuid(),
            Nombre = "Pasta",
            Tags = new List<Tag>() // ok
        };

        var usuario = CrearUsuarioBase(uid);

        // Usuario inicialmente con solo 2 gustos → para que haya cambios al agregar uno
        usuario.Gustos.Add(g2); // Ensalada
        usuario.Gustos.Add(g3); // Pasta

        var restriccionDiabetes = new Restriccion
        {
            Id = Guid.NewGuid(),
            Nombre = "Diabetes",
            TagsProhibidos = new List<Tag> { tagAzucar }
        };

        usuario.Restricciones.Add(restriccionDiabetes);

        // Vamos a guardar los 3 gustos (uno incompatible)
        var ids = new List<Guid> { g1.Id, g2.Id, g3.Id };

        _userMock
            .Setup(r => r.GetByFirebaseUidAsync(uid, _ct))
            .ReturnsAsync(usuario);

        _gustoMock
            .Setup(r => r.GetByIdsAsync(ids, _ct))
            .ReturnsAsync(new List<Gusto> { g1, g2, g3 });

        // Act
        var result = await _useCase.HandleAsync(
            uid,
            ids,
            ModoPreferencias.Registro,
            _ct
        );

        // Assert: Pizza debe ser incompatible y removida
        Assert.Contains("Pizza", result);
        Assert.DoesNotContain(usuario.Gustos, g => g.Nombre == "Pizza");
    }
}
