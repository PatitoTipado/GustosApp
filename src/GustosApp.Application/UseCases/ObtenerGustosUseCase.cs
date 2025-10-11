using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GustosApp.Application.DTO;
using GustosApp.Domain.Interfaces;

namespace GustosApp.Application.UseCases
{
    public class ObtenerGustosUseCase
    {
        private readonly IUsuarioRepository _usuarioRepo;

        public ObtenerGustosUseCase(IUsuarioRepository usuarioRepo)
        {
            _usuarioRepo = usuarioRepo;
        }

       /* public async Task<List<GustoResponse>> HandleAsync(CancellationToken ct = default)
        {
            var gustos = await _usuarioRepo.GetAllAsync(ct);
            return gustos.Select(g => new GustoResponse(g.Id, g.Nombre, g.ImagenUrl)).ToList();
        }*/

        
        public async Task<List<string>> Handle(string firebaseUid, CancellationToken ct = default)
        {
            var usuario = await _usuarioRepo.GetByFirebaseUidAsync(firebaseUid, ct);

            if (usuario == null)
                throw new UnauthorizedAccessException("Usuario no encontrado o no registrado.");

            var gustos = usuario.Gustos?.Select(g => g.Nombre).ToList() ?? new List<string>();

            return gustos;

        }
    
    }
}
