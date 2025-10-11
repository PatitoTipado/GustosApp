using GustosApp.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GustosApp.Domain.Interfaces
{
    public interface IModelMatcherService
    {
        // Usa Entidades de Dominio y devuelve un resultado de Dominio.
        // El Use Case solo le pasa lo que el negocio necesita para el match.
        List<(string restaurant, double score)> Match(
            Usuario user,
            List<string> restaurantes);
    }
}
