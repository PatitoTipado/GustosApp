using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;

namespace GustosApp.Application.UseCases.RestauranteUseCases
{
    public class ObtenerDatosRegistroRestauranteUseCase
    {
        private readonly IGustoRepository _gustos;
        private readonly IRestriccionRepository _restricciones;

        public ObtenerDatosRegistroRestauranteUseCase(
            IGustoRepository gustos,
            IRestriccionRepository restricciones)
        {
            _gustos = gustos;
            _restricciones = restricciones;
        }

        public async Task<(List<Gusto> gustos, List<Restriccion> restricciones)> HandleAsync(CancellationToken ct)
        {
            var gustos = await _gustos.GetAllAsync(ct);
            var restricciones = await _restricciones.GetAllAsync(ct);

            return (gustos, restricciones);

        }
    }

}
