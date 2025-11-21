using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GustosApp.Application.Interfaces;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model.@enum;

namespace GustosApp.Application.UseCases.RestauranteUseCases.SolicitudRestauranteUseCases
{
    public class RechazarSolicitudRestauranteUseCase
    {
        private readonly ISolicitudRestauranteRepository _solicitudes;
        private readonly IUsuarioRepository _usuarios;
        private readonly IFileStorageService _firebase;
        public RechazarSolicitudRestauranteUseCase(
            ISolicitudRestauranteRepository solicitudes,
            IUsuarioRepository usuarios,IFileStorageService firebase)
        {
            _solicitudes = solicitudes;
            _usuarios = usuarios;
            _firebase = firebase;
        }

        public async Task HandleAsync(Guid solicitudId, string motivo, CancellationToken ct = default)
        {
            var solicitud = await _solicitudes.GetByIdAsync(solicitudId, ct)
                ?? throw new Exception("Solicitud no encontrada");

            if (solicitud.Estado != EstadoSolicitudRestaurante.Pendiente)
                throw new InvalidOperationException("Solo se pueden rechazar solicitudes pendientes.");

            if (string.IsNullOrWhiteSpace(motivo))
                throw new ArgumentException("Debe especificar un motivo de rechazo.");

            // Cambiar el estado
            solicitud.Estado = EstadoSolicitudRestaurante.Rechazada;
            solicitud.MotivoRechazo = motivo;

            // Cambiar el rol del usuario nuevamente a Usuario
            solicitud.Usuario.Rol = RolUsuario.Usuario;


            //eliminar imagnees de firebase :D
            foreach (var img in solicitud.Imagenes)
            {
                await _firebase.DeleteFileAsync(img.Url);
            }

            await _usuarios.UpdateAsync(solicitud.Usuario, ct);

            await _usuarios.SaveChangesAsync(ct);
            await _solicitudes.UpdateAsync(solicitud, ct);
        }
    }

}
