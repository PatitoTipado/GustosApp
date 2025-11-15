using System;
using System.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Xunit;
using GustosApp.Application.Interfaces;
using GustosApp.Application.UseCases.UsuarioUseCases.CondicionesMedicasUseCases;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;

public class GuardarCondicionesUseCaseTests
{
    private readonly Mock<ICondicionMedicaRepository> _condRepo;
    private readonly Mock<IUsuarioRepository> _userRepo;
    private readonly Mock<IRegistroPasoService> _pasoService;
    private readonly Mock<ICacheService> _cache;
    private readonly GuardarCondicionesUseCase _useCase;
    private IEnumerable<object>? c1;

    public GuardarCondicionesUseCaseTests()
    {
        _condRepo = new Mock<ICondicionMedicaRepository>();
        _userRepo = new Mock<IUsuarioRepository>();
        _pasoService = new Mock<IRegistroPasoService>();
        _cache = new Mock<ICacheService>();

        _useCase = new GuardarCondicionesUseCase(
            _condRepo.Object,
            _userRepo.Object,
              _cache.Object,
            _pasoService.Object
        );
    }

    private Usuario CrearUsuarioCon(params Guid[] condiciones)
    {
        var u = new Usuario
        {
            FirebaseUid = "uid123",
            CondicionesMedicas = new List<CondicionMedica>()
        };

        foreach (var id in condiciones)
        {
            u.CondicionesMedicas.Add(new CondicionMedica { Id = id });
        }

        return u;
    }


    [Fact]
    public async Task HandleAsync_SkipTrue_DeberiaLimpiarYSetearPaso()
    {
        var usuario = CrearUsuarioCon(Guid.NewGuid(), Guid.NewGuid());

        _userRepo.Setup(r => r.GetByFirebaseUidAsync("uid123", It.IsAny<CancellationToken>()))
                 .ReturnsAsync(usuario);

        var result = await _useCase.HandleAsync("uid123", new List<Guid>(), skip: true, CancellationToken.None);

        Assert.Empty(usuario.CondicionesMedicas);

        _pasoService.Verify(p => p.AplicarPasoAsync(
            usuario,
            RegistroPaso.Condiciones,
            "registro:uid123:condiciones",
            null,
            It.IsAny<CancellationToken>()),
            Times.Once);
    }


    [Fact]
    public async Task HandleAsync_IdsInvalidos_DeberiaLanzarExcepcion()
    {
        var usuario = CrearUsuarioCon();

        _userRepo.Setup(r => r.GetByFirebaseUidAsync("uid123", It.IsAny<CancellationToken>()))
                 .ReturnsAsync(usuario);

        var ids = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };

        // Repositorio devuelve solo UNA entidad, falta otra → inválido
        _condRepo.Setup(r => r.GetByIdsAsync(ids, It.IsAny<CancellationToken>()))
                 .ReturnsAsync(new List<CondicionMedica> { new CondicionMedica { Id = ids[0] } });

