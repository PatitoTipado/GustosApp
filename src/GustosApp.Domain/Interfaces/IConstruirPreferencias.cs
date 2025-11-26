using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GustosApp.Domain.Common;

namespace GustosApp.Domain.Interfaces
{
    public interface IConstruirPreferencias
    {
        Task<UsuarioPreferencias> HandleAsync(
            string firebaseUid,
            string? amigoUsername,
            Guid? grupoId,
            List<string>? gustosDelFiltro,
            CancellationToken ct);
    }

}
