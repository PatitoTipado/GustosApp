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



        public AprobarSolicitudRestauranteUseCase(ISolicitudRestauranteRepository solicitudes,
             IRestauranteRepository restaurantes, IUsuarioRepository usuarios,
             IOcrService ocr, IMenuParser menuParser, IRestriccionRepository restricciones,
            IGustoRepository gustos, IRestauranteMenuRepository menuRepo)
        {
            _solicitudes = solicitudes;
            _restaurantes = restaurantes;
            _usuarios = usuarios;
            _ocr = ocr;
            _menuParser = menuParser;
            _restricciones = restricciones;
            _gustos = gustos;
            _menuRepo = menuRepo;
        }


        public async Task<Restaurante> HandleAsync(Guid solicitudId, CancellationToken ct)
        {
            var solicitud = await _solicitudes.GetByIdAsync(solicitudId, ct)
                ?? throw new Exception("Solicitud no encontrada");

            // 1. Crear restaurante
            var restaurante = await CrearRestauranteDesdeSolicitud(solicitud, ct);

            // 2. Intentar OCR
            await ProcesarMenuOCR(solicitud, restaurante, ct);

            // 3. Actualizar rol usuario
            solicitud.Usuario.Rol = RolUsuario.DuenoRestaurante;
            await _usuarios.UpdateAsync(solicitud.Usuario, ct);

            // 4. Actualizar solicitud
            solicitud.Estado = EstadoSolicitudRestaurante.Aprobada;
            await _solicitudes.UpdateAsync(solicitud, ct);

            return restaurante;
        }

        private async Task<Restaurante> CrearRestauranteDesdeSolicitud(
       SolicitudRestaurante solicitud,
       CancellationToken ct)
        {
            if (solicitud is null)
                throw new ArgumentNullException(nameof(solicitud));

            // ================
            // CREAR RESTAURANTE BASE
            // ================
            var restaurante = new Restaurante
            {
                DuenoId = solicitud.UsuarioId,
                Nombre = solicitud.Nombre,
                NombreNormalizado = solicitud.Nombre.ToLower().Trim(),
                Direccion = solicitud.Direccion,
                Latitud = solicitud.Latitud ?? 0,
                Longitud = solicitud.Longitud ?? 0,
                PrimaryType = solicitud.PrimaryType ?? "restaurant",
                TypesJson = solicitud.TypesJson ?? "[]",
                HorariosJson = solicitud.HorariosJson ?? "{}",
                CreadoUtc = DateTime.UtcNow,
                ActualizadoUtc = DateTime.UtcNow
            };

            await _restaurantes.AddAsync(restaurante, ct);

            // ================
            // RELACIONES: GUSTOS
            // ================
            var gustos = await _gustos.GetByIdsAsync(solicitud.GustosIds, ct);
            restaurante.SetGustos(gustos);



            // ================
            // RELACIONES: RESTRICCIONES
            // ================
            var restricciones = await _restricciones.GetRestriccionesByIdsAsync(solicitud.RestriccionesIds, ct);
            restaurante.SetRestricciones(restricciones);

            // ================
            // COPIAR IMÁGENES A RESTAURANTE
            // ================
            var imgDest = solicitud.Imagenes.FirstOrDefault(i => i.Tipo == TipoImagenSolicitud.Destacada);
            var imgLogo = solicitud.Imagenes.FirstOrDefault(i => i.Tipo == TipoImagenSolicitud.Logo);
            var imgMenu = solicitud.Imagenes.FirstOrDefault(i => i.Tipo == TipoImagenSolicitud.Menu);

            restaurante.ImagenUrl = imgDest?.Url;
            restaurante.LogoUrl = imgLogo?.Url;

            // IMÁGENES INTERIOR + COMIDA (colección)
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


            return restaurante;
        }



        private async Task ProcesarMenuOCR(
       SolicitudRestaurante solicitud,
       Restaurante restaurante,
       CancellationToken ct)
        {
            var imgMenu = solicitud.Imagenes
                .FirstOrDefault(i => i.Tipo == TipoImagenSolicitud.Menu);

            if (imgMenu == null || string.IsNullOrWhiteSpace(imgMenu.Url))
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
                    restaurante.MenuError = "El texto del menú está vacío.";
                    return;
                }

                var menuJson = await _menuParser.ParsearAsync(texto, "ARS", ct);

                var existente = await _menuRepo.GetByRestauranteIdAsync(restaurante.Id, ct);

                if (existente == null)
                {
                    var restauranteMenu = new RestauranteMenu
                    {
                        RestauranteId = restaurante.Id,
                        Moneda = "ARS",
                        Json = menuJson,
                        Version = 1,
                        FechaActualizacionUtc = DateTime.UtcNow
                    };

                    await _menuRepo.AddAsync(restauranteMenu, ct);
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
                restaurante.MenuError = $"Error al procesar OCR: {ex.Message}";
            }

            restaurante.ActualizadoUtc = DateTime.UtcNow;

            // ahora delegás el guardado al repositorio de restaurantes
            await _restaurantes.UpdateAsync(restaurante, ct);
        }

    }
    }
