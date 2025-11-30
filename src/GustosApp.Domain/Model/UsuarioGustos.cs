
namespace GustosApp.Domain.Model
{
    public class UsuarioGustos
    {
        public List<Gusto> GustosFiltrados { get; set; } = new();
        public List<Guid> GustosSeleccionados { get; set; } = new();
    }
}
