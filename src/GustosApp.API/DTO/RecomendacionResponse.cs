using Microsoft.AspNetCore.Mvc;

namespace GustosApp.API.DTO
{
    public class RecomendacionResponse
    {
        public Guid RestauranteId { get; set; }
        public string Explicacion { get; set; }
    }
    
}
