using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GustosApp.Application.DTO
{
    public record PasoResponse(
      string PasoActual,
      string Next,
      object? Data = null,
      IEnumerable<string>? Conflictos = null
  );
}
