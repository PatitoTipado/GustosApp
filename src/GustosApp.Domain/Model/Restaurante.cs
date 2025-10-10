namespace GustosApp.Domain.Model
{
    public class Restaurante
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; }
        public decimal? Latitud { get; set; }
        public decimal? Longitud { get; set; }

        public List<RestauranteEspecialidad> Especialidad { get; set; } = new List<RestauranteEspecialidad>();
        
        public Restaurante(Guid id, string nombre,decimal latitud,decimal longitud,List<RestauranteEspecialidad> especialidad)
        {
            Id = id;
            Especialidad = especialidad;
            Nombre = nombre;
            Latitud = latitud;
            Longitud = longitud;
        }

    }
}