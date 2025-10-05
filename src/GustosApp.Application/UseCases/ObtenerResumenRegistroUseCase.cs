using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GustosApp.Application.DTO;
using GustosApp.Domain.Interfaces;

namespace GustosApp.Application.UseCases
{
    public class ObtenerResumenRegistroUseCase
    {
        public readonly IUsuarioRepository _usuarios;
        public ObtenerResumenRegistroUseCase(IUsuarioRepository usuarios)
        {
            _usuarios = usuarios;
        }

        public async Task<UsuarioResumenResponse> HandleAsync(string firebaseUid, CancellationToken ct)
            {
                var usuario = await _usuarios.GetByFirebaseUidAsync(firebaseUid, ct)
                    ?? throw new KeyNotFoundException("Usuario no encontrado");

                return new UsuarioResumenResponse
                {
                    Id = usuario.Id,
                    Nombre = usuario.Nombre,
                    Apellido = usuario.Apellido,
                    Email = usuario.Email,
                    FotoPerfilUrl = usuario.FotoPerfilUrl,
                    Gustos = usuario.Gustos.Select(g => g.Nombre).ToList(),
                    Restricciones = usuario.Restricciones.Select(r => r.Nombre).ToList(),
                    CondicionesMedicas = usuario.CondicionesMedicas.Select(c => c.Nombre).ToList()
                };
            }
        }

    }

