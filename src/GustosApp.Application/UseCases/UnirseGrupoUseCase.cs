using GustosApp.Application.Common.Exceptions;
using GustosApp.Application.DTO;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;

namespace GustosApp.Application.UseCases
{
    public class UnirseGrupoUseCase
    {
        private readonly IGrupoRepository _grupoRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IMiembroGrupoRepository _miembroGrupoRepository;
        private IGustosGrupoRepository _gustosGrupoRepository;


        public UnirseGrupoUseCase(IGrupoRepository grupoRepository,
            IUsuarioRepository usuarioRepository,
            IMiembroGrupoRepository miembroGrupoRepository,
            IGustosGrupoRepository gustosGrupoRepository)
        {
            _grupoRepository = grupoRepository;
            _usuarioRepository = usuarioRepository;
            _miembroGrupoRepository = miembroGrupoRepository;
            _gustosGrupoRepository = gustosGrupoRepository;
        }

        public async Task<Grupo> HandleAsync(string firebaseUid, UnirseGrupoRequest request, CancellationToken cancellationToken = default)
        {
            // Obtener el usuario
            var usuario = await _usuarioRepository.GetByFirebaseUidAsync(firebaseUid, cancellationToken);
            if (usuario == null)
                throw new UnauthorizedAccessException("Usuario no encontrado");

            // Buscar el grupo por código de invitación
            var grupo = await _grupoRepository.GetByCodigoInvitacionAsync(request.CodigoInvitacion, cancellationToken);
            if (grupo == null)
                throw new ArgumentException("Código de invitación inválido");

            // Verificar que el código no ha expirado
            if (!grupo.EsCodigoInvitacionValido(request.CodigoInvitacion))
                throw new ArgumentException("El código de invitación ha expirado");

            // Verificar que el usuario no es ya miembro del grupo
            if (await _miembroGrupoRepository.UsuarioEsMiembroActivoAsync(grupo.Id, usuario.Id, cancellationToken))
                throw new ArgumentException("Ya eres miembro de este grupo");

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
                        gruposActuales: cantidadGrupos,
                        message: "Has alcanzado el límite de 3 grupos para usuarios gratuitos. Mejora a Premium para unirte a más grupos."
                    );
                }
            }

            // Agregar al usuario como miembro del grupo
            var miembro = new MiembroGrupo(grupo.Id, usuario.Id, false);
            await _miembroGrupoRepository.CreateAsync(miembro, cancellationToken);
            await _gustosGrupoRepository.AgregarGustosAlGrupo(grupo.Id, usuario.Gustos.ToList(), miembro.Id);

            // Obtener el grupo completo con relaciones
            var grupoCompleto = await _grupoRepository.GetByIdAsync(grupo.Id, cancellationToken);
            if (grupoCompleto == null)
                throw new InvalidOperationException("Error al unirse al grupo");

            return grupoCompleto;
        }
    }
}
