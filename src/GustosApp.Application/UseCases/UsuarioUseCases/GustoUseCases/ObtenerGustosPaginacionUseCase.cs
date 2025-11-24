using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GustosApp.Application.UseCases.UsuarioUseCases.GustoUseCases
{
    public class ObtenerGustosPaginacionUseCase
    {
        private readonly IUsuarioRepository _usuarios;
        private readonly IGustoRepository _gustos;
        public ObtenerGustosPaginacionUseCase(IUsuarioRepository usuarios, IGustoRepository gustos)
        {
            _usuarios = usuarios;
            _gustos = gustos;
        }
        public async Task<List<Gusto>> HandleAsync(int cantidadInicio, int final)
        {
            return _gustos.obtenerGustosPorPaginacion(cantidadInicio, final);
        }
    }
}
