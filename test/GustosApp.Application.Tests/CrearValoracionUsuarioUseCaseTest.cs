using GustosApp.Application.Interfaces;
using GustosApp.Application.UseCases;
using GustosApp.Domain.Model;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GustosApp.Application.Tests
{
    public class CrearValoracionUsuarioUseCaseTest
    {

        [Fact]
        public async Task CrearValoracion()
        {
            var repoMock = new Mock<IOpinionRestauranteRepository>();
            repoMock.Setup(r => r.ExisteValoracionAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(false);

            var useCase = new CrearOpinionRestaurante(repoMock.Object);
            var usuarioId = Guid.NewGuid();
            var restauranteId = Guid.NewGuid();
            var titulo = "Buen restaurante";
            var img = "";
            await useCase.HandleAsync(usuarioId, restauranteId, 5, "Excelente servicio",titulo,img, CancellationToken.None);

            repoMock.Verify(r => r.CrearAsync(It.Is<OpinionRestaurante>(
                v => v.UsuarioId == usuarioId &&
                v.RestauranteId == restauranteId &&
                v.Valoracion == 5
                ), It.IsAny<CancellationToken>()), Times.Once);
        }

       

        [Fact]
     public async Task NoPermitirValoracionDuplicada()
        {
            var repoMock = new Mock<IOpinionRestauranteRepository>();
            repoMock.Setup(r => r.ExisteValoracionAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var useCase = new CrearOpinionRestaurante(repoMock.Object);

            await Assert.ThrowsAsync<ArgumentException>(() =>
            useCase.HandleAsync(Guid.NewGuid(), Guid.NewGuid(), 4, "Ya valoro","Buen restaurante","", CancellationToken.None));
        }

        [Fact]
        public async Task NoPermitirValoracionFueraDelRango()
        {
            var repoMock = new Mock<IOpinionRestauranteRepository>();
            var useCase = new CrearOpinionRestaurante(repoMock.Object);

            await Assert.ThrowsAsync<ArgumentException>(() => 
            useCase.HandleAsync(Guid.NewGuid(),Guid.NewGuid(),6,"Buen restaurante","img","Invalid",CancellationToken.None));
        }
    }
}
