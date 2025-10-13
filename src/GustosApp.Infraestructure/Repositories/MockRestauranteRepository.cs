using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GustosApp.Domain.Model;
using GustosApp.Domain.Interfaces; 
namespace GustosApp.Infraestructure.Repositories
{
    public class MockRestauranteRepository : IRestauranteRepository
    {
        private readonly List<Restaurante> _restaurantesHardcoded = new List<Restaurante>
        {
            // Restaurante 1: Pizza 
            new Restaurante
            {
                Id = Guid.Parse("A0000000-0000-0000-0000-000000000001"),
                Nombre = "restaurante1",
                Especialidad = new List<RestauranteEspecialidad>
                {
                    new RestauranteEspecialidad(Guid.NewGuid(), Guid.Empty, "pizza"),
                   // new RestauranteEspecialidad(Guid.NewGuid(), Guid.Empty, "Pizza Pepperoni"),
                }
            },
            // Restaurante 2: Sushi 
            new Restaurante
            {
                Id = Guid.Parse("A0000000-0000-0000-0000-000000000002"),
                Nombre = "restaurante2",
                Especialidad = new List<RestauranteEspecialidad>
                {
                    new RestauranteEspecialidad(Guid.NewGuid(), Guid.Empty, "sushi"),
                    //new RestauranteEspecialidad(Guid.NewGuid(), Guid.Empty, "Rolls California"),
                }
            },
            // Restaurante 3: Milanesa
            new Restaurante
            {
                Id = Guid.Parse("A0000000-0000-0000-0000-000000000003"),
                Nombre = "restaurante3",
                Especialidad = new List<RestauranteEspecialidad>
                {
                    new RestauranteEspecialidad(Guid.NewGuid(), Guid.Empty, "milanesa "),
                    //new RestauranteEspecialidad(Guid.NewGuid(), Guid.Empty, "Pizza"),
                   // new RestauranteEspecialidad(Guid.NewGuid(), Guid.Empty, "Sushi"),

                }
            },
        };





        // DEVOLVER DATOS REALES PARA LA PRUEBA
        public Task<List<Restaurante>> GetAllAsync(CancellationToken ct = default)
        {
            return Task.FromResult(_restaurantesHardcoded);
        }

        public Task AddAsync(Restaurante restaurante, CancellationToken ct)
        {
            return Task.CompletedTask;
        }

        public Task<Restaurante?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
            Task.FromResult(_restaurantesHardcoded.FirstOrDefault(r => r.Id == id));

        public Task<Restaurante?> GetByPlaceIdAsync(string placeId, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task SaveChangesAsync(CancellationToken ct)
        {
            return Task.CompletedTask;
        }

        public Task<List<Restaurante>> GetNearbyAsync(double lat, double lng, int radiusMeters, TimeSpan? maxAge = null, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
    }
}