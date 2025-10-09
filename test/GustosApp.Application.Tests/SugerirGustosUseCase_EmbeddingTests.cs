using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;
using GustosApp.Application.UseCases;
using GustosApp.Application.Interfaces;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;

namespace GustosApp.Application.Tests
{
    public class SugerirGustosUseCase_EmbeddingTests
    {

    [Fact]
    public async Task HandleAsync_DevuelveSegunEmbeddings()
    {

        var firebaseUid = "uid-123";
        var gustoExistenteId = Guid.NewGuid();

        // Crear usuario usando el constructor existente
        var usuario = new Usuario(firebaseUid, "u@mail.com", "Nombre", "Apellido", "idUsuario")
        {
            // agregar gustos usando la colección pública
        };
        usuario.Gustos.Add(new Gusto { Id = gustoExistenteId, Nombre = "Pizza" });

        var gustoPizza = new Gusto { Id = gustoExistenteId, Nombre = "Pizza" };
        var gustoPasta = new Gusto { Id = Guid.NewGuid(), Nombre = "Pasta" };
        var gustoSushi = new Gusto { Id = Guid.NewGuid(), Nombre = "Sushi" };   

        var allGustos = new List<Gusto> { gustoPizza, gustoPasta, gustoSushi };

    var mockUsuarios = new Mock<IUsuarioRepository>();
        mockUsuarios
            .Setup(r => r.GetByFirebaseUidWithGustosAsync(firebaseUid, It.IsAny<CancellationToken>()))
            .ReturnsAsync(usuario);
        mockUsuarios
            .Setup(r => r.GetAllWithGustosAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Usuario> { usuario});
            
            var mockGustos = new Mock<IGustoRepository>();
            mockGustos
                .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(allGustos);

        var userVec = new float[] { 1f, 0f, 0f }; 
        var pizzaVec = new float[] { 1f, 0f, 0f }; 
        var sushiVec = new float[] { 0f, 1f, 0f }; 
        
        var mockEmbedding = new Mock<IEmbeddingService>();

        // Devuelve embedding para el texto del usuario y para cada gusto por nombre
        mockEmbedding.Setup(m => m.GetTextEmbeddingAsync(It.Is<string>(s => s != null && s.Contains("Pizza")), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pizzaVec);
        mockEmbedding.Setup(m => m.GetTextEmbeddingAsync(It.Is<string>(s => s != null && s.Contains("Pasta")), It.IsAny<CancellationToken>()))
            .ReturnsAsync(userVec);
        mockEmbedding.Setup(m => m.GetTextEmbeddingAsync(It.Is<string>(s => s != null && s.Contains("Sushi")), It.IsAny<CancellationToken>()))
            .ReturnsAsync(sushiVec);

    var useCase = new SugerirGustosUseCase(mockUsuarios.Object, mockGustos.Object, mockEmbedding.Object);
        var resultados = await useCase.HandleAsync(firebaseUid, top: 2,ct: CancellationToken.None);

        Assert.NotNull(resultados);
        Assert.DoesNotContain(resultados, g => g.ID == gustoPizza.Id);
        Assert.Contains(resultados, g => g.ID == gustoPasta.Id);
        }
    }
}