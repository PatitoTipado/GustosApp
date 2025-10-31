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
            var repoMock = new Mock<IValoracionUsuarioRepository>();
            repoMock.Setup(r => r.ExisteValoracionAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(false);

            var useCase = new CrearValoracionUsuarioUseCase(repoMock.Object);
            var usuarioId = Guid.NewGuid();
            var restauranteId = Guid.NewGuid();

            await useCase.HandleAsync(usuarioId, restauranteId, 5, "Excelente servicio", CancellationToken.None);

            repoMock.Verify(r => r.CrearAsync(It.Is<Valoracion>(
                v => v.UsuarioId == usuarioId &&
                v.RestauranteId == restauranteId &&
                v.ValoracionUsuario == 5
                ), It.IsAny<CancellationToken>()), Times.Once);
        }

     [Fact]
     public async Task NoPermitirValoracionDuplicada()
        {
            var repoMock = new Mock<IValoracionUsuarioRepository>();
            repoMock.Setup(r => r.ExisteValoracionAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var useCase = new CrearValoracionUsuarioUseCase(repoMock.Object);

            await Assert.ThrowsAsync<ArgumentException>(() =>
            useCase.HandleAsync(Guid.NewGuid(), Guid.NewGuid(), 4, "Ya valoro", CancellationToken.None));
        }

        [Fact]
        public async Task NoPermitirValoracionFueraDelRango()
        {
            var repoMock = new Mock<IValoracionUsuarioRepository>();
            var useCase = new CrearValoracionUsuarioUseCase(repoMock.Object);

            await Assert.ThrowsAsync<ArgumentException>(() => 
            useCase.HandleAsync(Guid.NewGuid(),Guid.NewGuid(),6,"Invalid",CancellationToken.None));
        }
    }
}
