using Moq;
using GustosApp.Application.Interfaces;
using GustosApp.Application.UseCases.UsuarioUseCases.GustoUseCases;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;

public class GuardarGustosUseCaseTests
{
    private readonly Mock<IUsuarioRepository> _userRepo;
    private readonly Mock<IGustoRepository> _gustoRepo;
    private readonly Mock<ICacheService> _cache;
    private readonly Mock<IRegistroPasoService> _pasoService;

    private readonly GuardarGustosUseCase _useCase;

    public GuardarGustosUseCaseTests()
    {
        _userRepo = new Mock<IUsuarioRepository>();
        _gustoRepo = new Mock<IGustoRepository>();
        _cache = new Mock<ICacheService>();
        _pasoService = new Mock<IRegistroPasoService>();

        _useCase = new GuardarGustosUseCase(
            _userRepo.Object,
            _gustoRepo.Object,
            _cache.Object,
            _pasoService.Object
        );
    }

    private Usuario CrearUsuarioConGustos(params Guid[] gustos)
    {
        var u = new Usuario
        {
            FirebaseUid = "uid123",
            Gustos = new List<Gusto>()
        };

        foreach (var id in gustos)
        {
            u.Gustos.Add(new Gusto { Id = id });
        }

        return u;
    }

  

  
    [Fact]
    public async Task HandleAsync_UsuarioNoExiste_DeberiaLanzarExcepcion()
    {
        _userRepo.Setup(r => r.GetByFirebaseUidAsync("uid123", It.IsAny<CancellationToken>()))
                 .ReturnsAsync((Usuario)null);

        await Assert.ThrowsAsync<Exception>(() =>
            _useCase.HandleAsync("uid123", new List<Guid>(), CancellationToken.None)
        );
    }

    
    [Fact]
    public async Task HandleAsync_BorrarTodo_DeberiaLimpiarYResetearPaso()
    {
        var u = CrearUsuarioConGustos(Guid.NewGuid(), Guid.NewGuid());

        _userRepo.Setup(r => r.GetByFirebaseUidAsync("uid123", It.IsAny<CancellationToken>()))
                 .ReturnsAsync(u);

        var result = await _useCase.HandleAsync("uid123", new List<Guid>(), CancellationToken.None);

        Assert.Empty(u.Gustos);

        _pasoService.Verify(p => p.AplicarPasoAsync(
            u,
            RegistroPaso.Gustos,
            "registro:uid123:gustos",
            null,
            It.IsAny<CancellationToken>()),
            Times.Once);
    }


