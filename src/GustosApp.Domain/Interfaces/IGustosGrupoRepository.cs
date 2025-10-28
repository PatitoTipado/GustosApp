using GustosApp.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GustosApp.Domain.Interfaces
{
    public interface IGustosGrupoRepository
    {
        public Task<bool> AgregarGustosAlGrupo(Guid grupoId, List<Gusto> gustos);
    }
}
