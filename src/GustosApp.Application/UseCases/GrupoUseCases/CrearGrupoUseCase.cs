using GustosApp.Application.Common.Exceptions;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;

namespace GustosApp.Application.UseCases
{
    public class CrearGrupoUseCase
    {
        private readonly IGrupoRepository _grupoRepository;
        private readonly IMiembroGrupoRepository _miembroGrupoRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private IGustosGrupoRepository _gustosGrupoRepository;

        public CrearGrupoUseCase(IGrupoRepository grupoRepository,
            IMiembroGrupoRepository miembroGrupoRepository,
            IUsuarioRepository usuarioRepository,
            IGustosGrupoRepository gustosGrupoRepository)
        {
            _grupoRepository = grupoRepository;
            _miembroGrupoRepository = miembroGrupoRepository;
            _usuarioRepository = usuarioRepository;
            _gustosGrupoRepository = gustosGrupoRepository;
        }

        public async Task<Grupo> HandleAsync(string firebaseUid, string NombreGrupo, string? DescripcionGrupo, CancellationToken cancellationToken = default)
        {
            // Obtener el usuario por Firebase UID  
            var usuario = await _usuarioRepository.GetByFirebaseUidAsync(firebaseUid, cancellationToken);
            if (usuario == null)
                throw new UnauthorizedAccessException("Usuario no encontrado");

            // Verificar límite de grupos para usuarios Free  
            if (!usuario.EsPremium())
            {
                var gruposActuales = await _grupoRepository.GetGruposByUsuarioIdAsync(usuario.Id, cancellationToken);
                var cantidadGrupos = gruposActuales.Count();

                if (cantidadGrupos >= 3)
                {
                    throw new LimiteGruposAlcanzadoException(
                        tipoPlan: "Free",
                        limiteActual: 3,
                        gruposActuales: cantidadGrupos
                    );
                }
            }

            // Crear el grupo  
            var grupo = new Grupo(NombreGrupo, usuario.Id, DescripcionGrupo);
            await _grupoRepository.CreateAsync(grupo, cancellationToken);

            // Agregar al creador como miembro administrador  
            var miembro = new MiembroGrupo(grupo.Id, usuario.Id, true);
            await _miembroGrupoRepository.CreateAsync(miembro, cancellationToken);

            await _gustosGrupoRepository.AgregarGustosAlGrupo(grupo.Id, usuario.Gustos.ToList(),miembro.Id);

            // Obtener el grupo completo con relaciones  
            var grupoCompleto = await _grupoRepository.GetByIdAsync(grupo.Id, cancellationToken);
            if (grupoCompleto == null)
                throw new InvalidOperationException("Error al crear el grupo");

            return grupoCompleto;
        }
    }
}
