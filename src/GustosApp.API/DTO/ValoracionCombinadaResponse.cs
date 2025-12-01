namespace GustosApp.API.DTO
{
    public class ValoracionCombinadaResponse
    {
        public Guid RestauranteId { get; set; }
        public string NombreRestaurante { get; set; } = string.Empty;
        
        // Valoración combinada que es el promedio ponderado
        public double ValoracionCombinada { get; set; }
        public int TotalValoraciones { get; set; }
        
        // Desglose de valoraciones
        public ValoracionFuente ValoracionUsuariosApp { get; set; } = null!;
        public ValoracionFuente ValoracionGooglePlaces { get; set; } = null!;
    }
    
    public class ValoracionFuente
    {
        public double Rating { get; set; }
        public int CantidadValoraciones { get; set; }
        public bool Disponible { get; set; }
    }
}
