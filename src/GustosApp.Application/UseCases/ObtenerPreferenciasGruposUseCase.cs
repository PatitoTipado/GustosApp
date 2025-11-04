using GustosApp.Application.DTO;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GustosApp.Application.UseCases
{
    public class ObtenerPreferenciasGruposUseCase
    {
        private readonly IGustosGrupoRepository _gustosGrupoRepository;

        public ObtenerPreferenciasGruposUseCase(IGustosGrupoRepository gustosGrupoRepository)
        {
            _gustosGrupoRepository = gustosGrupoRepository;
        }

        public async Task<UsuarioPreferencias> HandleAsync(Guid grupoId,CancellationToken ct)
        {
            var gustos = await _gustosGrupoRepository.ObtenerGustosDelGrupo(grupoId);

            return new UsuarioPreferencias { Gustos = gustos };
        }

    }
}
