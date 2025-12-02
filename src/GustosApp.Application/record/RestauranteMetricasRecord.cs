using GustosApp.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GustosApp.Application.record
{
    public record RestauranteMetricasRecord(
        Guid RestauranteId,
        RestauranteEstadisticas Estadisticas,
        List<UsuarioRestauranteFavorito> TotalFavoritos
    );
}
