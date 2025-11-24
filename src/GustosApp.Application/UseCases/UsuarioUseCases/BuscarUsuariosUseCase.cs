using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;

namespace GustosApp.Application.UseCases.UsuarioUseCases
{
    public class BuscarUsuariosUseCase
    {

        private readonly IUsuarioRepository _usuarioRepository;

        public BuscarUsuariosUseCase(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public async Task<IEnumerable<Usuario>> HandleAsync(
            string firebaseUid, string? username, CancellationToken ct = default)
        {
            var usuarioActual = await _usuarioRepository.GetByFirebaseUidAsync(firebaseUid, ct);
            if (usuarioActual == null)
                throw new UnauthorizedAccessException("Usuario no encontrado o token inválido.");

            if (string.IsNullOrWhiteSpace(username))
            {
                return await _usuarioRepository.GetAllExceptAsync(usuarioActual.Id, 50, ct);
            }

            if (username.Length < 2)
                throw new ArgumentException("El nombre de usuario debe tener al menos 2 caracteres.");

            return await _usuarioRepository.BuscarPorUsernameAsync(username, usuarioActual.Id, ct);
        }
    }
}

