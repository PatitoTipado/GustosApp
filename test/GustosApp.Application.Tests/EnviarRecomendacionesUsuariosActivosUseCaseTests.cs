using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using GustosApp.Application.Interfaces;
using GustosApp.Application.UseCases.NotificacionUseCases;
using GustosApp.Application.UseCases.RestauranteUseCases;
using GustosApp.Application.UseCases.UsuarioUseCases;
using GustosApp.Domain.Common;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;

using Moq;

namespace GustosApp.Application.Tests
{
    public class EnviarRecomendacionesUsuariosActivosUseCaseTests
    {
        private readonly Mock<IUsuariosActivosService> _usuariosActivos;
        private readonly Mock<ICacheService> _cache;
        private readonly Mock<IRecomendadorRestaurantes> _recomendador;
        private readonly Mock<IConstruirPreferencias> _construirPreferencias;
        private readonly Mock<IEmailService> _email;
        private readonly Mock<IUsuarioRepository> _usuariosRepo;
        private readonly Mock<IServicioRestaurantes> _servicioRestaurantes;

        public EnviarRecomendacionesUsuariosActivosUseCaseTests()
        {
            _usuariosActivos = new Mock<IUsuariosActivosService>();
            _cache = new Mock<ICacheService>();
            _recomendador = new Mock<IRecomendadorRestaurantes>();
            _construirPreferencias = new Mock<IConstruirPreferencias>();
            _email = new Mock<IEmailService>();
            _usuariosRepo = new Mock<IUsuarioRepository>();
            _servicioRestaurantes = new Mock<IServicioRestaurantes>();

            // Defaults
            _recomendador
                .Setup(r => r.Handle(
                    It.IsAny<UsuarioPreferencias>(),
                    It.IsAny<List<Restaurante>>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Restaurante>());

            _construirPreferencias
                .Setup(c => c.HandleAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<Guid?>(),
                    It.IsAny<List<string>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new UsuarioPreferencias());
        }

        private EnviarRecomendacionesUsuariosActivosUseCase CreateUseCase()
        {
            return new EnviarRecomendacionesUsuariosActivosUseCase(
                _usuariosActivos.Object,
                _cache.Object,
                _recomendador.Object,
                _construirPreferencias.Object,
                _email.Object,
                _usuariosRepo.Object,
                _servicioRestaurantes.Object
            );
        }

       
        [Fact]
        public async Task HandleAsync_SinUsuariosActivos_DeberiaRetornarFalse()
        {
            _usuariosActivos.Setup(s => s.GetUsuariosActivos())
                .Returns(Array.Empty<string>());

            var useCase = CreateUseCase();

            var result = await useCase.HandleAsync("uid", CancellationToken.None);

            result.Should().BeFalse();

            _email.Verify(e => e.EnviarEmailAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
                Times.Never);
        }

       
        [Fact]
        public async Task HandleAsync_SinUbicacion_DeberiaRetornarFalse()
        {
            _usuariosActivos.Setup(s => s.GetUsuariosActivos())
                .Returns(new[] { "uid1" });

            _cache.Setup(c => c.GetAsync<UserLocation>("usuario:uid1:location"))
                .ReturnsAsync((UserLocation)null);

            var useCase = CreateUseCase();

            var result = await useCase.HandleAsync("admin", CancellationToken.None);

            result.Should().BeFalse();
        }

       
        [Fact]
        public async Task HandleAsync_SinRestaurantes_DeberiaRetornarFalse()
        {
            _usuariosActivos.Setup(s => s.GetUsuariosActivos())
                .Returns(new[] { "uid1" });

            _cache.Setup(c => c.GetAsync<UserLocation>("usuario:uid1:location"))
                .ReturnsAsync(new UserLocation(-34.6, -58.4, 1000, DateTime.UtcNow));

            _usuariosRepo.Setup(r => r.GetByFirebaseUidAsync("uid1", It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Usuario
                {
                    Id = Guid.NewGuid(),
                    FirebaseUid = "uid1",
                    Email = "user@test.com"
                });

            _servicioRestaurantes.Setup(s => s.BuscarAsync(
                3.5,
                null,
                "",
                -34.6,
                -58.4,
                1000))
                .ReturnsAsync(new List<Restaurante>());

            var useCase = CreateUseCase();

            var result = await useCase.HandleAsync("admin", CancellationToken.None);

            result.Should().BeFalse();
        }

       
        [Fact]
        public async Task HandleAsync_SinRecomendaciones_DeberiaRetornarFalse()
        {
            _usuariosActivos.Setup(s => s.GetUsuariosActivos())
                .Returns(new[] { "uid1" });

            _cache.Setup(c => c.GetAsync<UserLocation>("usuario:uid1:location"))
                .ReturnsAsync(new UserLocation(-34.6, -58.4, 1000, DateTime.UtcNow));

            var usuario = new Usuario
            {
                Id = Guid.NewGuid(),
                FirebaseUid = "uid1",
                Email = "user@test.com"
            };

            _usuariosRepo.Setup(r => r.GetByFirebaseUidAsync("uid1", It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            _servicioRestaurantes.Setup(r => r.BuscarAsync(
                    3.5,
                    null,
                    "",
                    -34.6,
                    -58.4,
                    1000))
                .ReturnsAsync(new List<Restaurante>
                {
                new Restaurante { Id = Guid.NewGuid(), Nombre = "Resto 1" }
                });

            _recomendador.Setup(r => r.Handle(
                    It.IsAny<UsuarioPreferencias>(),
                    It.IsAny<List<Restaurante>>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Restaurante>()); // No hay recomendaciones

            var useCase = CreateUseCase();

            var result = await useCase.HandleAsync("admin", CancellationToken.None);

            result.Should().BeFalse();
        }

        [Fact]
        public async Task HandleAsync_ConRecomendaciones_DeberiaEnviarEmail_YRetornarTrue()
        {
            _usuariosActivos.Setup(s => s.GetUsuariosActivos())
                .Returns(new[] { "uid1" });

            _cache.Setup(c => c.GetAsync<UserLocation>("usuario:uid1:location"))
                .ReturnsAsync(new UserLocation(-34.6, -58.4, 1000, DateTime.UtcNow));

            var usuario = new Usuario
            {
                Id = Guid.NewGuid(),
                FirebaseUid = "uid1",
                Email = "user@test.com"
            };

            _usuariosRepo.Setup(r => r.GetByFirebaseUidAsync("uid1", It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            _servicioRestaurantes.Setup(r => r.BuscarAsync(
                    3.5,
                    null,
                    "",
                    -34.6,
                    -58.4,
                    1000))
                .ReturnsAsync(new List<Restaurante>
                {
                new Restaurante { Id = Guid.NewGuid(), Nombre = "Pizza Loca" }
                });

            _recomendador.Setup(r => r.Handle(
                    It.IsAny<UsuarioPreferencias>(),
                    It.IsAny<List<Restaurante>>(),
                    1,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Restaurante>
                {
                new Restaurante { Nombre = "Pizza Loca" }
                });

            var useCase = CreateUseCase();

            var result = await useCase.HandleAsync("admin", CancellationToken.None);

            result.Should().BeTrue();

            _email.Verify(e => e.EnviarEmailAsync(
                "user@test.com",
                "Recomendación personalizada",
                It.Is<string>(body => body.Contains("Pizza Loca")),
                It.IsAny<CancellationToken>()),
                Times.Once); 
        }
    }
    }
