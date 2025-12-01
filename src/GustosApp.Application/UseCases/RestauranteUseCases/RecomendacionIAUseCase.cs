using GustosApp.Application.Interfaces;
using GustosApp.Application.Services;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GustosApp.Application.UseCases.RestauranteUseCases
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
            var gustosRestaurante = restaurante.GustosQueSirve.Select(g => g.Nombre).ToList();

            var restriccionesUsuario = usuario.Restricciones.Select(r => r.Nombre).ToList();
            var restriccionesRestaurante = restaurante.RestriccionesQueRespeta.Select(r => r.Nombre).ToList();

            // Cálculo real
            var gustosCoinciden = gustosUsuario.Intersect(gustosRestaurante).ToList();
            var gustosNoCoinciden = gustosUsuario.Except(gustosRestaurante).ToList();

            var restriccionesCumple = restriccionesUsuario.Intersect(restriccionesRestaurante).ToList();
            var restriccionesNoCumple = restriccionesUsuario.Except(restriccionesRestaurante).ToList();

            return
        $@"Quiero que analices si este restaurante es adecuado para el usuario basándote solamente en gustos y restricciones.

            USUARIO:
            - Nombre: {usuario.Nombre} {usuario.Apellido}
- Gustos del usuario: {string.Join(", ", gustosUsuario)}
- Restricciones del usuario: {string.Join(", ", restriccionesUsuario)}

RESTAURANTE:
- Gustos que ofrece: {string.Join(", ", gustosRestaurante)}
- Restricciones que respeta: {string.Join(", ", restriccionesRestaurante)}

ANÁLISIS:
- Gustos que coinciden entre usuario y restaurante: {string.Join(", ", gustosCoinciden)}
- Gustos del usuario que el restaurante no ofrece: {string.Join(", ", gustosNoCoinciden)}
- Restricciones que el restaurante sí cumple: {string.Join(", ", restriccionesCumple)}
- Restricciones que NO cumple: {string.Join(", ", restriccionesNoCumple)}

Con esta información, generá una recomendación clara y simple para el usuario, en texto plano, sin markdown, explicándole si este restaurante es una buena opción o no.";
        }

    }
}