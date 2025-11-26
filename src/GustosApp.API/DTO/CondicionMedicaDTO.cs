namespace GustosApp.API.DTO
{
  


        public class CondicionMedicaResponse
        {

            public Guid Id { get; set; }
            public string Nombre { get; set; } = string.Empty;
            public bool Seleccionado { get; set; }

        }
    public class GuardarCondicionesResponse()
    {

        public string Mensaje { get; set; }
        public List<string> GustosRemovidos { get; set; }
    }
}

