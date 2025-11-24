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
            // Arrange
            var id = Guid.NewGuid();

            _servicioRestaurantes
                .Setup(s => s.ObtenerAsync(id))
                .ReturnsAsync((Restaurante?)null);

            // Act
            Func<Task> act = async () =>
                await _useCase.HandleAsync(id, "uid123", CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<KeyNotFoundException>()
                .WithMessage("Restaurante no encontrado");
        }
        [Fact]
        public async Task HandleAsync_SinUsuarioLogueado_NoRegistraVisita_YEsFavoritoFalse()
        {
            // Arrange
            var id = Guid.NewGuid();
            var rest = FakeRestaurante(id);

            _servicioRestaurantes.Setup(s => s.ObtenerAsync(id))
                .ReturnsAsync(rest);

            // Act
            var result = await _useCase.HandleAsync(id, null, CancellationToken.None);

            // Assert
            result.Restaurante.Should().Be(rest);
            result.EsFavorito.Should().BeFalse();

            _visitas.Verify(v => v.IncrementarVisitaPerfilAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }
        [Fact]
        public async Task HandleAsync_UsuarioLogueado_DeberiaRegistrarVisita()
        {
            // Arrange
            var id = Guid.NewGuid();
            var rest = FakeRestaurante(id);

            var user = FakeUsuario(Guid.NewGuid(), "uid123");

            _servicioRestaurantes.Setup(s => s.ObtenerAsync(id))
                .ReturnsAsync(rest);

            _usuarios.Setup(r => r.GetByFirebaseUidAsync("uid123", It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            _favoritos.Setup(f => f.ExistsAsync(user.Id, id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _useCase.HandleAsync(id, "uid123", CancellationToken.None);

            // Assert
            _visitas.Verify(v => v.IncrementarVisitaPerfilAsync(id, It.IsAny<CancellationToken>()),
                Times.Once);

            result.EsFavorito.Should().BeFalse();
        }
        [Fact]
        public async Task HandleAsync_UsuarioLogueado_EsFavoritoTrue()
        {
            // Arrange
            var id = Guid.NewGuid();
            var rest = FakeRestaurante(id);
            var user = FakeUsuario(Guid.NewGuid(), "uid123");

            _servicioRestaurantes.Setup(s => s.ObtenerAsync(id))
                .ReturnsAsync(rest);

            _usuarios.Setup(r => r.GetByFirebaseUidAsync("uid123", It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            _favoritos.Setup(f => f.ExistsAsync(user.Id, id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _useCase.HandleAsync(id, "uid123", CancellationToken.None);

            // Assert
            result.EsFavorito.Should().BeTrue();
        }
        [Fact]
        public async Task HandleAsync_SinReviewsLocales_ConPlaceId_DeberiaObtenerDeGooglePlaces()
        {
            // Arrange
            var id = Guid.NewGuid();

            var rest = FakeRestaurante(id);
            rest.PlaceId = "google123";
            rest.Reviews = new List<OpinionRestaurante>(); // vacío → dispara Google

            var actualizado = FakeRestaurante(id);
            actualizado.Reviews = new List<OpinionRestaurante>
    {
        new OpinionRestaurante { Opinion = "Google!", EsImportada = true }
    };

            _servicioRestaurantes.Setup(s => s.ObtenerAsync(id))
                .ReturnsAsync(rest);

            _servicioRestaurantes.Setup(s =>
                    s.ObtenerResenasDesdeGooglePlaces(rest.PlaceId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(actualizado);

            // Act
            var result = await _useCase.HandleAsync(id, null, CancellationToken.None);

            // Assert
            result.Restaurante.Reviews.Should().HaveCount(1);
            result.Restaurante.Reviews.First().Opinion.Should().Be("Google!");
        }
        [Fact]
        public async Task HandleAsync_ConReviewsLocales_NoDeberiaLlamarGoogle()
        {
            // Arrange
            var id = Guid.NewGuid();

            var rest = FakeRestaurante(id);
            rest.PlaceId = "google123";
            rest.Reviews = new List<OpinionRestaurante>
    {
        new OpinionRestaurante { Opinion = "Local", EsImportada = false }
    };

            _servicioRestaurantes.Setup(s => s.ObtenerAsync(id))
                .ReturnsAsync(rest);

            // Act
            var result = await _useCase.HandleAsync(id, null, CancellationToken.None);

            // Assert
            _servicioRestaurantes.Verify(s =>
                s.ObtenerResenasDesdeGooglePlaces(It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }
        [Fact]
        public async Task HandleAsync_DeberiaOrdenarReviews_Correctamente()
        {
            // Arrange
            var id = Guid.NewGuid();
            var rest = FakeRestaurante(id);

            var baseTime = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc);

            rest.Reviews = new List<OpinionRestaurante>
    {
        new OpinionRestaurante { EsImportada = false, FechaCreacion = baseTime.AddMinutes(-10) },
        new OpinionRestaurante { EsImportada = true,  FechaCreacion = baseTime.AddMinutes(-5)  },
        new OpinionRestaurante { EsImportada = false, FechaCreacion = baseTime                 }
    };

            _servicioRestaurantes.Setup(s => s.ObtenerAsync(id))
                .ReturnsAsync(rest);

            // Act
            var result = await _useCase.HandleAsync(id, null, CancellationToken.None);

            // Assert
            var ordered = result.Restaurante.Reviews.ToList();

            ordered[0].EsImportada.Should().BeFalse(); // local más reciente
            ordered[1].EsImportada.Should().BeFalse(); // local antigua
            ordered[2].EsImportada.Should().BeTrue();  // importada
        }

    }

    }
