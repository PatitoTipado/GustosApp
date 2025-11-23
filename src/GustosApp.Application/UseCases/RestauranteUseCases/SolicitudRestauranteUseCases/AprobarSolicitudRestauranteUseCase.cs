using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GustosApp.Application.Interfaces;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model.@enum;
using GustosApp.Domain.Model;

namespace GustosApp.Application.UseCases.RestauranteUseCases.SolicitudRestauranteUseCases
{
    public class AprobarSolicitudRestauranteUseCase
    {
        private readonly ISolicitudRestauranteRepository _solicitudes;
        private readonly IRestauranteRepository _restaurantes;
        private readonly IUsuarioRepository _usuarios;
        private readonly IOcrService _ocr;
        private readonly IMenuParser _menuParser;
        private readonly IRestriccionRepository _restricciones;
        private readonly IGustoRepository _gustos;
        private readonly IRestauranteMenuRepository _menuRepo;
        private readonly IFirebaseAuthService _firebase;
        private readonly IEmailService _email;
        private readonly IEmailTemplateService _templates;


        public AprobarSolicitudRestauranteUseCase(ISolicitudRestauranteRepository solicitudes,
             IRestauranteRepository restaurantes, IUsuarioRepository usuarios,
             IOcrService ocr, IMenuParser menuParser, IRestriccionRepository restricciones,
            IGustoRepository gustos, IRestauranteMenuRepository menuRepo, 
            IFirebaseAuthService firebase, IEmailService email, 
            IEmailTemplateService templates
            )
        {
            _solicitudes = solicitudes;
            _restaurantes = restaurantes;
            _usuarios = usuarios;
            _ocr = ocr;
            _menuParser = menuParser;
            _restricciones = restricciones;
            _gustos = gustos;
            _menuRepo = menuRepo;
            _firebase = firebase;
            _email = email;
            _templates = templates;
        }


        public async Task<Restaurante> HandleAsync(Guid solicitudId, CancellationToken ct)
        {
            var solicitud = await _solicitudes.GetByIdAsync(solicitudId, ct)
                ?? throw new Exception("Solicitud no encontrada");

            var restaurante = await CrearRestauranteDesdeSolicitud(solicitud, ct);

            await ProcesarMenuOCR(solicitud, restaurante, ct);

            solicitud.Usuario.Rol = RolUsuario.DuenoRestaurante;
            solicitud.Estado = EstadoSolicitudRestaurante.Aprobada;

            await _firebase.SetUserRoleAsync(solicitud.Usuario.FirebaseUid, RolUsuario.DuenoRestaurante.ToString());

            await _restaurantes.SaveChangesAsync(ct);


            //modifcar para deploy
            await _email.EnviarEmailAsync(
                solicitud.Usuario.Email,
                "Tu solicitud fue aprobada",
             _templates.Render("SolicitudAprobada.html", new Dictionary<string, string>
              {
             { "USUARIO", solicitud.Usuario.Nombre },
            { "NOMBRE", restaurante.Nombre },
            { "LINK", $"http://localhost:3000/restaurante/panel/{restaurante.Id}" }
             })
            );


            return restaurante;
        }



        private async Task<Restaurante> CrearRestauranteDesdeSolicitud(
        SolicitudRestaurante solicitud,
        CancellationToken ct)
        {
            var restaurante = new Restaurante
            {
                PropietarioUid= solicitud.UsuarioId.ToString(),
                DuenoId = solicitud.UsuarioId,
                Nombre = solicitud.Nombre,
                NombreNormalizado = solicitud.Nombre.ToLower().Trim(),
                Direccion = solicitud.Direccion,
                Latitud = solicitud.Latitud ?? 0,
                Longitud = solicitud.Longitud ?? 0,
                PrimaryType = "Restaurante",
                TypesJson = "",
                HorariosJson = solicitud.HorariosJson ?? "{}",
                CreadoUtc = DateTime.UtcNow,
                ActualizadoUtc = DateTime.UtcNow,
                WebUrl = solicitud.WebsiteUrl,
                Rating= 4.0,
            };

            // Relaciones
            restaurante.SetGustos(await _gustos.GetByIdsAsync(solicitud.GustosIds, ct));
            restaurante.SetRestricciones(await _restricciones.GetRestriccionesByIdsAsync(solicitud.RestriccionesIds, ct));

            // Imagen principal y logo
            restaurante.ImagenUrl = solicitud.Imagenes.FirstOrDefault(i => i.Tipo == TipoImagenSolicitud.Destacada)?.Url;
            restaurante.LogoUrl = solicitud.Imagenes.FirstOrDefault(i => i.Tipo == TipoImagenSolicitud.Logo)?.Url;

            // Imágenes múltiples
            restaurante.Imagenes = solicitud.Imagenes
                .Where(i => i.Tipo == TipoImagenSolicitud.Interior || i.Tipo == TipoImagenSolicitud.Comida)
                .Select(i => new RestauranteImagen
                {
                    RestauranteId = restaurante.Id,
                    Tipo = i.Tipo == TipoImagenSolicitud.Interior
                        ? TipoImagenRestaurante.Interior
                        : TipoImagenRestaurante.Comida,
                    Url = i.Url,
                    FechaCreacionUtc = DateTime.UtcNow
                })
                .ToList();

            await _restaurantes.AddAsync(restaurante, ct);

            return restaurante;
        }




        private async Task ProcesarMenuOCR(
     SolicitudRestaurante solicitud,
     Restaurante restaurante,
     CancellationToken ct)
        {
            var imgMenu = solicitud.Imagenes
                .FirstOrDefault(i => i.Tipo == TipoImagenSolicitud.Menu);

            foreach (var img in solicitud.Imagenes)
            {
                Console.WriteLine($"IMG | Tipo={img.Tipo} | Url={img.Url}");
            }

            if (imgMenu == null)
                return;

            try
            {
                using var http = new HttpClient();
                var bytes = await http.GetByteArrayAsync(imgMenu.Url, ct);
                using var stream = new MemoryStream(bytes);

                var texto = await _ocr.ReconocerTextoAsync(new[] { stream }, "spa+eng", ct);

                if (string.IsNullOrWhiteSpace(texto))
                {
                    restaurante.MenuProcesado = false;
                    restaurante.MenuError = "Menú vacío";
                    return;
                }

                var menuJson = await _menuParser.ParsearAsync(texto, "ARS", ct);

                var existente = await _menuRepo.GetByRestauranteIdAsync(restaurante.Id, ct);

                if (existente == null)
                {
                    await _menuRepo.AddAsync(new RestauranteMenu
                    {
                        RestauranteId = restaurante.Id,
                        Moneda = "ARS",
                        Json = menuJson,
                        Version = 1,
                        FechaActualizacionUtc = DateTime.UtcNow
                    }, ct);
                }
                else
                {
                    existente.Json = menuJson;
                    existente.Version++;
                    existente.FechaActualizacionUtc = DateTime.UtcNow;

                    await _menuRepo.UpdateAsync(existente, ct);
                }

                restaurante.MenuProcesado = true;
                restaurante.MenuError = null;
            }
            catch (Exception ex)
            {
                restaurante.MenuProcesado = false;
                restaurante.MenuError = ex.Message;
            }
        }

    }
}