    [Fact]
    public async Task HandleAsync_MenosDeTresGustos_DeberiaLanzarExcepcion()
    {
        var u = CrearUsuarioConGustos();

        _userRepo.Setup(r => r.GetByFirebaseUidAsync("uid123", It.IsAny<CancellationToken>()))
                 .ReturnsAsync(u);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            _useCase.HandleAsync("uid123", new List<Guid> { Guid.NewGuid(), Guid.NewGuid() }, CancellationToken.None)
        );
    }


    [Fact]
    public async Task HandleAsync_GustosInvalidos_DeberiaLanzarExcepcion()
    {
        var u = CrearUsuarioConGustos();

        _userRepo.Setup(r => r.GetByFirebaseUidAsync("uid123", It.IsAny<CancellationToken>()))
                 .ReturnsAsync(u);

        var ids = new List<Guid> { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };

        // BD devuelve solo 1 válido → inválido
        _gustoRepo.Setup(r => r.GetByIdsAsync(ids, It.IsAny<CancellationToken>()))
                  .ReturnsAsync(new List<Gusto>
                  {
                      new Gusto { Id = ids[0] }
                  });

        await Assert.ThrowsAsync<ArgumentException>(() =>
            _useCase.HandleAsync("uid123", ids, CancellationToken.None)
        );
    }

    [Fact]
    public async Task HandleAsync_SinCambios_NoDebeAvanzarPaso()
    {
        var g1 = Guid.NewGuid();
        var g2 = Guid.NewGuid();
        var g3 = Guid.NewGuid();
        var u = CrearUsuarioConGustos(g1, g2, g3);

        _userRepo.Setup(r => r.GetByFirebaseUidAsync("uid123", It.IsAny<CancellationToken>()))
                 .ReturnsAsync(u);

        _gustoRepo.Setup(r => r.GetByIdsAsync(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Gusto> { new Gusto { Id = g1 }, new Gusto { Id = g2 }, new Gusto { Id = g3 } });

        var result = await _useCase.HandleAsync("uid123", new List<Guid> { g1, g2, g3 }, CancellationToken.None);

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
    public async Task HandleAsync_AgregarGustos_DeberiaAgregarYAvanzarPaso()
    {
        var g1 = Guid.NewGuid();
        var g2 = Guid.NewGuid();

        var u = CrearUsuarioConGustos(g1);

        _userRepo.Setup(r => r.GetByFirebaseUidAsync("uid123", It.IsAny<CancellationToken>()))
                 .ReturnsAsync(u);

        _gustoRepo.Setup(r => r.GetByIdsAsync(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
                  .ReturnsAsync(new List<Gusto>
                  {
                      new Gusto { Id = g1 },
                      new Gusto { Id = g2 }
                  });

        var ids = new List<Guid> { g1, g2, Guid.NewGuid() }; // siempre >= 3
        ids = ids.Take(2).ToList(); // forzar 2 válidos + 1 inválido removido arriba

        // pero ajustemos: necesitamos al menos 3 válidos
        ids = new List<Guid> { g1, g2, Guid.NewGuid() }; // -> 3 IDs

        // simular que GetByIds devuelve 3 válidos
        _gustoRepo.Setup(r => r.GetByIdsAsync(ids, It.IsAny<CancellationToken>()))
                  .ReturnsAsync(ids.Select(id => new Gusto { Id = id }).ToList());

        await _useCase.HandleAsync("uid123", ids, CancellationToken.None);

        Assert.Equal(ids.Count, u.Gustos.Count);

        _pasoService.Verify(p => p.AplicarPasoAsync(
            u,
            RegistroPaso.Verificacion,
            "registro:uid123:gustos",
            ids,
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_QuitarGustos_DeberiaQuitarCorrectamente()
    {
        var g1 = Guid.NewGuid();
        var g2 = Guid.NewGuid();

        var u = CrearUsuarioConGustos(g1, g2);

        var ids = new List<Guid> { g1, Guid.NewGuid(), Guid.NewGuid() }; // siempre >= 3

        // hacer que GetByIds devuelva todas válidas
        _gustoRepo.Setup(r => r.GetByIdsAsync(ids, It.IsAny<CancellationToken>()))
                  .ReturnsAsync(ids.Select(x => new Gusto { Id = x }).ToList());

        _userRepo.Setup(r => r.GetByFirebaseUidAsync("uid123", It.IsAny<CancellationToken>()))
                 .ReturnsAsync(u);

        await _useCase.HandleAsync("uid123", ids, CancellationToken.None);

        Assert.DoesNotContain(u.Gustos, g => g.Id == g2);

        _pasoService.Verify(p => p.AplicarPasoAsync(
            u,
            RegistroPaso.Verificacion,
            "registro:uid123:gustos",
            ids,
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

   
    [Fact]
    public async Task HandleAsync_AgregarYQuitarGustos_DeberiaActualizarCorrectamente()
    {
        var g1 = Guid.NewGuid(); // queda
        var g2 = Guid.NewGuid(); // se quita
        var nuevo = Guid.NewGuid();

        var u = CrearUsuarioConGustos(g1, g2);

        var ids = new List<Guid> { g1, nuevo, Guid.NewGuid() }; // >=3

        _gustoRepo.Setup(r => r.GetByIdsAsync(ids, It.IsAny<CancellationToken>()))
                  .ReturnsAsync(ids.Select(x => new Gusto { Id = x }).ToList());

        _userRepo.Setup(r => r.GetByFirebaseUidAsync("uid123", It.IsAny<CancellationToken>()))
                 .ReturnsAsync(u);

        await _useCase.HandleAsync("uid123", ids, CancellationToken.None);

        Assert.Equal(3, u.Gustos.Count);
        Assert.DoesNotContain(u.Gustos, g => g.Id == g2);
        Assert.Contains(u.Gustos, g => g.Id == nuevo);

        _pasoService.Verify(p => p.AplicarPasoAsync(
            u,
            RegistroPaso.Verificacion,
            "registro:uid123:gustos",
            ids,
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    
    [Fact]
    public async Task HandleAsync_ValidarCompatibilidad_DeberiaRemoverGustosIncompatibles()
    {
        var gustoCompatible = Guid.NewGuid();
        var gustoIncompatible = Guid.NewGuid();

        var u = new Usuario
        {
            FirebaseUid = "uid123",
            Gustos = new List<Gusto>
            {
                new Gusto { Id = gustoCompatible, Nombre="OK", Tags=new List<Tag>{ new Tag{ Nombre="fruta"} } },
                new Gusto { Id = gustoIncompatible, Nombre="PROHIBIDO", Tags=new List<Tag>{ new Tag{ Nombre="gluten"} } }
            },
            Restricciones = new List<Restriccion>
            {
                new Restriccion
                {
                    TagsProhibidos = new List<Tag>{ new Tag{ Nombre="gluten"} }
                }
            }
        };

        var ids = new List<Guid> { gustoCompatible, gustoIncompatible, Guid.NewGuid() };

        _userRepo.Setup(r => r.GetByFirebaseUidAsync("uid123", It.IsAny<CancellationToken>()))
                 .ReturnsAsync(u);

        _gustoRepo.Setup(r => r.GetByIdsAsync(ids, It.IsAny<CancellationToken>()))
                  .ReturnsAsync(ids.Select(x => new Gusto { Id = x, Tags = new List<Tag>() }).ToList());

        await _useCase.HandleAsync("uid123", ids, CancellationToken.None);

        Assert.DoesNotContain(u.Gustos, g => g.Id == gustoIncompatible);

        _pasoService.Verify(p => p.AplicarPasoAsync(
            u,
            RegistroPaso.Verificacion,
            "registro:uid123:gustos",
            ids,
            It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
