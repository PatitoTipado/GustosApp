using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model.@enum;
using GustosApp.Domain.Model;
using static System.Runtime.InteropServices.JavaScript.JSType;
using GustosApp.Application.Interfaces;

namespace GustosApp.Application.UseCases.RestauranteUseCases.SolicitudRestauranteUseCases
{
    public class CrearSolicitudRestauranteUseCase
    {
        private readonly ISolicitudRestauranteRepository _solicitudes;
        private readonly IGustoRepository _gustos;
        private readonly IRestriccionRepository _restricciones;
        private readonly IUsuarioRepository _usuarios;
        private readonly IFirebaseAuthService _firebase;
        private readonly IEmailService _email;
        private readonly IEmailTemplateService _templates;

        public CrearSolicitudRestauranteUseCase(
            ISolicitudRestauranteRepository solicitudes,
             IRestriccionRepository restricciones, IGustoRepository gustos,
            IUsuarioRepository usuarios, IFirebaseAuthService firebase,
            IEmailService email, IEmailTemplateService templates
)
        {
            _solicitudes = solicitudes;
            _usuarios = usuarios;
            _gustos = gustos;
            _restricciones = restricciones;
            _firebase = firebase;
            _email = email;
           _templates = templates;
        }

        public async Task<Guid> HandleAsync(
     string firebaseUid,
     string nombre,
     string direccion,
     double? latitud,
     double? longitud,
     string? horariosJson,
     List<Guid>? gustosIds,
     List<Guid>? restriccionesIds,
     List<SolicitudRestauranteImagen> imagenes,
     string websiteUrl,
     CancellationToken ct = default)
        {
            // 1) Obtener usuario
            var usuario = await _usuarios.GetByFirebaseUidAsync(firebaseUid, ct);
            if (usuario == null)
                throw new Exception("Usuario no encontrado");

            // 2) Validar rol
            if (usuario.Rol != RolUsuario.Usuario)
                throw new Exception("Ya tenés una solicitud pendiente o ya sos dueño de un restaurante.");

            // 3) Crear la solicitud
            var solicitud = new SolicitudRestaurante
            {
                UsuarioId = usuario.Id,
                Nombre = nombre.Trim(),
                Direccion = direccion.Trim(),
                Latitud = latitud,
                Longitud = longitud,
                HorariosJson = horariosJson,
                GustosIds = gustosIds ?? new List<Guid>(),
                RestriccionesIds = restriccionesIds ?? new List<Guid>(),
                Imagenes = imagenes ?? new List<SolicitudRestauranteImagen>(),
                FechaCreacion = DateTime.UtcNow,
                Estado = EstadoSolicitudRestaurante.Pendiente,
                WebsiteUrl = websiteUrl.Trim()
            };

            solicitud.Gustos = await _gustos.GetByIdsAsync(gustosIds,ct);
            solicitud.Restricciones = await _restricciones.GetRestriccionesByIdsAsync(restriccionesIds,ct);

            await _solicitudes.AddAsync(solicitud, ct);

            
            usuario.Rol = RolUsuario.PendienteRestaurante;
            await _usuarios.UpdateAsync(usuario, ct);

            await _firebase.SetUserRoleAsync(usuario.FirebaseUid, RolUsuario.PendienteRestaurante.ToString());

            //Modificar esto si es deploy?
            await _email.EnviarEmailAsync(
        "gonzalomarcos551@gmail.com",
        "Nueva solicitud de restaurante",
        _templates.Render("SolicitudNueva.html", new Dictionary<string, string>
        {
        { "USUARIO", solicitud.Usuario.Email },
        { "NOMBRE", solicitud.Nombre },
        { "DIRECCION", solicitud.Direccion },
        { "LINK", "http://localhost:3000/admin" }
        })
    );

            return solicitud.Id;
        }

    }

    }
