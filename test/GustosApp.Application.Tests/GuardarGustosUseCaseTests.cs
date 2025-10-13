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
using GustosApp.Application.Tests.mocks;


  
    public class GuardarGustosUseCaseTests
    {
        private readonly Mock<IUsuarioRepository> _usuarioRepoMock;
        private readonly Mock<IGustoRepository> _gustoRepoMock;
        private readonly GuardarGustosUseCase _useCase;

        public GuardarGustosUseCaseTests()
        {
            _usuarioRepoMock = new Mock<IUsuarioRepository>();
            _gustoRepoMock = new Mock<IGustoRepository>();
            _useCase = new GuardarGustosUseCase(_usuarioRepoMock.Object, _gustoRepoMock.Object);
        }



    [Fact]
    public async Task HandleAsync_DeberiaDetectarConflictoPorRestriccion()
    {
        // Arrange
        var uid = "firebase_gluten";
        var tagGluten = new Tag { Nombre = "Gluten" };

        var gustoPizza = new Gusto { Id = Guid.NewGuid(), Nombre = "Pizza", Tags = new List<Tag> { tagGluten } };
        var gustoSushi = new Gusto { Id = Guid.NewGuid(), Nombre = "Sushi", Tags = new List<Tag>() };

        var usuario = new UsuarioRealFake(uid, "gluten@mail.com", "Ana", "Perez", "USR_GLUTEN");
        usuario.Restricciones.Add(new Restriccion { Nombre = "Sin gluten", TagsProhibidos = new List<Tag> { tagGluten } });

        _usuarioRepoMock.Setup(r => r.GetByFirebaseUidAsync(uid, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(usuario);
        _gustoRepoMock.Setup(r => r.GetByIdsAsync(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync(new List<Gusto> { gustoPizza, gustoSushi });
        _usuarioRepoMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                        .Returns(Task.CompletedTask);

        // Act
        var conflictos = await _useCase.HandleAsync(uid, new List<Guid> { gustoPizza.Id, gustoSushi.Id, Guid.NewGuid() }, CancellationToken.None);

        // Assert
        Assert.Contains("Pizza", conflictos);
        Assert.DoesNotContain(usuario.Gustos, g => g.Nombre == "Pizza"); // eliminado
        Assert.Equal(RegistroPaso.Verificacion, usuario.PasoActual);
    }
    [Fact]
    public async Task HandleAsync_DeberiaDetectarConflictoPorCondicionMedica()
    {
        // Arrange
        var uid = "firebase_diabetes";
        var tagAzucar = new Tag { Nombre = "Azúcar" };

        var gustoHelado = new Gusto { Id = Guid.NewGuid(), Nombre = "Helado", Tags = new List<Tag> { tagAzucar } };
        var gustoEnsalada = new Gusto { Id = Guid.NewGuid(), Nombre = "Ensalada verde", Tags = new List<Tag>() };

        var usuario = new UsuarioRealFake(uid, "diab@mail.com", "Carlos", "Lopez", "USR_DIAB");
        usuario.CondicionesMedicas.Add(new CondicionMedica { Nombre = "Diabetes", TagsCriticos = new List<Tag> { tagAzucar } });

        _usuarioRepoMock.Setup(r => r.GetByFirebaseUidAsync(uid, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(usuario);
        _gustoRepoMock.Setup(r => r.GetByIdsAsync(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync(new List<Gusto> { gustoHelado, gustoEnsalada });
        _usuarioRepoMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                        .Returns(Task.CompletedTask);

        // Act
        var conflictos = await _useCase.HandleAsync(uid, new List<Guid> { gustoHelado.Id, gustoEnsalada.Id, Guid.NewGuid() }, CancellationToken.None);

        // Assert
        Assert.Contains("Helado", conflictos);
        Assert.DoesNotContain(usuario.Gustos, g => g.Nombre == "Helado");
    }
    [Fact]
    public async Task HandleAsync_DeberiaCombinarRestriccionesYCondiciones()
    {
        // Arrange
        var uid = "firebase_mix";
        var tagGluten = new Tag { Nombre = "Gluten" };
        var tagAzucar = new Tag { Nombre = "Azúcar" };
        var tagMarisco = new Tag { Nombre = "Mariscos" };

        var gustoPizza = new Gusto { Id = Guid.NewGuid(), Nombre = "Pizza", Tags = new List<Tag> { tagGluten } };
        var gustoHelado = new Gusto { Id = Guid.NewGuid(), Nombre = "Helado", Tags = new List<Tag> { tagAzucar } };
        var gustoCeviche = new Gusto { Id = Guid.NewGuid(), Nombre = "Ceviche", Tags = new List<Tag> { tagMarisco } };

        var usuario = new UsuarioRealFake(uid, "mix@mail.com", "Lucia", "Mendez", "USR_MIX");
        usuario.Restricciones.Add(new Restriccion { Nombre = "Sin mariscos", TagsProhibidos = new List<Tag> { tagMarisco } });
        usuario.CondicionesMedicas.Add(new CondicionMedica { Nombre = "Diabetes", TagsCriticos = new List<Tag> { tagAzucar } });

        _usuarioRepoMock.Setup(r => r.GetByFirebaseUidAsync(uid, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(usuario);
        _gustoRepoMock.Setup(r => r.GetByIdsAsync(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync(new List<Gusto> { gustoPizza, gustoHelado, gustoCeviche });
        _usuarioRepoMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                        .Returns(Task.CompletedTask);

        // Act
        var conflictos = await _useCase.HandleAsync(uid, new List<Guid> { gustoPizza.Id, gustoHelado.Id, gustoCeviche.Id }, CancellationToken.None);

        // Assert
        Assert.Equal(2, conflictos.Count); // Helado y Ceviche
        Assert.Contains("Helado", conflictos);
        Assert.Contains("Ceviche", conflictos);
        Assert.DoesNotContain(usuario.Gustos, g => g.Nombre == "Helado");
        Assert.DoesNotContain(usuario.Gustos, g => g.Nombre == "Ceviche");
    }


}

