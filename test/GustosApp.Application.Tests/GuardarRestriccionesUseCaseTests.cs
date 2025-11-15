using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;
using FluentAssertions;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using GustosApp.Application.Tests.mocks;
using GustosApp.Application.UseCases.UsuarioUseCases.CondicionesMedicasUseCases;
using GustosApp.Application.Interfaces;
using GustosApp.Application.UseCases.UsuarioUseCases.RestriccionesUseCases;



public class GuardarRestriccionesUseCaseTests
{
    private readonly Mock<IUsuarioRepository> _userRepo;
    private readonly Mock<IRestriccionRepository> _restriccionesRepo;
    private readonly Mock<IRegistroPasoService> _pasoService;
    private readonly Mock<ICacheService> _cache;
    private readonly GuardarRestriccionesUseCase _useCase;

    public GuardarRestriccionesUseCaseTests()
    {
        _userRepo = new Mock<IUsuarioRepository>();
        _restriccionesRepo = new Mock<IRestriccionRepository>();
        _pasoService = new Mock<IRegistroPasoService>();
        _cache = new Mock<ICacheService>();

        _useCase = new GuardarRestriccionesUseCase(
            _userRepo.Object,
            _restriccionesRepo.Object,
            _cache.Object,
             _pasoService.Object
        );
    }

    private Usuario CrearUsuarioConRestricciones(params Guid[] ids)
    {
        return new Usuario
        {
            Id = Guid.NewGuid(),
            FirebaseUid = "test-uid",
            PasoActual = RegistroPaso.Restricciones,
            Restricciones = ids.Select(id => new Restriccion { Id = id, Nombre = $"R-{id}" }).ToList()
        };
    }

    
    [Fact]
    public async Task HandleAsync_UsuarioNoExiste_ThrowInvalidOperation()
    {
        _userRepo.Setup(x => x.GetByFirebaseUidAsync("abc", It.IsAny<CancellationToken>()))
                 .ReturnsAsync((Usuario)null);

        Func<Task> act = async () => await _useCase.HandleAsync("abc", new List<Guid>(), false, default);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Usuario no encontrado.");
    }

