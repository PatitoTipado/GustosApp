using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using GustosApp.Application.Interfaces;
using GustosApp.Application.UseCases.RestauranteUseCases.SolicitudRestauranteUseCases;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model.@enum;
using GustosApp.Domain.Model;
using Moq;

namespace GustosApp.Application.Tests
{
    public class AprobarSolicitudRestauranteUseCaseTests
    {
        private readonly Mock<ISolicitudRestauranteRepository> _solicitudes;
        private readonly Mock<IRestauranteRepository> _restaurantes;
        private readonly Mock<IUsuarioRepository> _usuarios;
        private readonly Mock<IOcrService> _ocr;
        private readonly Mock<IMenuParser> _menuParser;
        private readonly Mock<IRestriccionRepository> _restricciones;
        private readonly Mock<IGustoRepository> _gustos;
        private readonly Mock<IRestauranteMenuRepository> _menuRepo;
        private readonly Mock<IFirebaseAuthService> _firebase;
        private readonly Mock<IEmailService> _email;
        private readonly Mock<IEmailTemplateService> _templates;
        private readonly Mock<IHttpDownloader> _downloader;
        private readonly AprobarSolicitudRestauranteUseCase _useCase;

        public AprobarSolicitudRestauranteUseCaseTests()
        {
            _solicitudes = new Mock<ISolicitudRestauranteRepository>();
            _restaurantes = new Mock<IRestauranteRepository>();
            _usuarios = new Mock<IUsuarioRepository>();
            _ocr = new Mock<IOcrService>();
            _menuParser = new Mock<IMenuParser>();
            _restricciones = new Mock<IRestriccionRepository>();
            _gustos = new Mock<IGustoRepository>();
            _menuRepo = new Mock<IRestauranteMenuRepository>();
            _firebase = new Mock<IFirebaseAuthService>();
            _email = new Mock<IEmailService>();
            _templates = new Mock<IEmailTemplateService>();
            _downloader = new Mock<IHttpDownloader>();

            _useCase = new AprobarSolicitudRestauranteUseCase(
                _solicitudes.Object,
                _restaurantes.Object,
                _usuarios.Object,
                _ocr.Object,
                _menuParser.Object,
                _restricciones.Object,
                _gustos.Object,
                _menuRepo.Object,
                _firebase.Object,
                _email.Object,
                _templates.Object,
                _downloader.Object

            );
        }
        private Usuario FakeUsuario(Guid id)
            => new Usuario
            {
                Id = id,
                FirebaseUid = "firebase-uid",
                Nombre = "Gonza",
                Email = "user@test.com",
                Rol = RolUsuario.Usuario
            };

        private SolicitudRestaurante FakeSolicitudConMenu(Guid idSolicitud, Usuario usuario, bool conMenu)
        {
            var solicitud = new SolicitudRestaurante
            {
                Id = idSolicitud,
                UsuarioId = usuario.Id,
                Usuario = usuario,
                Nombre = "Mi Resto",
                Direccion = "Calle Falsa 123",
                Latitud = 1.23,
                Longitud = 4.56,
                HorariosJson = "{\"lunes\":\"10-22\"}",
                WebsiteUrl = "https://mi-resto.com",
                GustosIds = new List<Guid> { Guid.NewGuid() },
                RestriccionesIds = new List<Guid> { Guid.NewGuid() },
                Imagenes = new List<SolicitudRestauranteImagen>
            {
                new SolicitudRestauranteImagen { Tipo = TipoImagenSolicitud.Destacada, Url="https://img.com/d.jpg" },
                new SolicitudRestauranteImagen { Tipo = TipoImagenSolicitud.Logo, Url="https://img.com/l.jpg" },
                new SolicitudRestauranteImagen { Tipo = TipoImagenSolicitud.Interior, Url="https://img.com/i.jpg" },
                new SolicitudRestauranteImagen { Tipo = TipoImagenSolicitud.Comida, Url="https://img.com/c.jpg" },
            },
                Estado = EstadoSolicitudRestaurante.Pendiente,
                FechaCreacion = DateTime.UtcNow
            };

            if (conMenu)
            {
                solicitud.Imagenes.Add(new SolicitudRestauranteImagen
                {
                    Tipo = TipoImagenSolicitud.Menu,
                    Url = "https://img.com/menu.jpg" // luego lo reemplazamos por una válida
                });
            }

            return solicitud;
        }

