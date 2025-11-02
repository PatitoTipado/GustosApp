namespace GustosApp.Applicationm.DTO
{
    public class UsuarioPreferenciasDTO
    {
        public List<string> Gustos { get; set; } = new List<string>();
        public List<string> Restricciones { get; set; } = new List<string>();
        public List<string> CondicionesMedicas { get; set; } = new List<string>();

    }
}
