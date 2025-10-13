using FluentValidation;
using GustosApp.Application.DTOs.Restaurantes;
using GustosApp.Domain.Model;
using System;

namespace GustosApp.Application.Validations.Restaurantes
{
    public class CrearRestauranteValidator : AbstractValidator<CrearRestauranteDto>
    {
        public CrearRestauranteValidator()
        {
            RuleFor(x => x.Nombre)
                .NotEmpty()
                .MinimumLength(3)
                .MaximumLength(160);

            RuleFor(x => x.Direccion)
                .NotEmpty()
                .MaximumLength(300);

            RuleFor(x => x.Latitud).InclusiveBetween(-90, 90);
            RuleFor(x => x.Longitud).InclusiveBetween(-180, 180);

            RuleFor(x => x.Tipo)
                .NotEmpty()
                .Must(t => Enum.TryParse<TipoRestaurante>(t, true, out _))
                .WithMessage("Tipo inválido.");

            When(x => x.Platos != null, () =>
            {
                RuleForEach(x => x.Platos!)
                    .Must(p => Enum.TryParse<PlatoComida>(p, true, out _))
                    .WithMessage("Plato inválido.");
            });

            RuleFor(x => x.ImagenUrl)
                .MaximumLength(500)
                .Must(u => string.IsNullOrWhiteSpace(u) || Uri.IsWellFormedUriString(u, UriKind.Absolute))
                .WithMessage("ImagenUrl no es una URL válida.");

            RuleFor(x => x.Valoracion)
                .InclusiveBetween(0, 5)
                .When(x => x.Valoracion.HasValue)
                .WithMessage("Valoración debe estar entre 0 y 5.");
        }
    }
}
