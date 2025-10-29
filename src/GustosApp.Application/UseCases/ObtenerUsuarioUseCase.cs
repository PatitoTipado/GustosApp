using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;

namespace GustosApp.Application.UseCases
{
    public class ObtenerUsuarioUseCase
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public ObtenerUsuarioUseCase(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public async Task<Usuario> HandleAsync(string uid, CancellationToken ct)
        {
            var usuario = await _usuarioRepository.GetByFirebaseUidAsync(uid, ct);
            if (usuario == null)
            {
                throw new Exception("Usuario no encontrado");
            }

            return usuario;
        }
    }
}
