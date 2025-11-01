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

        public async Task<GrupoResponse> HandleAsync(string firebaseUid, CrearGrupoRequest request, CancellationToken cancellationToken = default)
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
                    var beneficios = new BeneficiosPremiumDto { Precio = 9999.99m }; // Precio en pesos argentinos
                    var response = new LimiteGruposAlcanzadoResponse(
                        "Has alcanzado el límite de 3 grupos para usuarios gratuitos. Upgrade a Premium para crear grupos ilimitados.",
                        "Free",
                        3,
                        cantidadGrupos,
                        beneficios,
                        "/api/pago/crear" // URL temporal, se actualizará cuando implementemos el controlador
                    );
                    
                    throw new InvalidOperationException($"LIMITE_GRUPOS_ALCANZADO:{System.Text.Json.JsonSerializer.Serialize(response)}");
                }
            }

            // Crear el grupo
            var grupo = new Grupo(request.Nombre, usuario.Id, request.Descripcion);
            await _grupoRepository.CreateAsync(grupo, cancellationToken);

            // Agregar al creador como miembro administrador
            var miembro = new MiembroGrupo(grupo.Id, usuario.Id, true);
            await _miembroGrupoRepository.CreateAsync(miembro, cancellationToken);

            await _gustosGrupoRepository.AgregarGustosAlGrupo(grupo.Id,usuario.Gustos.ToList());

            // Obtener el grupo completo con relaciones
            var grupoCompleto = await _grupoRepository.GetByIdAsync(grupo.Id, cancellationToken);
            if (grupoCompleto == null)
                throw new InvalidOperationException("Error al crear el grupo");

            return new GrupoResponse(
                grupoCompleto.Id,
                grupoCompleto.Nombre,
                grupoCompleto.Descripcion,
                grupoCompleto.AdministradorId,
                grupoCompleto.Administrador?.FirebaseUid, // Add Firebase UID
                grupoCompleto.Administrador != null ? (grupoCompleto.Administrador.Nombre + " " + grupoCompleto.Administrador.Apellido) : string.Empty,
                grupoCompleto.FechaCreacion,
                grupoCompleto.Activo,
                grupoCompleto.CodigoInvitacion,
                grupoCompleto.FechaExpiracionCodigo,
                grupoCompleto.Miembros.Count(m => m.Activo)
            );
        }
    }
}
