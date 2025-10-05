using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GustosApp.Domain.Model;

namespace GustosApp.Domain.Services
{
    public class CompatibilidadAlimentariaService
    {
        public (IReadOnlyList<Gusto> validos, IReadOnlyList<(Gusto gusto, string motivo)> conflictos)
            FiltrarGustos(
                IEnumerable<Gusto> todos,
                IEnumerable<Restriccion> restricciones,
                IEnumerable<CondicionMedica> condiciones)
        {
            var prohibidos = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var r in restricciones)
                foreach (var t in r.TagsProhibidos)
                    prohibidos.Add(t.NombreNormalizado);

            foreach (var c in condiciones)
                foreach (var t in c.TagsCriticos)
                    prohibidos.Add(t.NombreNormalizado);

            var validos = new List<Gusto>();
            var conflictos = new List<(Gusto, string)>();

            foreach (var g in todos)
            {
                bool choca = g.Tags.Any(t => prohibidos.Contains(t.NombreNormalizado));

                if (!choca)
                    validos.Add(g);
                else
                {
                    var motivo = "Conflicta con: " + string.Join(", ",
                        g.Tags
                         .Where(t => prohibidos.Contains(t.NombreNormalizado))
                         .Select(t => t.NombreNormalizado)
                         .Distinct());

                    conflictos.Add((g, motivo));
                }
            }

            return (validos, conflictos);
        }
    }
}
        


    

