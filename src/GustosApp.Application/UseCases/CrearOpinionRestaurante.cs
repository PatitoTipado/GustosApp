using GustosApp.Application.Interfaces;
using GustosApp.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GustosApp.Application.UseCases
{
    public class CrearOpinionRestaurante
    {
        private readonly IOpinionRestauranteRepository _repository;

        public CrearOpinionRestaurante(IOpinionRestauranteRepository repository)
        {
            _repository = repository;
        }

        public async Task HandleAsync(Guid usuarioId,Guid restauranteId,int valoracionUsuario,string opinion,string titulo,string img,CancellationToken cancellationToken)
        {
            if (valoracionUsuario < 1 || valoracionUsuario > 5)
            {
                throw new ArgumentException("La valoracion debe estar entre 1 y 5");

            }
                var existe = await _repository.ExisteValoracionAsync(usuarioId,restauranteId,cancellationToken);
                if (existe)
                {
                    throw new ArgumentException("El usuario ya valoro a este restaurante");
                }
                var opinionRestaurante = new OpinionRestaurante(usuarioId, restauranteId, valoracionUsuario, opinion,titulo,img);

                await _repository.CrearAsync(opinionRestaurante, cancellationToken);

            }
        }
    }

