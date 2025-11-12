using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Microsoft.Extensions.Logging;
using GustosApp.Application.Interfaces;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using GustosApp.Application.Tests.mocks;



    public class NotificacionesInteligentesServiceTests
    {
        [Fact]
        public async Task GenerarRecomendacionesPersonalizadasAsync_DeberiaEnviarNotificacion_CuandoHayRecomendaciones()
        {
            // Arrange
            var mockUsuarios = new Mock<IUsuarioRepository>();
            var mockRestaurantes = new Mock<IRestauranteRepository>();
            var mockNotificaciones = new Mock<INotificacionRepository>();
            var mockRealtime = new Mock<INotificacionRealtimeService>();
            var logger = new Mock<ILogger<NotificacionesInteligentesService>>();

            var usuario = new Usuario
            {
                Id = Guid.NewGuid(),
                FirebaseUid = "abc123",
                Gustos = new List<Gusto> { new Gusto { Nombre = "Pizza" } }
            };

            mockUsuarios.Setup(r => r.GetAllAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new List<Usuario> { usuario });

            var obtenerGustosFake = new ObtenerGustosUseCaseFake();
            var sugerirGustosFake = new SugerirGustosUseCaseFake();

            var service = new NotificacionesInteligentesService(
                mockUsuarios.Object,
                mockRestaurantes.Object,
                mockNotificaciones.Object,
                mockRealtime.Object,
                obtenerGustosFake,
                sugerirGustosFake,
                logger.Object
            );

            // Act
            await service.GenerarRecomendacionesPersonalizadasAsync(CancellationToken.None);

            // Assert
            mockNotificaciones.Verify(n => n.crearAsync(It.IsAny<Notificacion>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce);
            mockRealtime.Verify(r => r.EnviarNotificacionAsync(
                usuario.FirebaseUid,
                It.IsAny<string>(),
                It.IsAny<string>(),
                "Recomendacion",
                It.IsAny<CancellationToken>(),
                It.IsAny<Guid?>(),
                It.IsAny<Guid?>()),
                Times.AtLeastOnce);
        }

        [Fact]
        public async Task GenerarRecomendacionesPersonalizadasAsync_NoHaceNada_SiUsuarioNoTieneGustos()
        {
            // Arrange
            var mockUsuarios = new Mock<IUsuarioRepository>();
            var mockRestaurantes = new Mock<IRestauranteRepository>();
            var mockNotificaciones = new Mock<INotificacionRepository>();
            var mockRealtime = new Mock<INotificacionRealtimeService>();
            var logger = new Mock<ILogger<NotificacionesInteligentesService>>();

            var usuario = new Usuario
            {
                Id = Guid.NewGuid(),
                FirebaseUid = "abc123",
                Gustos = new List<Gusto>() // vacío
            };

            mockUsuarios.Setup(r => r.GetAllAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new List<Usuario> { usuario });

            var obtenerGustosFake = new ObtenerGustosUseCaseFake();
            var sugerirGustosFake = new SugerirGustosUseCaseFake();

            var service = new NotificacionesInteligentesService(
                mockUsuarios.Object,
                mockRestaurantes.Object,
                mockNotificaciones.Object,
                mockRealtime.Object,
                obtenerGustosFake,
                sugerirGustosFake,
                logger.Object
            );

            // Act
            await service.GenerarRecomendacionesPersonalizadasAsync(CancellationToken.None);

            // Assert
            mockNotificaciones.Verify(n => n.crearAsync(It.IsAny<Notificacion>(), It.IsAny<CancellationToken>()), Times.Never);
            mockRealtime.Verify(r => r.EnviarNotificacionAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<CancellationToken>(), It.IsAny<Guid?>(), It.IsAny<Guid?>()), Times.Never);
        }

        [Fact]
        public async Task GenerarRecomendacionesPersonalizadasAsync_DeberiaContinuar_SiUnUsuarioFalla()
        {
            // Arrange
            var mockUsuarios = new Mock<IUsuarioRepository>();
            var mockRestaurantes = new Mock<IRestauranteRepository>();
            var mockNotificaciones = new Mock<INotificacionRepository>();
            var mockRealtime = new Mock<INotificacionRealtimeService>();
            var logger = new Mock<ILogger<NotificacionesInteligentesService>>();

            var usuario1 = new Usuario { Id = Guid.NewGuid(), FirebaseUid = "abc123", Gustos = new List<Gusto> { new Gusto { Nombre = "Pizza" } } };
            var usuario2 = new Usuario { Id = Guid.NewGuid(), FirebaseUid = "xyz789", Gustos = new List<Gusto> { new Gusto { Nombre = "Sushi" } } };

            mockUsuarios.Setup(r => r.GetAllAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new List<Usuario> { usuario1, usuario2 });

            // Fake que lanza excepción para el primer usuario
            var obtenerGustosThrowFake = new ObtenerGustosUseCaseFakeThatThrows();
            var sugerirGustosFake = new SugerirGustosUseCaseFake();

            var service = new NotificacionesInteligentesService(
                mockUsuarios.Object,
                mockRestaurantes.Object,
                mockNotificaciones.Object,
                mockRealtime.Object,
                obtenerGustosThrowFake,
                sugerirGustosFake,
                logger.Object
            );

            // Act
            await service.GenerarRecomendacionesPersonalizadasAsync(CancellationToken.None);

            // Assert
            mockNotificaciones.Verify(n => n.crearAsync(It.IsAny<Notificacion>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce);
            mockRealtime.Verify(r => r.EnviarNotificacionAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), "Recomendacion",
                It.IsAny<CancellationToken>(), It.IsAny<Guid?>(), It.IsAny<Guid?>()), Times.AtLeastOnce);
        }
    }