        private void SetupGustosYRestricciones(SolicitudRestaurante solicitud)
        {
            _gustos.Setup(g => g.GetByIdsAsync(solicitud.GustosIds, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Gusto> { new Gusto { Id = solicitud.GustosIds[0] } });

            _restricciones.Setup(r => r.GetRestriccionesByIdsAsync(solicitud.RestriccionesIds, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Restriccion> { new Restriccion { Id = solicitud.RestriccionesIds[0] } });
        }

        [Fact]
        public async Task HandleAsync_DeberiaAprobarSolicitud_YCrearRestaurante_YMenuNuevo()
        {
            var idSolicitud = Guid.NewGuid();
            var usuario = FakeUsuario(Guid.NewGuid());
            var solicitud = FakeSolicitudConMenu(idSolicitud, usuario, true);

            // Ahora no importa la URL, nunca se usa realmente
            solicitud.Imagenes.First(i => i.Tipo == TipoImagenSolicitud.Menu).Url = "fake://menu";

            _solicitudes.Setup(r => r.GetByIdAsync(idSolicitud, It.IsAny<CancellationToken>()))
                .ReturnsAsync(solicitud);

            SetupGustosYRestricciones(solicitud);

            Restaurante? restCapturado = null;

            _restaurantes.Setup(r => r.AddAsync(It.IsAny<Restaurante>(), It.IsAny<CancellationToken>()))
                .Callback<Restaurante, CancellationToken>((r, _) => restCapturado = r)
                .Returns(Task.CompletedTask);

            _restaurantes.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // === ✔ MOCK HTTP COMPLETAMENTE ===
            _downloader.Setup(d => d.DownloadAsync("fake://menu", It.IsAny<CancellationToken>()))
                .ReturnsAsync(new byte[] { 1, 2, 3 }); // cualquier byte array

            // OCR válido
            _ocr.Setup(o => o.ReconocerTextoAsync(It.IsAny<IEnumerable<Stream>>(), "spa+eng", It.IsAny<CancellationToken>()))
                .ReturnsAsync("texto del menú");

            // Parser válido
            _menuParser.Setup(p => p.ParsearAsync("texto del menú", "ARS", It.IsAny<CancellationToken>()))
                .ReturnsAsync("{\"parsed\":true}");

            // No existe menú previo
            _menuRepo.Setup(m => m.GetByRestauranteIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((RestauranteMenu?)null);

            _templates.Setup(t => t.Render("SolicitudAprobada.html", It.IsAny<Dictionary<string, string>>()))
                .Returns("HTML");

            _email.Setup(e => e.EnviarEmailAsync(usuario.Email, "Tu solicitud fue aprobada",
                "HTML", CancellationToken.None))
                .Returns(Task.CompletedTask);

            _firebase.Setup(f => f.SetUserRoleAsync(usuario.FirebaseUid, RolUsuario.DuenoRestaurante.ToString()))
                .Returns(Task.CompletedTask);

            // === ACT ===
            var res = await _useCase.HandleAsync(idSolicitud, CancellationToken.None);

            // === ASSERT ===
            res.MenuProcesado.Should().BeTrue();
            res.MenuError.Should().BeNull();

            _menuRepo.Verify(m => m.AddAsync(It.IsAny<RestauranteMenu>(), It.IsAny<CancellationToken>()), Times.Once);
            _menuRepo.Verify(m => m.UpdateAsync(It.IsAny<RestauranteMenu>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task HandleAsync_OcrTextoVacio_DeberiaMarcarMenuVacio_YNoCrearMenu()
        {
            var idSolicitud = Guid.NewGuid();
            var usuario = FakeUsuario(Guid.NewGuid());
            var solicitud = FakeSolicitudConMenu(idSolicitud, usuario, true);

            // No importa URL real, no se usa
            solicitud.Imagenes.First(i => i.Tipo == TipoImagenSolicitud.Menu).Url = "fake://menu";

            _solicitudes.Setup(r => r.GetByIdAsync(idSolicitud, It.IsAny<CancellationToken>()))
                .ReturnsAsync(solicitud);

            SetupGustosYRestricciones(solicitud);

          
            _downloader.Setup(d => d.DownloadAsync("fake://menu", It.IsAny<CancellationToken>()))
                .ReturnsAsync(new byte[] { 1, 2, 3 });

         
            _ocr.Setup(o => o.ReconocerTextoAsync(It.IsAny<IEnumerable<Stream>>(), "spa+eng", It.IsAny<CancellationToken>()))
                .ReturnsAsync(string.Empty);

       
            _menuRepo.Setup(m => m.GetByRestauranteIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((RestauranteMenu?)null);

            _templates.Setup(t => t.Render("SolicitudAprobada.html", It.IsAny<Dictionary<string, string>>()))
                .Returns("HTML");

            _email.Setup(e => e.EnviarEmailAsync(It.IsAny<string>(), "Tu solicitud fue aprobada",
                "HTML", CancellationToken.None))
                .Returns(Task.CompletedTask);

            _firebase.Setup(f => f.SetUserRoleAsync(usuario.FirebaseUid, RolUsuario.DuenoRestaurante.ToString()))
                .Returns(Task.CompletedTask);

            var result = await _useCase.HandleAsync(idSolicitud, CancellationToken.None);

            // === ASSERT ===
            result.MenuProcesado.Should().BeFalse();
            result.MenuError.Should().Be("Menú vacío");

            // NO debe crearse ningún menú
            _menuRepo.Verify(m => m.AddAsync(It.IsAny<RestauranteMenu>(), It.IsAny<CancellationToken>()), Times.Never);
            _menuRepo.Verify(m => m.UpdateAsync(It.IsAny<RestauranteMenu>(), It.IsAny<CancellationToken>()), Times.Never);
        }


        [Fact]
        public async Task HandleAsync_ConMenuExistente_DeberiaActualizarMenu()
        {
            var idSolicitud = Guid.NewGuid();
            var usuario = FakeUsuario(Guid.NewGuid());
            var solicitud = FakeSolicitudConMenu(idSolicitud, usuario, true);

            solicitud.Imagenes.First(i => i.Tipo == TipoImagenSolicitud.Menu).Url =
                "https://httpbin.org/image/jpeg";

            _solicitudes.Setup(r => r.GetByIdAsync(idSolicitud, It.IsAny<CancellationToken>()))
                .ReturnsAsync(solicitud);

            SetupGustosYRestricciones(solicitud);

            Restaurante? rest = null;

            _restaurantes.Setup(r => r.AddAsync(It.IsAny<Restaurante>(), It.IsAny<CancellationToken>()))
                .Callback<Restaurante, CancellationToken>((x, _) => rest = x)
                .Returns(Task.CompletedTask);

            _restaurantes.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _ocr.Setup(o =>
                o.ReconocerTextoAsync(It.IsAny<IEnumerable<Stream>>(), "spa+eng", It.IsAny<CancellationToken>()))
                .ReturnsAsync("texto ocr");

            _menuParser.Setup(p => p.ParsearAsync("texto ocr", "ARS", It.IsAny<CancellationToken>()))
                .ReturnsAsync("{\"menu\":true}");

            var menuExistente = new RestauranteMenu
            {
                RestauranteId = Guid.NewGuid(),
                Json = "{}",
                Version = 1,
                Moneda = "ARS",
                FechaActualizacionUtc = DateTime.UtcNow.AddDays(-1)
            };

            _menuRepo.Setup(m => m.GetByRestauranteIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(menuExistente);

            _menuRepo.Setup(m => m.UpdateAsync(menuExistente, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _templates.Setup(t => t.Render("SolicitudAprobada.html", It.IsAny<Dictionary<string, string>>()))
                .Returns("HTML");

            _email.Setup(e => e.EnviarEmailAsync(usuario.Email, "Tu solicitud fue aprobada",
                "HTML", CancellationToken.None))
                .Returns(Task.CompletedTask);

            _firebase.Setup(f => f.SetUserRoleAsync(usuario.FirebaseUid, RolUsuario.DuenoRestaurante.ToString()))
                .Returns(Task.CompletedTask);

            var result = await _useCase.HandleAsync(idSolicitud, CancellationToken.None);

            result.MenuProcesado.Should().BeTrue();
            result.MenuError.Should().BeNull();

            menuExistente.Json.Should().Be("{\"menu\":true}");
            menuExistente.Version.Should().Be(2);

            _menuRepo.Verify(m => m.UpdateAsync(menuExistente, It.IsAny<CancellationToken>()), Times.Once);
            _menuRepo.Verify(m => m.AddAsync(It.IsAny<RestauranteMenu>(), It.IsAny<CancellationToken>()), Times.Never);
        }

     
        [Fact]
        public async Task HandleAsync_ExcepcionEnOcr_DeberiaMarcarMenuError_YNoLanzar()
        {
            var idSolicitud = Guid.NewGuid();
            var usuario = FakeUsuario(Guid.NewGuid());
            var solicitud = FakeSolicitudConMenu(idSolicitud, usuario, true);

            solicitud.Imagenes.First(i => i.Tipo == TipoImagenSolicitud.Menu).Url =
                "https://httpbin.org/image/jpeg";

            _solicitudes.Setup(r => r.GetByIdAsync(idSolicitud, It.IsAny<CancellationToken>()))
                .ReturnsAsync(solicitud);

            SetupGustosYRestricciones(solicitud);

            Restaurante? rest = null;

            _restaurantes.Setup(r => r.AddAsync(It.IsAny<Restaurante>(), It.IsAny<CancellationToken>()))
                .Callback<Restaurante, CancellationToken>((x, _) => rest = x)
                .Returns(Task.CompletedTask);

            _ocr.Setup(o =>
                o.ReconocerTextoAsync(It.IsAny<IEnumerable<Stream>>(), "spa+eng", It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Error en OCR"));

            _restaurantes.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _templates.Setup(t => t.Render("SolicitudAprobada.html", It.IsAny<Dictionary<string, string>>()))
                .Returns("HTML");

            _email.Setup(e => e.EnviarEmailAsync(usuario.Email, "Tu solicitud fue aprobada", "HTML"
                , CancellationToken.None))
                .Returns(Task.CompletedTask);

            _firebase.Setup(f => f.SetUserRoleAsync(usuario.FirebaseUid, RolUsuario.DuenoRestaurante.ToString()))
                .Returns(Task.CompletedTask);

            var result = await _useCase.HandleAsync(idSolicitud, CancellationToken.None);

            result.MenuProcesado.Should().BeFalse();
            result.MenuError.Should().Be("Error en OCR");

            _menuRepo.Verify(m => m.AddAsync(It.IsAny<RestauranteMenu>(), It.IsAny<CancellationToken>()), Times.Never);
            _menuRepo.Verify(m => m.UpdateAsync(It.IsAny<RestauranteMenu>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
