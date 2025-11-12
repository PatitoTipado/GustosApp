using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GustosApp.Application.DTO;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;

namespace GustosApp.Application.UseCases.UsuarioUseCases
{
    public class ObtenerResumenRegistroUseCase
    {
        public readonly IUsuarioRepository _usuarios;
        public ObtenerResumenRegistroUseCase(IUsuarioRepository usuarios)
        {
            _usuarios = usuarios;
        }

        public async Task<Usuario> HandleAsync(string firebaseUid, CancellationToken ct)
        {
            var usuario = await _usuarios.GetByFirebaseUidAsync(firebaseUid, ct)
                ?? throw new KeyNotFoundException("Usuario no encontrado");

            return usuario;
        }
    }

}

