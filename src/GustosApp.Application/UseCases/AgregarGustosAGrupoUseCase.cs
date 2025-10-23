using GustosApp.Application.DTO;
using GustosApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GustosApp.Application.UseCases
{
    public class AgregarGustosAGrupoUseCase
    {
        private IGrupoRepository _grupoRepository;

        public AgregarGustosAGrupoUseCase(IGrupoRepository grupoRepository)
        {
            _grupoRepository=grupoRepository;
        }

        public Task<bool> Handle(UsuarioPreferenciasDTO invitadoPreferencias, Guid grupoId)
        {
          //buscamos grupos y le asignamos si los gustos son los de aca
          
          return null;
        }
    }
}
