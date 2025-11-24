using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GustosApp.Domain.Model;

namespace GustosApp.Application.Model
{
    public record RestauranteDetalleResult(
     Restaurante Restaurante,
     bool EsFavorito
 );

}
