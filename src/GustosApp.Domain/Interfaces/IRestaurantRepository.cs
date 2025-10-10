using GustosApp.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GustosApp.Domain.Interfaces
{
    public interface IRestaurantRepository
    {
        Task<List<Restaurante>> GetAllAsync(CancellationToken ct = default);
    }
}
