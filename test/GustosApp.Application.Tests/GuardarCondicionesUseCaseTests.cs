using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using GustosApp.Application.Tests.mocks;
using GustosApp.Application.UseCases.UsuarioUseCases.CondicionesMedcasUseCases;



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
    public async Task HandleAsync_DeberiaGuardarRestriccionesYValidarCompatibilidadGustos()
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

        // Asserts
        _usuarioRepoMock.Verify(r => r.SaveChangesAsync(ct), Times.Once);
        Assert.Equal(RegistroPaso.Gustos, usuario.PasoActual);
        Assert.Equal(2, usuario.CondicionesMedicas.Count);
       
    }

    [Fact]
    public async Task HandleAsync_DeberiaQuitarCondicionesDesmarcadas()
    {
        // Arrange
        var uid = "firebase_123";
        var ct = CancellationToken.None;

        // Condiciones en la BD (todas las posibles)
        var diabetes = new CondicionMedica { Id = Guid.NewGuid(), Nombre = "Diabetes" };
        var hipertension = new CondicionMedica { Id = Guid.NewGuid(), Nombre = "Hipertensión" };
        var obesidad = new CondicionMedica { Id = Guid.NewGuid(), Nombre = "Obesidad" };

        // Usuario tenía 3 condiciones guardadas
        var usuario = new UsuarioFake(uid, "user@mail.com", "Juan", "Pérez", "USR1")
        {
            CondicionesMedicas = new List<CondicionMedica> { diabetes, hipertension, obesidad }
        };

        // Pero ahora solo seleccionó 2 (quitó Obesidad)
        var nuevosIds = new List<Guid> { diabetes.Id, hipertension.Id };

        _usuarioRepoMock
            .Setup(r => r.GetByFirebaseUidAsync(uid, ct))
            .ReturnsAsync(usuario);

        _condicionRepoMock
            .Setup(r => r.GetByIdsAsync(It.IsAny<List<Guid>>(), ct))
            .ReturnsAsync(new List<CondicionMedica> { diabetes, hipertension });

        _usuarioRepoMock
            .Setup(r => r.SaveChangesAsync(ct))
            .Returns(Task.CompletedTask);

        var useCase = new GuardarCondicionesUseCase(_condicionRepoMock.Object, _usuarioRepoMock.Object);

        // Act
        var result = await useCase.HandleAsync(uid, nuevosIds, skip: false, ct);

        // Assert
        Assert.Equal(2, usuario.CondicionesMedicas.Count);
        Assert.DoesNotContain(usuario.CondicionesMedicas, c => c.Nombre == "Obesidad");
        
        Assert.Equal(RegistroPaso.Gustos, usuario.PasoActual);

        _usuarioRepoMock.Verify(r => r.SaveChangesAsync(ct), Times.Once);
    }
    [Fact]
    public async Task HandleAsync_DeberiaAgregarCondicionMedicaSiHayNuevoIdListGuid()
    {
        // Arrange
        var uid = "firebase_123";
        var ct = CancellationToken.None;


        var diabetes = new CondicionMedica { Id = Guid.NewGuid(), Nombre = "Diabetes" };
        var hipertension = new CondicionMedica { Id = Guid.NewGuid(), Nombre = "Hipertensión" };
        var obesidad = new CondicionMedica { Id = Guid.NewGuid(), Nombre = "Obesidad" };

        var sarampion = new CondicionMedica { Id = Guid.NewGuid(), Nombre = "Sarampion" };

        // Usuario tenía 3 condiciones guardadas
        var usuario = new UsuarioFake(uid, "user@mail.com", "Juan", "Pérez", "USR1")
        {
            CondicionesMedicas = new List<CondicionMedica> { diabetes, hipertension, obesidad }
        };

        // Pero ahora solo seleccionó 4 (Agregó sarampión)
        var nuevosIds = new List<Guid> { diabetes.Id, hipertension.Id,obesidad.Id,sarampion.Id };

        _usuarioRepoMock
            .Setup(r => r.GetByFirebaseUidAsync(uid, ct))
            .ReturnsAsync(usuario);


        _condicionRepoMock
            .Setup(r => r.GetByIdsAsync(It.IsAny<List<Guid>>(), ct))
            .ReturnsAsync((List<Guid> ids, CancellationToken _) =>
                new List<CondicionMedica> { diabetes, hipertension, obesidad, sarampion }
                    .Where(c => ids.Contains(c.Id))
                    .ToList());


        _usuarioRepoMock
            .Setup(r => r.SaveChangesAsync(ct))
            .Returns(Task.CompletedTask);

        var useCase = new GuardarCondicionesUseCase(_condicionRepoMock.Object, _usuarioRepoMock.Object);

        // Act
        var result = await useCase.HandleAsync(uid, nuevosIds, skip: false, ct);

        // Assert
        Assert.Equal(4, usuario.CondicionesMedicas.Count);
        Assert.Contains(usuario.CondicionesMedicas, c => c.Nombre == "Sarampion");
       
        Assert.Equal(RegistroPaso.Gustos, usuario.PasoActual);

        _usuarioRepoMock.Verify(r => r.SaveChangesAsync(ct), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_DeberiaQuitarTodasLasCondicionesSiQuedanDesmarcadasTodas()
    {
        // Arrange
        var uid = "firebase_123";
        var ct = CancellationToken.None;

        // Condiciones en la BD (todas las posibles)
        var diabetes = new CondicionMedica { Id = Guid.NewGuid(), Nombre = "Diabetes" };
        var hipertension = new CondicionMedica { Id = Guid.NewGuid(), Nombre = "Hipertensión" };
        var obesidad = new CondicionMedica { Id = Guid.NewGuid(), Nombre = "Obesidad" };

        // Usuario tenía 3 condiciones guardadas
        var usuario = new UsuarioFake(uid, "user@mail.com", "Juan", "Pérez", "USR1")
        {
            CondicionesMedicas = new List<CondicionMedica> { diabetes, hipertension, obesidad }
        };

        
        var nuevosIds = new List<Guid> {};

        _usuarioRepoMock
            .Setup(r => r.GetByFirebaseUidAsync(uid, ct))
            .ReturnsAsync(usuario);

        _condicionRepoMock
            .Setup(r => r.GetByIdsAsync(It.IsAny<List<Guid>>(), ct))
            .ReturnsAsync(new List<CondicionMedica> { diabetes, hipertension ,obesidad});

        _usuarioRepoMock
            .Setup(r => r.SaveChangesAsync(ct))
            .Returns(Task.CompletedTask);

        var useCase = new GuardarCondicionesUseCase(_condicionRepoMock.Object, _usuarioRepoMock.Object);

        // Act
        var result = await useCase.HandleAsync(uid, nuevosIds, skip: false, ct);

        // Assert

        Assert.Empty(usuario.CondicionesMedicas);
        Assert.DoesNotContain(usuario.CondicionesMedicas, c => c.Nombre == "Obesidad" 
        && c.Nombre == "Hipertensión" && c.Nombre == "Diabetes");
        
        Assert.Equal(RegistroPaso.Gustos, usuario.PasoActual);

        _usuarioRepoMock.Verify(r => r.SaveChangesAsync(ct), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_DeberiaSkipearCuandoSkipea()
    {
        // Arrange
        var uid = "firebase_123";
        var ct = CancellationToken.None;

        // Act
        var result = await _useCase.HandleAsync(uid, new List<Guid>(), skip: true, ct);

        // Assert
       
        _usuarioRepoMock.Verify(r => r.SaveChangesAsync(ct), Times.Never);
    }

    [Fact]
    public async Task HandleAsync_DeberiaLanzarThrowCuandoNoEncuentraUsuario()
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

