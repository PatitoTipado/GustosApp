using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GustosApp.Domain.Model;

namespace GustosApp.Domain.Interfaces
{
    public interface IEnviarMensajeGrupoUseCase
    {
        Task<ChatMensaje> HandleAsync(string firebaseUid, Guid grupoId, string mensaje
            , CancellationToken cancellationToken = default);

        

    }
}
