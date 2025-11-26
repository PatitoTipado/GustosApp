using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using GustosApp.Application.UseCases.UsuarioUseCases;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using Moq;

namespace GustosApp.Application.Tests
{
    public class ObtenerRestaurantesFavoritosUseCaseTests
    {

        private readonly Mock<IUsuarioRepository> _usuarios;
        private readonly Mock<IUsuarioRestauranteFavoritoRepository> _favoritos;

        private readonly ObtenerRestaurantesFavoritosUseCase _useCase;

        public ObtenerRestaurantesFavoritosUseCaseTests()
        {
            _usuarios = new Mock<IUsuarioRepository>();
            _favoritos = new Mock<IUsuarioRestauranteFavoritoRepository>();

            _useCase = new ObtenerRestaurantesFavoritosUseCase(
                _usuarios.Object,
                _favoritos.Object
            );
        }

        // Helpers
        private Usuario FakeUser(Guid id) => new Usuario
        {
            Id = id,
            FirebaseUid = "uid123",
            Nombre = "Gonza"
        };

        private Restaurante FakeRest(string name) => new Restaurante
        {
            Id = Guid.NewGuid(),
            Nombre = name,
            Rating = 4.7
        };

     

        [Fact]
        public async Task HandleAsync_UsuarioNoEncontrado_DeberiaLanzarExcepcion()
        {
            _usuarios.Setup(r => r.GetByFirebaseUidAsync("uid", default))
                .ReturnsAsync((Usuario?)null);

            Func<Task> act = () => _useCase.HandleAsync("uid", default);

            await act.Should().ThrowAsync<Exception>()
                .WithMessage("Usuario no encontrado");
        }

  
        [Fact]
        public async Task HandleAsync_ConFavoritos_DeberiaRetornarLista()
        {
            var user = FakeUser(Guid.NewGuid());

            var lista = new List<Restaurante>
        {
            FakeRest("Resto 1"),
            FakeRest("Resto 2")
        };

            _usuarios.Setup(r => r.GetByFirebaseUidAsync("uid", default))
                .ReturnsAsync(user);

            _favoritos.Setup(r => r.GetFavoritosByUsuarioAsync(user.Id, default))
                .ReturnsAsync(lista);

            var result = await _useCase.HandleAsync("uid", default);

            result.Should().HaveCount(2);
            result[0].Nombre.Should().Be("Resto 1");
            result[1].Nombre.Should().Be("Resto 2");
        }

       

        [Fact]
        public async Task HandleAsync_SinFavoritos_DeberiaDevolverListaVacia()
        {
            var user = FakeUser(Guid.NewGuid());

            _usuarios.Setup(r => r.GetByFirebaseUidAsync("uid", default))
                .ReturnsAsync(user);

            _favoritos.Setup(r => r.GetFavoritosByUsuarioAsync(user.Id, default))
                .ReturnsAsync(new List<Restaurante>());

            var result = await _useCase.HandleAsync("uid", default);

            result.Should().BeEmpty();
        }

      

        [Fact]
        public async Task HandleAsync_DeberiaLlamarRepositoriosConParametrosCorrectos()
        {
            var user = FakeUser(Guid.NewGuid());

            _usuarios.Setup(r => r.GetByFirebaseUidAsync("uid", default))
                .ReturnsAsync(user);

            _favoritos.Setup(r => r.GetFavoritosByUsuarioAsync(user.Id, default))
                .ReturnsAsync(new List<Restaurante>());

            await _useCase.HandleAsync("uid", default);

            _usuarios.Verify(r => r.GetByFirebaseUidAsync("uid", default), Times.Once);
            _favoritos.Verify(r => r.GetFavoritosByUsuarioAsync(user.Id, default), Times.Once);
        }
    }
}
