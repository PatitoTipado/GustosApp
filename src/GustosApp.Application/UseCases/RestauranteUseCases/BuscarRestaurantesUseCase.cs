using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GustosApp.Application.DTO;

namespace GustosApp.Application.UseCases.RestauranteUseCases
{
    public class BuscarRestaurantesUseCase
    {
        private readonly IRestauranteRepository _repo;

        public BuscarRestaurantesUseCase(IRestauranteRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<Restaurante>> HandleAsync(string texto, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return new List<Restaurante>();

            var restaurantes = await _repo.BuscarPorTextoAsync(texto, ct);
            return restaurantes;
        }
    }
    }

