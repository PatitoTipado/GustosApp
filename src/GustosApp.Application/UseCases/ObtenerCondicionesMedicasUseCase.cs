using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GustosApp.Application.DTO;
using GustosApp.Domain.Interfaces;

namespace GustosApp.Application.UseCases
{
     public class ObtenerCondicionesMedicasUseCase
    {
        public readonly ICondicionMedicaRepository _repo;

        public ObtenerCondicionesMedicasUseCase(ICondicionMedicaRepository repo)
        {
            _repo = repo;
            
        }

        public async Task<List<CondicionMedicaResponse>> HandleAsync(CancellationToken ct = default)
        {
            var condicionMedicas= await _repo.GetAllAsync(ct);
            return condicionMedicas.Select(c =>new CondicionMedicaResponse(c.Id,c.Nombre)).ToList();
        }
    }
}
