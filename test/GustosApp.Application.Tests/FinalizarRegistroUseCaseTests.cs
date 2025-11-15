using Moq;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GustosApp.Application.Interfaces;
using GustosApp.Application.UseCases.UsuarioUseCases;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;

public class FinalizarRegistroUseCaseTests
{
    private readonly Mock<IUsuarioRepository> _usuarioRepo;
    private readonly Mock<ICacheService> _cache;
    private readonly FinalizarRegistroUseCase _useCase;

    public FinalizarRegistroUseCaseTests()
    {
        _usuarioRepo = new Mock<IUsuarioRepository>();
        _cache = new Mock<ICacheService>();

        _useCase = new FinalizarRegistroUseCase(_usuarioRepo.Object, _cache.Object);
    }

    private Usuario CrearUsuario(
        List<Gusto>? gustos = null,
        List<Restriccion>? restricciones = null,
        List<CondicionMedica>? condiciones = null)
    {
        return new Usuario
        {
            FirebaseUid = "uid123",
            Gustos = gustos ?? new List<Gusto>(),
            Restricciones = restricciones ?? new List<Restriccion>(),
            CondicionesMedicas = condiciones ?? new List<CondicionMedica>(),
        };
    }


    [Fact]
    public async Task HandleAsync_UsuarioNoExiste_DeberiaLanzarExcepcion()
    {
        _usuarioRepo.Setup(r => r.GetByFirebaseUidAsync("uid123", It.IsAny<CancellationToken>()))
                    .ReturnsAsync((Usuario)null);

        await Assert.ThrowsAsync<Exception>(() =>
            _useCase.HandleAsync("uid123", CancellationToken.None)
        );
    }

   
    [Fact]
    public async Task HandleAsync_SinGustos_DeberiaLanzarExcepcion()
    {
        var usuario = CrearUsuario(gustos: new List<Gusto>());

        _usuarioRepo.Setup(r => r.GetByFirebaseUidAsync("uid123", It.IsAny<CancellationToken>()))
                    .ReturnsAsync(usuario);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _useCase.HandleAsync("uid123", CancellationToken.None)
        );
    }


    [Fact]
    public async Task HandleAsync_GustoIncompatibleConRestricciones_DeberiaLanzarExcepcion()
    {
        var usuario = CrearUsuario(
            gustos: new List<Gusto>
            {
                new Gusto
                {
                    Nombre = "Pizza Gluten",
                    Tags = new List<Tag> { new Tag { Nombre = "gluten" } }
                }
            },
            restricciones: new List<Restriccion>
            {
                new Restriccion
                {
                    TagsProhibidos = new List<Tag>
                    {
                        new Tag { Nombre = "gluten" }
                    }
                }
            }
        );

        _usuarioRepo.Setup(r => r.GetByFirebaseUidAsync("uid123", It.IsAny<CancellationToken>()))
                    .ReturnsAsync(usuario);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _useCase.HandleAsync("uid123", CancellationToken.None)
        );

        Assert.Contains("Pizza Gluten", ex.Message);
    }

    
    [Fact]
    public async Task HandleAsync_GustoIncompatibleConCondiciones_DeberiaLanzarExcepcion()
    {
        var usuario = CrearUsuario(
            gustos: new List<Gusto>
            {
                new Gusto
                {
                    Nombre = "Helado Lactosa",
                    Tags = new List<Tag> { new Tag { Nombre = "lactosa" } }
                }
            },
            condiciones: new List<CondicionMedica>
            {
                new CondicionMedica
                {
                    TagsCriticos = new List<Tag>
                    {
                        new Tag { Nombre = "lactosa" }
                    }
                }
            }
        );

        _usuarioRepo.Setup(r => r.GetByFirebaseUidAsync("uid123",
                It.IsAny<CancellationToken>()))
                    .ReturnsAsync(usuario);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _useCase.HandleAsync("uid123", CancellationToken.None)
        );

        Assert.Contains("Helado Lactosa", ex.Message);
    }

    
    [Fact]
    public async Task HandleAsync_GustoIncompatibleConAmbos_DeberiaLanzarExcepcion()
    {
        var usuario = CrearUsuario(
            gustos: new List<Gusto>
            {
                new Gusto
                {
                    Nombre = "Chocolate Cacahuate",
                    Tags = new List<Tag> { new Tag { Nombre = "mani" } }
                }
            },
            restricciones: new List<Restriccion>
            {
                new Restriccion
                {
                    TagsProhibidos = new List<Tag> { new Tag { Nombre = "MANI" } }
                }
            },
            condiciones: new List<CondicionMedica>
            {
                new CondicionMedica
                {
                    TagsCriticos = new List<Tag> { new Tag { Nombre = "mani" } }
                }
            }
        );

        _usuarioRepo.Setup(r => r.GetByFirebaseUidAsync("uid123",
                It.IsAny<CancellationToken>()))
                    .ReturnsAsync(usuario);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _useCase.HandleAsync("uid123", CancellationToken.None)
        );

        Assert.Contains("Chocolate Cacahuate", ex.Message);
    }

   
    [Fact]
    public async Task HandleAsync_TodoValido_DeberiaFinalizarRegistro()
    {
        var usuario = CrearUsuario(
            gustos: new List<Gusto>
            {
                new Gusto
                {
                    Nombre = "Sushi",
                    Tags = new List<Tag> { new Tag { Nombre = "pescado" } }
                }
            }
        );

        _usuarioRepo.Setup(r => r.GetByFirebaseUidAsync("uid123", It.IsAny<CancellationToken>()))
                    .ReturnsAsync(usuario);

        await _useCase.HandleAsync("uid123", CancellationToken.None);

        Assert.Equal(RegistroPaso.Finalizado, usuario.PasoActual);

        _usuarioRepo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }


    [Fact]
    public async Task HandleAsync_TodoValido_DeberiaEliminarCache()
    {
        var usuario = CrearUsuario(
            gustos: new List<Gusto>
            {
                new Gusto
                {
                    Nombre = "Hamburguesa",
                    Tags = new List<Tag> { new Tag { Nombre = "carne" } }
                }
            }
        );

        _usuarioRepo.Setup(r => r.GetByFirebaseUidAsync("uid123", It.IsAny<CancellationToken>()))
                    .ReturnsAsync(usuario);

        await _useCase.HandleAsync("uid123", CancellationToken.None);

        _cache.Verify(c => c.DeleteAsync("registro:uid123:restricciones"), Times.Once);
        _cache.Verify(c => c.DeleteAsync("registro:uid123:condiciones"), Times.Once);
        _cache.Verify(c => c.DeleteAsync("registro:uid123:gustos"), Times.Once);
        _cache.Verify(c => c.DeleteAsync("registro:uid123:estado"), Times.Once);
    }


    [Fact]
    public async Task HandleAsync_TagsCaseInsensitive_DeberiaDetectarConflicto()
    {
        var usuario = CrearUsuario(
            gustos: new List<Gusto>
            {
                new Gusto
                {
                    Nombre = "Almendrado",
                    Tags = new List<Tag> { new Tag { Nombre = "MANI" } }
                }
            },
            restricciones: new List<Restriccion>
            {
                new Restriccion
                {
                    TagsProhibidos = new List<Tag>
                    {
                        new Tag { Nombre = "mani" }
                    }
                }
            }
        );

        _usuarioRepo.Setup(r => r.GetByFirebaseUidAsync("uid123",
                It.IsAny<CancellationToken>()))
                    .ReturnsAsync(usuario);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _useCase.HandleAsync("uid123", CancellationToken.None)
        );

        Assert.Contains("Almendrado", ex.Message);
    }
}
