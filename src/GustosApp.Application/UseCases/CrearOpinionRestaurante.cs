using GustosApp.Application.Interfaces;
using GustosApp.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GustosApp.Application.UseCases.RestauranteUseCases
{
    public class CrearOpinionRestaurante
    {
        private readonly IOpinionRestauranteRepository _repository;

        public CrearOpinionRestaurante(IOpinionRestauranteRepository repository)
        {
            _repository = repository;
        }

<<<<<<<< HEAD:src/GustosApp.Application/UseCases/CrearOpinionRestaurante.cs
        public async Task HandleAsync(Guid usuarioId,Guid restauranteId,int valoracionUsuario,string opinion,string titulo,string img,CancellationToken cancellationToken)
========
        public async Task HandleAsync(Guid usuarioId, Guid restauranteId, int valoracionUsuario, string? comentario, CancellationToken cancellationToken)
>>>>>>>> develop:src/GustosApp.Application/UseCases/RestauranteUseCases/CrearValoracionUsuarioUseCase.cs
        {
            if (valoracionUsuario < 1 || valoracionUsuario > 5)
            {
                throw new ArgumentException("La valoracion debe estar entre 1 y 5");

            }
<<<<<<<< HEAD:src/GustosApp.Application/UseCases/CrearOpinionRestaurante.cs
                var existe = await _repository.ExisteValoracionAsync(usuarioId,restauranteId,cancellationToken);
                if (existe)
                {
                    throw new ArgumentException("El usuario ya valoro a este restaurante");
                }
                var opinionRestaurante = new OpinionRestaurante(usuarioId, restauranteId, valoracionUsuario, opinion,titulo,img);

                await _repository.CrearAsync(opinionRestaurante, cancellationToken);

========
            var existe = await _repository.ExisteValoracionAsync(usuarioId, restauranteId, cancellationToken);
            if (existe)
            {
                throw new ArgumentException("El usuario ya valoro a este restaurante");
>>>>>>>> develop:src/GustosApp.Application/UseCases/RestauranteUseCases/CrearValoracionUsuarioUseCase.cs
            }
            var valoracion = new Valoracion(usuarioId, restauranteId, valoracionUsuario, comentario);

            await _repository.CrearAsync(valoracion, cancellationToken);

        }
    }
}

