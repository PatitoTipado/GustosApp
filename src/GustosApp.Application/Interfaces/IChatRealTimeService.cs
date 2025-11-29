using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GustosApp.Application.Interfaces
{
    public interface IChatRealTimeService
    {


        Task UsuarioSeUnio(Guid grupoId, Guid usuarioId, string nombre);
        Task UsuarioExpulsadoDelGrupo(Guid grupoId, string firebaseUid, string nombreGrupo);

        Task UsuarioAbandono(Guid grupoId, Guid usuarioId, string IdUser, string firebaseUid);
    }

}