        await Assert.ThrowsAsync<ArgumentException>(() =>
            _useCase.HandleAsync("uid123", ids, skip: false, CancellationToken.None)
        );
    }

   
    [Fact]
    public async Task HandleAsync_SinCambios_NoDebeAvanzarPaso()
    {
        var c1 = Guid.NewGuid();
        var usuario = CrearUsuarioCon(c1);

        _userRepo.Setup(r => r.GetByFirebaseUidAsync("uid123", It.IsAny<CancellationToken>()))
                 .ReturnsAsync(usuario);

        _condRepo.Setup(r => r.GetByIdsAsync(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(new List<CondicionMedica> { new CondicionMedica { Id = c1 } });

        var ids = new List<Guid> { c1 };

        var result = await _useCase.HandleAsync("uid123", ids, false, CancellationToken.None);

        Assert.Empty(result);

        _pasoService.Verify(p => p.AplicarPasoAsync(
            It.IsAny<Usuario>(),
            It.IsAny<RegistroPaso>(),
            It.IsAny<string>(),
            It.IsAny<object>(),
            It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task HandleAsync_AgregarCondiciones_DeberiaAvanzarPaso()
    {
        var cExisting = Guid.NewGuid();
        var cNew = Guid.NewGuid();

        var usuario = CrearUsuarioCon(cExisting);

        _userRepo.Setup(r => r.GetByFirebaseUidAsync("uid123", It.IsAny<CancellationToken>()))
                 .ReturnsAsync(usuario);

        _condRepo.Setup(r => r.GetByIdsAsync(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(new List<CondicionMedica>
                 {
                     new CondicionMedica { Id = cExisting },
                     new CondicionMedica { Id = cNew }
                 });

        var result = await _useCase.HandleAsync(
            "uid123",
            new List<Guid> { cExisting, cNew },
            false,
            CancellationToken.None);

        Assert.Equal(2, usuario.CondicionesMedicas.Count);

        _pasoService.Verify(p => p.AplicarPasoAsync(
            usuario,
            RegistroPaso.Gustos,
            "registro:uid123:condiciones",
            It.Is<List<Guid>>(list => list.Contains(cExisting) && list.Contains(cNew)),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

 
    [Fact]
    public async Task HandleAsync_QuitarCondiciones_DeberiaEliminarCorrecto()
    {
        var c1 = Guid.NewGuid();
        var c2 = Guid.NewGuid();

        var usuario = CrearUsuarioCon(c1, c2);

        _userRepo.Setup(r => r.GetByFirebaseUidAsync("uid123", It.IsAny<CancellationToken>()))
                 .ReturnsAsync(usuario);

        _condRepo.Setup(r => r.GetByIdsAsync(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(new List<CondicionMedica>
                 {
                     new CondicionMedica { Id = c1 }
                 });

        var result = await _useCase.HandleAsync("uid123", new List<Guid> { c1 }, false, CancellationToken.None);

        Assert.Single(usuario.CondicionesMedicas);
        Assert.Equal(c1, usuario.CondicionesMedicas.First().Id);

        _pasoService.Verify(p => p.AplicarPasoAsync(
            usuario,
            RegistroPaso.Gustos,
            "registro:uid123:condiciones",
            It.Is<List<Guid>>(list => list.Count == 1 && list[0] == c1),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

  
    [Fact]
    public async Task HandleAsync_AgregarYQuitar_DeberiaActualizarCorrectamente()
    {
        var c1 = Guid.NewGuid(); // existente
        var cEliminar = Guid.NewGuid(); // existente que se quitará
        var cNuevo = Guid.NewGuid();   // nuevo

        var usuario = CrearUsuarioCon(c1, cEliminar);

        _userRepo.Setup(r => r.GetByFirebaseUidAsync("uid123", It.IsAny<CancellationToken>()))
                 .ReturnsAsync(usuario);

        _condRepo.Setup(r => r.GetByIdsAsync(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(new List<CondicionMedica>
                 {
                     new CondicionMedica { Id = c1 },
                     new CondicionMedica { Id = cNuevo }
                 });

        var idsNuevos = new List<Guid> { c1, cNuevo };

        var result = await _useCase.HandleAsync("uid123", idsNuevos, false, CancellationToken.None);

        Assert.Equal(2, usuario.CondicionesMedicas.Count);
        Assert.DoesNotContain(usuario.CondicionesMedicas, c => c.Id == cEliminar);

        _pasoService.Verify(p => p.AplicarPasoAsync(
            usuario,
            RegistroPaso.Gustos,
            "registro:uid123:condiciones",
            It.Is<List<Guid>>(l => l.Count == 2 && l.Contains(c1) && l.Contains(cNuevo)),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    

    [Fact]
    public async Task HandleAsync_ValidarCompatibilidad_DeberiaRemoverGustos()
    {
        // Preparar un usuario con un gusto incompatible
        var condId = Guid.NewGuid();
        var usuario = new Usuario
        {
            FirebaseUid = "uid123",
            CondicionesMedicas = new List<CondicionMedica>(),
            Gustos = new List<Gusto>
           {
               new Gusto
               {
                   Id = Guid.NewGuid(),
                   Nombre = "Lácteos",
                   Tags = new List<Tag> { new Tag { Nombre = "Lactosa", Tipo = TipoTag.Ingrediente } }
               }
           }
        };

        var condicion = new CondicionMedica
        {
            Id = condId,
            TagsCriticos = new List<Tag> { new Tag { Nombre = "lactosa", Tipo = TipoTag.Ingrediente } }
        };

        _userRepo.Setup(r => r.GetByFirebaseUidAsync("uid123", It.IsAny<CancellationToken>()))
                 .ReturnsAsync(usuario);

        _condRepo.Setup(r => r.GetByIdsAsync(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(new List<CondicionMedica> { condicion });

        var result = await _useCase.HandleAsync("uid123", new List<Guid> { condId }, false, CancellationToken.None);

        Assert.Empty(usuario.Gustos);  // gusto eliminado

        _pasoService.Verify(p => p.AplicarPasoAsync(
            usuario,
            RegistroPaso.Gustos,
            "registro:uid123:condiciones",
            It.IsAny<List<Guid>>(),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }
}