    [Fact]
    public async Task HandleAsync_SkipTrue_LimpiaRestriccionesYAplicaPaso()
    {
        var u = CrearUsuarioConRestricciones(Guid.NewGuid());

        _userRepo.Setup(x => x.GetByFirebaseUidAsync("uid", It.IsAny<CancellationToken>()))
                 .ReturnsAsync(u);

        var result = await _useCase.HandleAsync("uid", new List<Guid>(), true, default);

        u.Restricciones.Should().BeEmpty();

        _pasoService.Verify(x =>
            x.AplicarPasoAsync(
                u,
                RegistroPaso.Restricciones,
                "registro:uid:restricciones",
                null,
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );

        result.Should().BeEmpty();
    }

   
    [Fact]
    public async Task HandleAsync_IdsInvalidos_ThrowArgumentException()
    {
        var u = CrearUsuarioConRestricciones(Guid.NewGuid());

        var ids = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };

        _userRepo.Setup(x => x.GetByFirebaseUidAsync("uid", It.IsAny<CancellationToken>()))
                 .ReturnsAsync(u);

        // Solo existe 1 restricción → mismatch
        _restriccionesRepo.Setup(x => x.GetRestriccionesByIdsAsync(ids, It.IsAny<CancellationToken>()))
                          .ReturnsAsync(new List<Restriccion> { new Restriccion { Id = ids[0] } });

        Func<Task> act = async () => await _useCase.HandleAsync("uid", ids, false, default);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Una o más restricciones no existen.");
    }

  
    [Fact]
    public async Task HandleAsync_SinCambios_NoAplicaPasoYDevuelveVacio()
    {
        var id = Guid.NewGuid();
        var u = CrearUsuarioConRestricciones(id);

        _userRepo.Setup(x => x.GetByFirebaseUidAsync("uid", It.IsAny<CancellationToken>()))
                 .ReturnsAsync(u);

        _restriccionesRepo.Setup(x => x.GetRestriccionesByIdsAsync(new List<Guid> { id }, It.IsAny<CancellationToken>()))
                          .ReturnsAsync(new List<Restriccion> { new Restriccion { Id = id } });

        var result = await _useCase.HandleAsync("uid", new List<Guid> { id }, false, default);

        result.Should().BeEmpty();

        _pasoService.Verify(x =>
            x.AplicarPasoAsync(It.IsAny<Usuario>(), It.IsAny<RegistroPaso>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

  
    [Fact]
    public async Task HandleAsync_AgregarNuevas_ActualizaUsuarioYAplicaPaso()
    {
        var idExistente = Guid.NewGuid();
        var idNuevo = Guid.NewGuid();

        var u = CrearUsuarioConRestricciones(idExistente);

        _userRepo.Setup(x => x.GetByFirebaseUidAsync("uid", It.IsAny<CancellationToken>()))
                 .ReturnsAsync(u);

        _restriccionesRepo.Setup(x => x.GetRestriccionesByIdsAsync(
            It.Is<List<Guid>>(l => l.Contains(idExistente) && l.Contains(idNuevo)),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Restriccion>
            {
                new Restriccion { Id = idExistente },
                new Restriccion { Id = idNuevo }
            });

        var result = await _useCase.HandleAsync("uid", new List<Guid> { idExistente, idNuevo }, false, default);

        u.Restricciones.Should().HaveCount(2);
        u.Restricciones.Any(r => r.Id == idNuevo).Should().BeTrue();

        _pasoService.Verify(x =>
            x.AplicarPasoAsync(
                u,
                RegistroPaso.Condiciones,
                "registro:uid:restricciones",
                It.IsAny<List<Guid>>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
    }

  
    [Fact]
    public async Task HandleAsync_QuitarRestricciones_UsuarioActualizado()
    {
        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();

        var usuario = CrearUsuarioConRestricciones(id1, id2);

        _userRepo.Setup(x => x.GetByFirebaseUidAsync("uid", default))
                 .ReturnsAsync(usuario);

        _restriccionesRepo.Setup(x => x.GetRestriccionesByIdsAsync(
            It.Is<List<Guid>>(l => l.SequenceEqual(new List<Guid> { id1 })),
            default))
        .ReturnsAsync(new List<Restriccion> { new Restriccion { Id = id1 } });

        await _useCase.HandleAsync("uid", new List<Guid> { id1 }, false, default);

        usuario.Restricciones.Should().HaveCount(1);
        usuario.Restricciones.Any(r => r.Id == id2).Should().BeFalse();
    }

 

    [Fact]
    public async Task HandleAsync_CompatibilidadDevuelveValores_UseCaseRetornaLista()
    {
        var id = Guid.NewGuid();

    
        var usuarioMock = new Mock<Usuario>();
        usuarioMock.SetupAllProperties(); 
        usuarioMock.Object.FirebaseUid = "uid";
        usuarioMock.Object.Restricciones = new List<Restriccion>();
        usuarioMock.Object.Gustos = new List<Gusto>();

     
        usuarioMock
            .Setup(u => u.ValidarCompatibilidad())
            .Returns(new List<string> { "Gusto1", "Gusto2" });

        _userRepo.Setup(r => r.GetByFirebaseUidAsync("uid", It.IsAny<CancellationToken>()))
            .ReturnsAsync(usuarioMock.Object);

        _restriccionesRepo
            .Setup(r => r.GetRestriccionesByIdsAsync(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Restriccion> { new Restriccion { Id = id } });

        var result = await _useCase.HandleAsync("uid", new List<Guid> { id }, false, default);

        result.Should().Contain(new[] { "Gusto1", "Gusto2" });
    }
}