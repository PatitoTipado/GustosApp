using GustosApp.Domain.Model.@enum;

namespace GustosApp.Domain.Model
{
    public class Tag
    {
        public Guid Id { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public string NombreNormalizado => Nombre.Trim().ToLowerInvariant();

        public TipoTag Tipo { get; set; }


        
    }
}