using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GustosApp.Application.Interfaces;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;

namespace GustosApp.Application.Services
{
    public class RegistroPasoService : IRegistroPasoService
    {
        private readonly IUsuarioRepository _repo;
        private readonly ICacheService _cache;

        public RegistroPasoService(
            IUsuarioRepository repo,
            ICacheService cache)
        {
            _repo = repo;
            _cache = cache;
        }

        public async Task AplicarPasoAsync(
            Usuario usuario,
            RegistroPaso paso,
            string cacheKey,
            object? cacheValue,
            CancellationToken ct)
        {
            
            usuario.AvanzarPaso(paso);

            await _repo.SaveChangesAsync(ct);

            if (cacheValue == null)
            {
                await _cache.DeleteAsync(cacheKey);
            }
            else
            {
                await _cache.SetAsync(cacheKey, cacheValue, TimeSpan.FromHours(1));
            }

            // Estado del usuario actual
            await _cache.SetAsync(
                $"registro:{usuario.FirebaseUid}:estado",
                paso.ToString().ToLower(),
                TimeSpan.FromHours(1));
        }
    }

}
