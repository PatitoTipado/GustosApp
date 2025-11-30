using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using GustosApp.Application.Interfaces;
using GustosApp.Application.UseCases.RestauranteUseCases;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using Moq;

namespace GustosApp.Application.Tests
{
    public class ObtenerRestauranteDetalleUseCaseTests
    {
        private readonly Mock<IServicioRestaurantes> _servicioRestaurantes;
        private readonly Mock<IRestauranteEstadisticasRepository> _visitas;
        private readonly Mock<IUsuarioRestauranteFavoritoRepository> _favoritos;
        private readonly Mock<IUsuarioRepository> _usuarios;

        private readonly ObtenerRestauranteDetalleUseCase _useCase;

        public ObtenerRestauranteDetalleUseCaseTests()
        {
            _servicioRestaurantes = new();
            _visitas = new();
            _favoritos = new();
            _usuarios = new();

            _useCase = new ObtenerRestauranteDetalleUseCase(
                _servicioRestaurantes.Object,
                _visitas.Object,
                _favoritos.Object,
                _usuarios.Object
            );
        }

        private Restaurante FakeRestaurante(Guid id)
            => new Restaurante
            {
                Id = id,
                Nombre = "Test Resto",
                Reviews = new List<OpinionRestaurante>()
            };

        private Usuario FakeUsuario(Guid id, string firebase)
            => new Usuario
            {
                Id = id,
                FirebaseUid = firebase,
                Nombre = "Gonza"
            };



        [Fact]
        public async Task HandleAsync_RestauranteNoExiste_DeberiaLanzar()
        {
            var id = Guid.NewGuid();

            _servicioRestaurantes
                .Setup(s => s.ObtenerAsync(id))
                .ReturnsAsync((Restaurante?)null);

            Func<Task> act = async () =>
                await _useCase.HandleAsync(id, "uid123", CancellationToken.None);

            await act.Should()
                .ThrowAsync<KeyNotFoundException>()
                .WithMessage("Restaurante no encontrado");
        }

     
        [Fact]
        public async Task HandleAsync_SinUsuarioLogueado_NoRegistraVisita_YEsFavoritoFalse()
        {
            var id = Guid.NewGuid();
            var rest = FakeRestaurante(id);

            _servicioRestaurantes.Setup(s => s.ObtenerAsync(id))
                .ReturnsAsync(rest);

            var result = await _useCase.HandleAsync(id, null, CancellationToken.None);

            result.Restaurante.Should().Be(rest);
            result.EsFavorito.Should().BeFalse();

            _visitas.Verify(v => v.IncrementarVisitaPerfilAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }



        [Fact]
        public async Task HandleAsync_UsuarioLogueado_DeberiaRegistrarVisita()
        {
            var id = Guid.NewGuid();
            var rest = FakeRestaurante(id);
            var user = FakeUsuario(Guid.NewGuid(), "uid123");

            _servicioRestaurantes.Setup(s => s.ObtenerAsync(id))
                .ReturnsAsync(rest);

            _usuarios.Setup(r => r.GetByFirebaseUidAsync("uid123", It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            _favoritos.Setup(f => f.ExistsAsync(user.Id, id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var result = await _useCase.HandleAsync(id, "uid123", CancellationToken.None);

            _visitas.Verify(v => v.IncrementarVisitaPerfilAsync(id, It.IsAny<CancellationToken>()),
                Times.Once);

            result.EsFavorito.Should().BeFalse();
        }



        [Fact]
        public async Task HandleAsync_UsuarioLogueado_EsFavoritoTrue()
        {
            var id = Guid.NewGuid();
            var rest = FakeRestaurante(id);
            var user = FakeUsuario(Guid.NewGuid(), "uid123");

            _servicioRestaurantes.Setup(s => s.ObtenerAsync(id))
                .ReturnsAsync(rest);

            _usuarios.Setup(r => r.GetByFirebaseUidAsync("uid123", It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            _favoritos.Setup(f => f.ExistsAsync(user.Id, id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var result = await _useCase.HandleAsync(id, "uid123", CancellationToken.None);

            result.EsFavorito.Should().BeTrue();
        }



        [Fact]
        public async Task HandleAsync_ConReviewsLocales_NoDeberiaLlamarGoogle()
        {
            var id = Guid.NewGuid();

            var rest = FakeRestaurante(id);
            rest.PlaceId = "google123";
            rest.Reviews = new List<OpinionRestaurante>
        {
            new OpinionRestaurante { Opinion = "Local", EsImportada = false }
        };

            _servicioRestaurantes.Setup(s => s.ObtenerAsync(id))
                .ReturnsAsync(rest);

            var result = await _useCase.HandleAsync(id, null, CancellationToken.None);

            _servicioRestaurantes.Verify(s =>
                s.ActualizarReviewsDesdeGoogleLegacyAsync(
                    It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task HandleAsync_SinReviewsLocales_DeberiaLlamarGoogle()
        {
            var id = Guid.NewGuid();

            var rest = FakeRestaurante(id);
            rest.PlaceId = "google123";
            rest.Reviews = new List<OpinionRestaurante>(); // vacío → dispara Google

            var restConReviews = FakeRestaurante(id);
            restConReviews.Reviews = new List<OpinionRestaurante>
        {
            new OpinionRestaurante { Opinion = "Google!", EsImportada = true }
        };

            // Primer llamada: antes de importar
            _servicioRestaurantes.SetupSequence(s => s.ObtenerAsync(id))
                .ReturnsAsync(rest)              // primera vez → sin reviews
                .ReturnsAsync(restConReviews);   // segunda vez → con reviews importadas

            var result = await _useCase.HandleAsync(id, null, CancellationToken.None);

            _servicioRestaurantes.Verify(s =>
                s.ActualizarReviewsDesdeGoogleLegacyAsync(id, "google123", It.IsAny<CancellationToken>()),
                Times.Once);
        }
        [Fact]
        public async Task HandleAsync_SinPlaceId_NoDebeLlamarGoogle()
        {
            var id = Guid.NewGuid();

            var rest = FakeRestaurante(id);
            rest.PlaceId = null;
            rest.Reviews = new List<OpinionRestaurante>();

            _servicioRestaurantes.Setup(s => s.ObtenerAsync(id))
                .ReturnsAsync(rest);

            var result = await _useCase.HandleAsync(id, null, CancellationToken.None);

            _servicioRestaurantes.Verify(s =>
                s.ActualizarReviewsDesdeGoogleLegacyAsync(
                    It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }
        

    }

}
