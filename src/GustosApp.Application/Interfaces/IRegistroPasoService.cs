using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GustosApp.Domain.Model;

namespace GustosApp.Application.Interfaces
{
    public interface IRegistroPasoService
    {
        Task AplicarPasoAsync(
            Usuario usuario,
            RegistroPaso paso,
            string cacheKey,
            object? cacheValue,
            CancellationToken ct);
    }

}
