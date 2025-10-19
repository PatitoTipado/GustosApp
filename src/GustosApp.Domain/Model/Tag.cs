namespace GustosApp.Domain.Model
{
    public class Tag
    {
        public Guid Id { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public string NombreNormalizado => Nombre.Trim().ToLowerInvariant();

        public TipoTag Tipo { get; set; }


        
    }

    public enum TipoTag
    {
        Ingrediente = 1,
        Nutriente = 2,
        Categoria = 3
    }
}