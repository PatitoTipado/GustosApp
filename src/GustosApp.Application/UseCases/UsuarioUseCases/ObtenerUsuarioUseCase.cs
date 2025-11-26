using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;

namespace GustosApp.Application.UseCases.UsuarioUseCases
{
    public class ObtenerUsuarioUseCase
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public ObtenerUsuarioUseCase(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public virtual async Task<Usuario> HandleAsync(
            string? FirebaseUid = null,
            string? Username = null,
            Guid? id = null,
            CancellationToken ct = default)
        {
            Usuario? usuario = null;

            if (!string.IsNullOrWhiteSpace(FirebaseUid))
            {
                usuario = await _usuarioRepository.GetByFirebaseUidAsync(FirebaseUid, ct);
            }
            else if (!string.IsNullOrWhiteSpace(Username))
            {
                usuario = await _usuarioRepository.GetByUsernameAsync(Username, ct);
            }

            if (usuario == null)
                throw new Exception("Usuario no encontrado");

            return usuario;
        }

   
        public async Task<Usuario> HandleWithVisitadosAsync(
            
            string? Username,
            CancellationToken ct = default)
        {
            Usuario? usuario = null;
             usuario = await _usuarioRepository.GetByUsernameWithRestaurantesVisitadosAsync(Username, ct);
         
            if (usuario == null)
                throw new Exception("Usuario no encontrado");

            return usuario;
        }
    }

}
