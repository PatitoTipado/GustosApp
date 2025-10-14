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
        public async Task<List<Restaurante>> GetAllAsync(CancellationToken ct= default)
        {
            return await _context.Restaurantes 
                           .Include(r => r.GustosQueSirve)
                           .ToListAsync(ct);
        }


        public async Task AddAsync(Restaurante restaurante, CancellationToken ct)
            => await _context.Restaurantes.AddAsync(restaurante, ct);

        public async Task SaveChangesAsync(CancellationToken ct)
            => await _context.SaveChangesAsync(ct);

        public async Task<List<Restaurante>> buscarRestauranteParaUsuariosConGustosYRestricciones(
            List<string> gustos,
            List<string> restricciones,
            CancellationToken ct = default)
        {
            var gustosNormalizados = gustos.Select(g => g.ToLower()).ToList();
            var restriccionesNormalizadas = restricciones.Select(r => r.ToLower()).ToList();

            // 1. CARGA COMPLETA Y MATERIALIZACIÓN TEMPRANA (¡Aquí forzamos el ToList!)
            // Esto trae TODOS los restaurantes y sus colecciones a la memoria del servidor.
            var todosLosRestaurantes = await _context.Restaurantes
                .Include(r => r.Reviews)
                .Include(r => r.RestriccionesQueRespeta)
                .Include(r => r.Platos)
                .Include(r => r.GustosQueSirve)
                .ToListAsync(ct); // <-- El ToList() se ejecuta aquí.

            // A partir de aquí, el filtrado se realiza en la memoria de .NET (LINQ to Objects).

            var query = todosLosRestaurantes.AsEnumerable(); // Usa AsEnumerable para claridad en el filtro de memoria

            // 2. FILTRADO POR GUSTOS (INCLUSIÓN)
            if (gustosNormalizados.Any())
            {
                // El restaurante debe servir AL MENOS UN gusto que el usuario quiere.
                query = query.Where(r => r.GustosQueSirve
                    .Any(g => g.Nombre != null && gustosNormalizados.Contains(g.Nombre.ToLower())));
            }

            // 3. FILTRADO POR RESTRICCIONES (EXCLUSIÓN - Lógica Corregida en Memoria)
            if (restriccionesNormalizadas.Any())
            {
                // El restaurante NO debe respetar NINGUNA de las restricciones que tiene el usuario.
                query = query.Where(r => r.RestriccionesQueRespeta
                    .Any(res => res.Nombre != null && restriccionesNormalizadas.Contains(res.Nombre.ToLower())));
            }

            // 4. Devolver la lista filtrada en memoria.
            return query.ToList();
        }

    }
}
