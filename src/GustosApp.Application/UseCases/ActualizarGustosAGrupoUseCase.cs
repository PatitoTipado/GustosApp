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
    public class ActualizarGustosAGrupoUseCase
    {
        private IGrupoRepository _grupoRepository;
        private IGustoRepository _gustoRepository;
        private IGustosGrupoRepository _gustosGrupoRepository;

        public ActualizarGustosAGrupoUseCase(IGrupoRepository grupoRepository, IGustoRepository gustoRepository, IGustosGrupoRepository gustosGrupoRepository)
        {
            _grupoRepository = grupoRepository;
            _gustoRepository = gustoRepository;
            _gustosGrupoRepository = gustosGrupoRepository;
        }

        public Task<bool> Handle(List<string>gustosDeUsuario, Guid grupoId)
        {
            //primero validamos que exista el grupo -> todos estos false los cambiare por exeptions despues

            if (!_grupoRepository.ExistsAsync(grupoId).Result)
            {
                return Task.FromResult(false);
            }

            //validamos que los gustos existan

            List<Gusto> gustos= _gustoRepository.obtenerGustosPorNombre(gustosDeUsuario).Result;

            if (gustos==null || gustos.Count()==0)
            {
                return Task.FromResult(false);
            }

            return _gustosGrupoRepository.actualizarPreferenciasGrupo(grupoId,gustos);
        }
    }
}
