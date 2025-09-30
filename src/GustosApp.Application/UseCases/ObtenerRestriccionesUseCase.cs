using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GustosApp.Application.DTO;
using GustosApp.Domain.Interfaces;

namespace GustosApp.Application.UseCases
{
    public class ObtenerRestriccionesUseCase
    {

        private readonly IRestriccionRepository _repo;

        public ObtenerRestriccionesUseCase(IRestriccionRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<RestriccionResponse>> HandleAsync(CancellationToken ct = default)
        {
            var gustos = await _repo.GetAllAsync(ct);
            return gustos.Select(g => new RestriccionResponse(g.Id, g.Nombre)).ToList();
        }
    }

}
