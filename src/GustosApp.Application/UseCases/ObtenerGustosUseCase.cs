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
        private readonly IGustoRepository _repo;

        public ObtenerGustosUseCase(IGustoRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<GustoResponse>> HandleAsync(CancellationToken ct = default)
        {
            var gustos = await _repo.GetAllAsync(ct);
            return gustos.Select(g => new GustoResponse(g.Id, g.Nombre, g.ImagenUrl)).ToList();
        }
    }
}
