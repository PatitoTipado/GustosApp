using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GustosApp.Domain.Model;

namespace GustosApp.Domain.Interfaces
{
    
        public interface IRecomendadorRestaurantes
        {
            List<Restaurante> GenerarRecomendaciones(
                List<string> gustos,
                List<string> restricciones,
                List<Restaurante> restaurantesDisponibles,
                int maxResults = 10,
                CancellationToken ct = default);
        }
    }

