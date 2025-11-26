using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GustosApp.Application.Common.Exceptions
{
    public class LimiteFavoritosAlcanzadoException : Exception
    {
        public string TipoPlan { get; }
        public int LimiteActual { get; }
        public int FavoritosActuales { get; }

        public LimiteFavoritosAlcanzadoException(
            string tipoPlan,
            int limiteActual,
            int favoritosActuales,
            string message = "Límite de favoritos alcanzado para este plan.")
            : base(message)
        {
            TipoPlan = tipoPlan;
            LimiteActual = limiteActual;
            FavoritosActuales = favoritosActuales;
        }
    }

}
