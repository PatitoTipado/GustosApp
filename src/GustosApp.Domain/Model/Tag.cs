namespace GustosApp.Domain.Model
{
    public class Tag
    {
        public Guid Id { get; set; }

        public string NombreNormalizado => NombreNormalizado.Trim().ToLowerInvariant();

        public TipoTag Tipo { get; set; }


    }

    public enum TipoTag
    {
        Gusto=1,
        Restriccion=2 ,
        CondicionMedica=3
    }
}