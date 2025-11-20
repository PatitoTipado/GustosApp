using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model.@enum;
using GustosApp.Domain.Model;

namespace GustosApp.Application.UseCases.RestauranteUseCases.SolicitudRestauranteUseCases
{
    public class CrearSolicitudRestauranteUseCase
    {
        private readonly ISolicitudRestauranteRepository _solicitudes;
        private readonly IUsuarioRepository _usuarios;

        public CrearSolicitudRestauranteUseCase(
            ISolicitudRestauranteRepository solicitudes,
            IUsuarioRepository usuarios)
        {
            _solicitudes = solicitudes;
            _usuarios = usuarios;
        }

        public async Task<Guid> HandleAsync(string firebaseUid,string Nombre, string Direccion,
            double? Latitud, double? Longitud, string? PrimaryType, string? TypesJson,
            string? HorariosJson, List<Guid>? GustosIds, List<Guid>? RestriccionesIds,
            List<string>? Platos,IEnumerable<SolicitudRestauranteImagen> Imagenes,
            CancellationToken ct = default)
        {
            var usuario = await _usuarios.GetByFirebaseUidAsync(firebaseUid, ct);
            if (usuario == null)
                throw new Exception("Usuario no encontrado");

            if (usuario.Rol != RolUsuario.Usuario)
                throw new Exception("Ya tenés una solicitud pendiente o sos dueño.");

            var solicitud = new SolicitudRestaurante
            {
                UsuarioId = usuario.Id,
                Nombre = Nombre,
                Direccion = Direccion,
                Latitud = Latitud,
                Longitud = Longitud,
                PrimaryType = PrimaryType,
                TypesJson = TypesJson ?? "[]",
                HorariosJson = HorariosJson,
                GustosIds = GustosIds ?? new(),
                RestriccionesIds = RestriccionesIds ?? new(),
                Platos = Platos ?? new(),
                Imagenes = Imagenes.Select(im => new SolicitudRestauranteImagen
                {
                    Tipo = im.Tipo,
                    Url = im.Url
                }).ToList()
            };

            // Guardar la solicitud
            await _solicitudes.AddAsync(solicitud, ct);

            // Cambiar el rol del usuario
            usuario.Rol = RolUsuario.PendienteRestaurante;

            await _usuarios.UpdateAsync(usuario, ct);

            return solicitud.Id;
        }
    }

}
