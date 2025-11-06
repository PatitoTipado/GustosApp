using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GustosApp.Application.Common.Exceptions
{
    public class LimiteGruposAlcanzadoException : Exception
    {
        public string TipoPlan { get; }
        public int LimiteActual { get; }
        public int GruposActuales { get; }

        public LimiteGruposAlcanzadoException(
            string tipoPlan,
            int limiteActual,
            int gruposActuales,
            string message = "Límite de grupos alcanzado para usuarios gratuitos.")
            : base(message)
        {
            TipoPlan = tipoPlan;
            LimiteActual = limiteActual;
            GruposActuales = gruposActuales;
        }
    }
}
