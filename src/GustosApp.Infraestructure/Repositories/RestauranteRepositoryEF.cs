<<<<<<< HEAD
﻿using GustosApp.Domain.Model;
using System;
=======
﻿using System;
>>>>>>> origin/develop
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
<<<<<<< HEAD
using Microsoft.EntityFrameworkCore;
using GustosApp.Domain.Interfaces;

namespace GustosApp.Infraestructure.Repositories
{
    public class RestauranteRepositoryEF : IRestaurantRepository
    {
        private readonly GustosDbContext _dbContext;

        public RestauranteRepositoryEF(GustosDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Restaurante>> GetAllAsync(CancellationToken ct= default)
        {
            return await _dbContext.Restaurante 
                           .Include(r => r.Especialidad)
                           .ToListAsync(ct);
        }
=======
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace GustosApp.Infraestructure.Repositories
{
    public class RestauranteRepositoryEF : IRestauranteRepository
    {
        private readonly GustosDbContext _context;

        public RestauranteRepositoryEF(GustosDbContext context)
        {
            _context = context;
        }

        public async Task<Restaurante?> GetByPlaceIdAsync(string placeId, CancellationToken ct)
        {
            return await _context.Restaurantes
                .Include(r => r.Reviews) 
                .FirstOrDefaultAsync(r => r.PlaceId == placeId, ct);
        }


        public async Task AddAsync(Restaurante restaurante, CancellationToken ct)
            => await _context.Restaurantes.AddAsync(restaurante, ct);

        public async Task SaveChangesAsync(CancellationToken ct)
            => await _context.SaveChangesAsync(ct);
>>>>>>> origin/develop
    }
}
