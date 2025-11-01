using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GustosApp.Application.DTO;
using GustosApp.Domain.Interfaces;

namespace GustosApp.Application.UseCases
{
    public class ObtenerGrupoDetalleUseCase
    {
        private readonly IGrupoRepository _grupoRepository;

        public ObtenerGrupoDetalleUseCase(IGrupoRepository grupoRepository)
        {
            _grupoRepository = grupoRepository;
        }

        public async Task<GrupoResponse> HandleAsync(string firebaseUid, Guid grupoId, CancellationToken cancellationToken = default)
        {
            var grupo = await _grupoRepository.GetByIdAsync(grupoId, cancellationToken);
            if (grupo == null) throw new ArgumentException("Grupo no encontrado");

            var resp = new GrupoResponse(grupo.Id, grupo.Nombre, grupo.Descripcion, grupo.AdministradorId, 
                grupo.Administrador?.FirebaseUid, // Include Firebase UID
                grupo.Administrador != null ? (grupo.Administrador.Nombre + " " + grupo.Administrador.Apellido) : string.Empty,
                grupo.FechaCreacion, grupo.Activo, grupo.CodigoInvitacion, grupo.FechaExpiracionCodigo, grupo.Miembros?.Count(m => m.Activo) ?? 0);

            if (grupo.Miembros != null)
            {
                foreach (var m in grupo.Miembros.Where(m => m.Activo)) // Only include active members
                {
                    if (m == null) continue;
                    resp.Miembros.Add(new MiembroGrupoResponse(
                    m.Id,
                    m.UsuarioId,
                    m.Usuario?.FirebaseUid,
                    m.Usuario != null ? (m.Usuario.Nombre + " " + m.Usuario.Apellido) : string.Empty,
                    m.Usuario?.Email ?? string.Empty,
                    m.Usuario?.IdUsuario ?? string.Empty, 
                    m.FechaUnion,
                    m.EsAdministrador
                    ));

                }
            }

            return resp;
        }
    }
}
