using GustosApp.Application.DTO;
using GustosApp.Domain.Interfaces;
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

        public async Task<UsuarioPreferenciasDTO> Handle(Guid grupoId,CancellationToken ct)
        {
            var gustosGrupo = await _gustosGrupoRepository.ObtenerGustosDelGrupo(grupoId);

            UsuarioPreferenciasDTO preferenciasDTOs = new UsuarioPreferenciasDTO { Gustos = gustosGrupo };

            return preferenciasDTOs;
        }

    }
}
