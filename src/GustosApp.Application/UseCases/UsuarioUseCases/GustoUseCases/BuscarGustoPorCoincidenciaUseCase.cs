using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GustosApp.Application.UseCases.UsuarioUseCases.GustoUseCases
{
    public class BuscarGustoPorCoincidenciaUseCase
    {
        private readonly IGustoRepository _gustos;
        public BuscarGustoPorCoincidenciaUseCase(IGustoRepository gustos)
        {
            _gustos = gustos;
        }
        public async Task<List<Gusto>> HandleAsync(string nombreGusto)
        {
            return _gustos.ObtenerGustoPorCoincidencia(nombreGusto);
        }
    }
}
