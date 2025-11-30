using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GustosApp.Application.Interfaces
{
    public interface IChatRealTimeService
    {


        Task UsuarioSeUnio(Guid grupoId, Guid usuarioId, string nombre,string? fotourl);
        Task UsuarioExpulsadoDelGrupo(Guid grupoId, Guid usuarioId, string firebaseUid, string nombre);
        Task UsuarioAbandono(Guid grupoId, Guid usuarioId, string IdUser, string firebaseUid);
    }

}
