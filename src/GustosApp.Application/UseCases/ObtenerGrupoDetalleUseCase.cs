using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GustosApp.Application.DTO;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;

namespace GustosApp.Application.UseCases
{
    public class ObtenerGrupoDetalleUseCase
    {
        private readonly IGrupoRepository _grupoRepository;
        private readonly IUsuarioRepository _usuarioRepository;

        public ObtenerGrupoDetalleUseCase(IGrupoRepository grupoRepository,
            IUsuarioRepository usuarioRepository)
        {
            _grupoRepository = grupoRepository;
            _usuarioRepository = usuarioRepository;
        }

        public async Task<Grupo> HandleAsync(string firebaseUid, Guid grupoId, CancellationToken cancellationToken = default)
        {
            var usuario = await _usuarioRepository.GetByFirebaseUidAsync(firebaseUid, cancellationToken)
            ?? throw new UnauthorizedAccessException("Usuario no encontrado");

            // Buscar el grupo con sus relaciones
            var grupo = await _grupoRepository.GetByIdAsync(grupoId, cancellationToken)
                ?? throw new ArgumentException("Grupo no encontrado");

            return grupo;
        }
    } 
}
