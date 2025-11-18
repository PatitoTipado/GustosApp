using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GustosApp.Application.Interfaces;
using GustosApp.Application.UseCases.UsuarioUseCases.CondicionesMedicasUseCases;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using GustosApp.Domain.Model.@enum;
using Moq;
using Xunit;

public class GuardarCondicionesUseCaseTests
{
    private readonly Mock<IUsuarioRepository> _userMock;
    private readonly Mock<ICondicionMedicaRepository> _condicionesMock;
    private readonly Mock<ICacheService> _cacheMock;
    private readonly GuardarCondicionesUseCase _useCase;
    private readonly CancellationToken _ct = CancellationToken.None;

    public GuardarCondicionesUseCaseTests()
    {
        _userMock = new Mock<IUsuarioRepository>();
        _condicionesMock = new Mock<ICondicionMedicaRepository>();
        _cacheMock = new Mock<ICacheService>();

        _useCase = new GuardarCondicionesUseCase(
            _userMock.Object,
            _condicionesMock.Object,
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
    public async Task HandleAsync_SkipTrue_LimpiaCondiciones_YGuardaCacheEnRegistro()
    {
        // Arrange
        var uid = "uid-test";
        var usuario = CrearUsuarioBase(uid);

        usuario.CondicionesMedicas.Add(new CondicionMedica { Id = Guid.NewGuid(), Nombre = "Diabetes" });

        _userMock
            .Setup(r => r.GetByFirebaseUidAsync(uid, _ct))
            .ReturnsAsync(usuario);

        // Act
        var result = await _useCase.HandleAsync(
            uid,
            new List<Guid> { Guid.NewGuid() },
            skip: true,
            modo: ModoPreferencias.Registro,
            ct: _ct
        );

        // Assert
        Assert.Empty(usuario.CondicionesMedicas);
        _userMock.Verify(r => r.SaveChangesAsync(_ct), Times.Once);

        _cacheMock.Verify(c => c.SetAsync(
            $"registro:{uid}:condiciones",
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

        _condicionesMock
            .Setup(r => r.GetByIdsAsync(ids, _ct))
            .ReturnsAsync(new List<CondicionMedica>
            {
                new CondicionMedica { Id = ids[0], Nombre = "Diabetes" }
            });

        // Act + Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _useCase.HandleAsync(uid, ids, skip: false, modo: ModoPreferencias.Registro, ct: _ct));
    }

    [Fact]
    public async Task HandleAsync_SinCambios_ModoRegistro_SoloCache()
    {
        // Arrange
        var uid = "uid-test";
        var c1Id = Guid.NewGuid();
        var c2Id = Guid.NewGuid();

        var usuario = CrearUsuarioBase(uid);
        usuario.CondicionesMedicas = new List<CondicionMedica>
        {
            new CondicionMedica { Id = c1Id, Nombre = "Diabetes" },
            new CondicionMedica { Id = c2Id, Nombre = "Hipertensión" }
        };

        var ids = new List<Guid> { c1Id, c2Id };

        _userMock
            .Setup(r => r.GetByFirebaseUidAsync(uid, _ct))
            .ReturnsAsync(usuario);

        _condicionesMock
            .Setup(r => r.GetByIdsAsync(ids, _ct))
            .ReturnsAsync(usuario.CondicionesMedicas.ToList());

        // Act
        var result = await _useCase.HandleAsync(
            uid,
            ids,
            skip: false,
            modo: ModoPreferencias.Registro,
            ct: _ct
        );

        // Assert
        Assert.Empty(result);
        _userMock.Verify(r => r.SaveChangesAsync(_ct), Times.Never);

        _cacheMock.Verify(c => c.SetAsync(
            $"registro:{uid}:condiciones",
            It.Is<List<Guid>>(l => l.Count == 2 && l.Contains(c1Id) && l.Contains(c2Id)),
            It.IsAny<TimeSpan>()),
            Times.Once
        );
    }

    [Fact]
    public async Task HandleAsync_Cambios_AgregaYQuitaCorrectamente()
    {
        // Arrange
        var uid = "uid-test";
        var c1Id = Guid.NewGuid(); // queda
        var c2Id = Guid.NewGuid(); // se va
        var c3Id = Guid.NewGuid(); // entra

        var usuario = CrearUsuarioBase(uid);
        usuario.CondicionesMedicas = new List<CondicionMedica>
        {
            new CondicionMedica { Id = c1Id, Nombre = "Diabetes" },
            new CondicionMedica { Id = c2Id, Nombre = "Hipertensión" }
        };

        var ids = new List<Guid> { c1Id, c3Id };

        var c1 = usuario.CondicionesMedicas.First(x => x.Id == c1Id);
        var c3 = new CondicionMedica { Id = c3Id, Nombre = "Enfermedad celíaca" };

        _userMock
            .Setup(r => r.GetByFirebaseUidAsync(uid, _ct))
            .ReturnsAsync(usuario);

        _condicionesMock
            .Setup(r => r.GetByIdsAsync(ids, _ct))
            .ReturnsAsync(new List<CondicionMedica> { c1, c3 });

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

        Assert.Contains(usuario.CondicionesMedicas, c => c.Id == c1Id);
        Assert.Contains(usuario.CondicionesMedicas, c => c.Id == c3Id);
        Assert.DoesNotContain(usuario.CondicionesMedicas, c => c.Id == c2Id);
    }

    [Fact]
    public async Task HandleAsync_DevuelveGustosIncompatibles()
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

        var condicionDiabetes = new CondicionMedica
        {
            Id = Guid.NewGuid(),
            Nombre = "Diabetes",
            TagsCriticos = new List<Tag> { tagAzucar }
        };

        var ids = new List<Guid> { condicionDiabetes.Id };

        _userMock
            .Setup(r => r.GetByFirebaseUidAsync(uid, _ct))
            .ReturnsAsync(usuario);

        _condicionesMock
            .Setup(r => r.GetByIdsAsync(ids, _ct))
            .ReturnsAsync(new List<CondicionMedica> { condicionDiabetes });

        // Act
        var result = await _useCase.HandleAsync(
            uid,
            ids,
            skip: false,
            modo: ModoPreferencias.Registro,
            ct: _ct
        );

        // Assert
        Assert.Contains("Pizza", result);
        Assert.DoesNotContain(usuario.Gustos, g => g.Nombre == "Pizza");
    }
}
