using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GustosApp.Domain.Common
{
    public class CosineSimilarity
    {
        // Similitud de Coseno es una función matemática, va en el Core.
        public static double Coseno(float[] a, float[] b)
        {
            double dot = 0, na = 0, nb = 0;
            for (int i = 0; i < a.Length; i++)
            {
                dot += a[i] * b[i];
                na += a[i] * a[i];
                nb += b[i] * b[i];
            }
            // Evitamos división por cero (1e-8) y retornamos el resultado
            return dot / (Math.Sqrt(na) * Math.Sqrt(nb) + 1e-8);
        }

    }
}
