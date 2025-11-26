using GustosApp.Application.Common.Exceptions;
using GustosApp.Application.UseCases.UsuarioUseCases;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using GustosApp.Domain.Model.@enum;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GustosApp.Application.Tests
{
    public class AgregarUsuarioRestauranteFavoritoUseCaseTest
    {
        [Fact]
        public async Task AgregarUsuarioRestauranteFavorito()
        {
            var usuario = new Usuario
            {
                Id = Guid.NewGuid(),
                FirebaseUid = "abc",
                Plan = 0
            };

            var usuarioRepoMock = new Mock<IUsuarioRepository>();
            var favoritoRepoMock = new Mock<IUsuarioRestauranteFavoritoRepository>();

            usuarioRepoMock
                .Setup(r => r.GetByFirebaseUidAsync("abc", It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            favoritoRepoMock
                .Setup(r => r.CountByUsuarioAsync(usuario.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(0);

            favoritoRepoMock
                .Setup(r => r.ExistsAsync(usuario.Id, It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var useCase = new AgregarUsuarioRestauranteFavoritoUseCase(
                favoritoRepoMock.Object,
                usuarioRepoMock.Object
                );

            var restauranteId = Guid.NewGuid();

            await useCase.HandleAsync("abc", restauranteId);

            favoritoRepoMock.Verify(r => r.CrearAsync(
                It.Is<UsuarioRestauranteFavorito>(f =>
                f.UsuarioId == usuario.Id &&
                f.RestauranteId == restauranteId),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }
       
        [Fact]
        public async Task AgregarUsuarioRestauranteFavorito_CuandoSinPlanYLlegaAlLimite_LanzaExcepcion()
        {
            var usuario = new Usuario
            {
                Id = Guid.NewGuid(),
                FirebaseUid = "abc",
                Plan = 0
            };

            var usuarioRepoMock = new Mock<IUsuarioRepository>();
            var favoritoRepoMock = new Mock<IUsuarioRestauranteFavoritoRepository>();

            usuarioRepoMock
                .Setup(r => r.GetByFirebaseUidAsync("abc", It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            favoritoRepoMock
                .Setup(r => r.CountByUsuarioAsync(usuario.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(3);

            var useCase = new AgregarUsuarioRestauranteFavoritoUseCase(
                favoritoRepoMock.Object,
                usuarioRepoMock.Object
                );

            await Assert.ThrowsAsync<LimiteFavoritosAlcanzadoException>(() =>
              useCase.HandleAsync("abc", Guid.NewGuid()));

        }
        
        [Fact]
        public async Task AgregarUsuarioRestauranteFavorito_CuandoYaExiste_LanzaExcepcion()
        {
            var usuario = new Usuario
            {
                Id = Guid.NewGuid(),
                FirebaseUid = "abc",
                Plan = PlanUsuario.Plus
            }; 

            var usuarioRepoMock = new Mock<IUsuarioRepository>();
            var favoritoRepoMock = new Mock<IUsuarioRestauranteFavoritoRepository>();

            usuarioRepoMock
                .Setup(r => r.GetByFirebaseUidAsync("abc", It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            favoritoRepoMock
                .Setup(r => r.CountByUsuarioAsync(usuario.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            favoritoRepoMock
                .Setup(r => r.ExistsAsync(usuario.Id, It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var useCase = new AgregarUsuarioRestauranteFavoritoUseCase(
                favoritoRepoMock.Object,
                usuarioRepoMock.Object
                );
            await Assert.ThrowsAsync<Exception>(() =>
              useCase.HandleAsync("abc", Guid.NewGuid()));
        }

        [Fact]
        public async Task EliminarFavorito_DeberiaEliminarCuandoExiste()
        {
            var usuarioId = Guid.NewGuid();
            var restauranteId = Guid.NewGuid();
            var firebaseUid = "test_uid";

            var usuario = new Usuario
            {
                Id = usuarioId,
                FirebaseUid = firebaseUid
            };

            var mockUsuarioRepo = new Mock<IUsuarioRepository>();
            var mockFavoritoRepo = new Mock<IUsuarioRestauranteFavoritoRepository>();

            mockUsuarioRepo
                .Setup(r => r.GetByFirebaseUidAsync(firebaseUid, It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            mockFavoritoRepo
                .Setup(r => r.ExistsAsync(usuarioId, restauranteId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var useCase = new AgregarUsuarioRestauranteFavoritoUseCase(
               mockFavoritoRepo.Object,
               mockUsuarioRepo.Object
           );

            await useCase.HandleAsyncDelete(firebaseUid, restauranteId, CancellationToken.None);

            mockFavoritoRepo.Verify(
                r => r.EliminarAsync(usuarioId, restauranteId, It.IsAny<CancellationToken>()),
                Times.Once
            );
        }
    }


}

