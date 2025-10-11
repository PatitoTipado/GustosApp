using GustosApp.Application.DTO;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;

namespace GustosApp.Application.UseCases
{
    public class CrearGrupoUseCase
    {
        private readonly IGrupoRepository _grupoRepository;
        private readonly IMiembroGrupoRepository _miembroGrupoRepository;
        private readonly IUsuarioRepository _usuarioRepository;

        public CrearGrupoUseCase(IGrupoRepository grupoRepository, 
            IMiembroGrupoRepository miembroGrupoRepository, 
            IUsuarioRepository usuarioRepository)
        {
            _grupoRepository = grupoRepository;
            _miembroGrupoRepository = miembroGrupoRepository;
            _usuarioRepository = usuarioRepository;
        }

        public async Task<GrupoResponse> HandleAsync(string firebaseUid, CrearGrupoRequest request, CancellationToken cancellationToken = default)
        {
            // Obtener el usuario por Firebase UID
            var usuario = await _usuarioRepository.GetByFirebaseUidAsync(firebaseUid, cancellationToken);
            if (usuario == null)
                throw new UnauthorizedAccessException("Usuario no encontrado");

            // Crear el grupo
            var grupo = new Grupo(request.Nombre, usuario.Id, request.Descripcion);
            await _grupoRepository.CreateAsync(grupo, cancellationToken);

            // Agregar al creador como miembro administrador
            var miembro = new MiembroGrupo(grupo.Id, usuario.Id, true);
            await _miembroGrupoRepository.CreateAsync(miembro, cancellationToken);

            // Obtener el grupo completo con relaciones
            var grupoCompleto = await _grupoRepository.GetByIdAsync(grupo.Id, cancellationToken);
            if (grupoCompleto == null)
                throw new InvalidOperationException("Error al crear el grupo");

            return new GrupoResponse(
                grupoCompleto.Id,
                grupoCompleto.Nombre,
                grupoCompleto.Descripcion,
                grupoCompleto.AdministradorId,
                grupoCompleto.Administrador.Nombre + " " + grupoCompleto.Administrador.Apellido,
                grupoCompleto.FechaCreacion,
                grupoCompleto.Activo,
                grupoCompleto.CodigoInvitacion,
                grupoCompleto.FechaExpiracionCodigo,
                grupoCompleto.Miembros.Count(m => m.Activo)
            );
        }
    }
}
