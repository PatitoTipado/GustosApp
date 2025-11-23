using Microsoft.AspNetCore.Http;

namespace GustosApp.API.DTO
{
    public class ActualizarImagenRestauranteRequest
    {
        public IFormFile? Archivo { get; set; }
        public bool SoloBorrar { get; set; }
    }

    public class ActualizarImagenesRestauranteRequest
    {
        public List<IFormFile>? Archivos { get; set; }
        public bool SoloBorrar { get; set; }
    }
}
