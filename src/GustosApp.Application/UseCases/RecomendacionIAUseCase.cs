using GustosApp.Application.Interfaces;
using GustosApp.Application.Services;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GustosApp.Application.UseCases
{
    public class RecomendacionIAUseCase
    {

        private readonly IRecomendacionAIService _ai;


        public RecomendacionIAUseCase(IRecomendacionAIService ai)
        {
            _ai = ai;
        }

        public async Task<string> Handle(Usuario usuario, Restaurante restaurante, CancellationToken cancellationToken)
        {
            var prompt = ConstruirPrompt(usuario, restaurante);

            var explicacion = await _ai.GenerarRecomendacion(prompt);

            return explicacion;
        }

        private string ConstruirPrompt(Usuario usuario, Restaurante restaurante)
        {
            var gustosUsuario = usuario.Gustos.Select(g => g.Nombre).ToList();
            var restriccionesUsuario = usuario.Restricciones.Select(r => r.Nombre).ToList();
            var condicionesUsuario = usuario.CondicionesMedicas.Select(c => c.Nombre).ToList();

            var gustosRestaurante = restaurante.GustosQueSirve.Select(g => g.Nombre).ToList();
            var restriccionesRestaurante = restaurante.RestriccionesQueRespeta.Select(r => r.Nombre).ToList();
            var estadoMenu = restaurante.MenuProcesado ? "Sí (Menú detallado disponible)" : "No (Solo datos generales)";

            return $@"Sos una IA que recomienda restaurantes basándote en preferencias reales del usuario.
            USUARIO:
            - Nombre: {usuario.Nombre} {usuario.Apellido}
            - Gustos: {string.Join(", ", gustosUsuario)}
            - Restricciones: {string.Join(", ", restriccionesUsuario)}
             - Condiciones médicas: {string.Join(", ", condicionesUsuario)}

            RESTAURANTE:
            - Nombre: {restaurante.Nombre}
            - Ofrece: {string.Join(", ", gustosRestaurante)}
            - Respeta: {string.Join(", ", restriccionesRestaurante)}
            - Valoración media: {restaurante.Rating?.ToString("N1") ?? "Sin Reseñas"} 
            - Cantidad de Reseñas: {restaurante.CantidadResenas ?? 0} 
            - Disponibilidad del Menú Detallado: {estadoMenu}
            - Horarios (JSON Parcial): {restaurante.HorariosJson.Substring(0, Math.Min(restaurante.HorariosJson.Length, 100))} 

Basado en el perfil del usuario, la oferta y la valoración del restaurante, explicále al usuario si este restaurante es una buena coincidencia. **Incluye una mención a la valoración y al horario si es relevant";
        }
    }

}