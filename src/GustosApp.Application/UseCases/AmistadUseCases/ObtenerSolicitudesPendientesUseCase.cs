using GustosApp.Application.DTO;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;

namespace GustosApp.Application.UseCases.AmistadUseCases
{
    public class ObtenerSolicitudesPendientesUseCase
    {
        private readonly ISolicitudAmistadRepository _solicitudRepository;
        private readonly IUsuarioRepository _usuarioRepository;

        public ObtenerSolicitudesPendientesUseCase(ISolicitudAmistadRepository solicitudRepository, IUsuarioRepository usuarioRepository)
        {
            _solicitudRepository = solicitudRepository;
            _usuarioRepository = usuarioRepository;
        }

        public async Task<IEnumerable<SolicitudAmistad>> HandleAsync(
    string firebaseUid,
    CancellationToken ct = default)
        {
            Console.WriteLine($"🔥 UID recibido: {firebaseUid}");

            var usuario = await _usuarioRepository.GetByFirebaseUidAsync(firebaseUid, ct)
                ?? throw new UnauthorizedAccessException("Usuario no encontrado");

            Console.WriteLine($"✅ Usuario encontrado: {usuario.IdUsuario} ({usuario.Id})");

            var pendientes = await _solicitudRepository
                .GetSolicitudesPendientesByUsuarioIdAsync(usuario.Id, ct);

            Console.WriteLine($"📬 Solicitudes encontradas: {pendientes.Count()}");

            return pendientes;
        }

    }
}
