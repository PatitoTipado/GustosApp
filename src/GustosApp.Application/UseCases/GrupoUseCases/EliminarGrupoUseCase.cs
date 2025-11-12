using System;
using System.Threading;
using System.Threading.Tasks;
using GustosApp.Domain.Interfaces;

namespace GustosApp.Application.UseCases
{
    public class EliminarGrupoUseCase
    {
        private readonly IGrupoRepository _grupoRepository;
        private readonly IUsuarioRepository _usuarioRepository;

        public EliminarGrupoUseCase(IGrupoRepository grupoRepository, IUsuarioRepository usuarioRepository)
        {
            _grupoRepository = grupoRepository;
            _usuarioRepository = usuarioRepository;
        }

        public async Task<bool> HandleAsync(string firebaseUid, Guid grupoId, CancellationToken cancellationToken = default)
        {
            var usuario = await _usuarioRepository.GetByFirebaseUidAsync(firebaseUid, cancellationToken);
            if (usuario == null) throw new UnauthorizedAccessException("Usuario no encontrado");

            var esAdmin = await _grupoRepository.UsuarioEsAdministradorAsync(grupoId, usuario.Id, cancellationToken);
            if (!esAdmin) throw new UnauthorizedAccessException("No tienes permisos para eliminar este grupo");

            await _grupoRepository.DeleteAsync(grupoId, cancellationToken);
            return true;
        }
    }
}
