namespace GustosApp.API.DTO
{
    public class RestauranteMetricasDashboardResponse
    {
        public Guid RestauranteId { get; set; }

        public int TotalTop3Individual { get; set; }

        public int TotalTop3Grupo { get; set; }

        public int TotalVisitasPerfil { get; set; }


        public int TotalFavoritosHistorico { get; set; }

        public int TotalFavoritosActual { get; set; }
    }
}
