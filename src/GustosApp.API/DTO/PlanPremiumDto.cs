using System;
using System.Collections.Generic;

namespace GustosApp.API.DTO
{
    public class LimiteGruposAlcanzadoResponse
    {
        public string Mensaje { get; set; } = string.Empty;
        public string TipoPlan { get; set; } = string.Empty;
        public int LimiteActual { get; set; }
        public int GruposActuales { get; set; }
        public BeneficiosPremiumDto BeneficiosPremium { get; set; }
        public string UrlPago { get; set; } = string.Empty;

        public LimiteGruposAlcanzadoResponse(string mensaje, string tipoPlan, int limiteActual, int gruposActuales, BeneficiosPremiumDto beneficios, string urlPago)
        {
            Mensaje = mensaje;
            TipoPlan = tipoPlan;
            LimiteActual = limiteActual;
            GruposActuales = gruposActuales;
            BeneficiosPremium = beneficios;
            UrlPago = urlPago;
        }
    }

    public class BeneficiosPremiumDto
    {
        public List<string> Beneficios { get; set; } = new List<string>();
        public decimal Precio { get; set; }
        public string Moneda { get; set; } = "ARS";

        public BeneficiosPremiumDto()
        {
            Beneficios = new List<string>
            {
                "Grupos ilimitados",
                "Recomendaciones mejoradas con IA",
                "Análisis avanzado de preferencias grupales",
                "Soporte prioritario",
                "Funciones exclusivas para administradores"
            };
        }
    }

    public class CrearPagoRequest
    {
        public string FirebaseUid { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
    }

    public class CrearPagoResponse
    {
        public string InitPoint { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }

    public class CrearPagoTestRequest
    {
        public string? UsuarioId { get; set; }
        public string? Email { get; set; }
        public string? NombreCompleto { get; set; }
    }

    public class WebhookPagoRequest
    {
        public string Type { get; set; } = string.Empty;
        public WebhookPagoData Data { get; set; } = new WebhookPagoData();
    }

    public class WebhookPagoData
    {
        public string Id { get; set; } = string.Empty;
    }
}
