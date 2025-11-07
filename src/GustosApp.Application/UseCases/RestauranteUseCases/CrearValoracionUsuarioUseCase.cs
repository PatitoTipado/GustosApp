using GustosApp.Application.Interfaces;
using GustosApp.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GustosApp.Application.UseCases.RestauranteUseCases
{
    public class CrearValoracionUsuarioUseCase
    {
        private readonly IValoracionUsuarioRepository _repository;

        public CrearValoracionUsuarioUseCase(IValoracionUsuarioRepository repository)
        {
            _repository = repository;
        }

        public async Task HandleAsync(Guid usuarioId, Guid restauranteId, int valoracionUsuario, string? comentario, CancellationToken cancellationToken)
        {
            if (valoracionUsuario < 1 || valoracionUsuario > 5)
            {
                throw new ArgumentException("La valoracion debe estar entre 1 y 5");

            }
            var existe = await _repository.ExisteValoracionAsync(usuarioId, restauranteId, cancellationToken);
            if (existe)
            {
                throw new ArgumentException("El usuario ya valoro a este restaurante");
            }
            var valoracion = new Valoracion(usuarioId, restauranteId, valoracionUsuario, comentario);

            await _repository.CrearAsync(valoracion, cancellationToken);

        }
    }
}

