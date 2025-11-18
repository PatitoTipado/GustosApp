using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GustosApp.Application.Interfaces
{
    public interface IUsuariosActivosService
    {
        void UsuarioConectado(string firebaseUid);




       void UsuarioDesconectado(string firebaseUid);
     
    
        IReadOnlyCollection<string> GetUsuariosActivos();
    }

}
