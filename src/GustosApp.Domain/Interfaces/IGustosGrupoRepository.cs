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
        Task<bool> AgregarGustosAlGrupo(Guid grupoId, List<Gusto> gustos,Guid idMiembro);
        Task<bool> EliminarGustosAlGrupo(Guid grupoId, List<Gusto> gustos,Guid miembroGrupoId);
        Task<List<string>> ObtenerGustosDelGrupo(Guid grupoId);
    }
}
