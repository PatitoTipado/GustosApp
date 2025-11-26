using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GustosApp.Domain.Common;
using GustosApp.Domain.Model;

namespace GustosApp.Domain.Interfaces
{

    public interface IRecomendadorRestaurantes
    {
        Task<List<Restaurante>> Handle(
            UsuarioPreferencias usuario,
            List<Restaurante> restaurantesCercanos,
            int maxResults = 10,
            CancellationToken ct = default
        );
    }

}
