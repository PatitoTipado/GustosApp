using FluentValidation;
using GustosApp.Application.DTOs.Restaurantes;
using GustosApp.Domain.Model.@enum;
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

                        // Coordenadas: exigir lat/lng o latitud/longitud, y validar rango
            RuleFor(x => x).Custom((dto, ctx) =>
            {
                var lat = dto.Lat ?? dto.Latitud;
                var lng = dto.Lng ?? dto.Longitud;

                if (lat is null || lng is null)
                {
                    ctx.AddFailure("Coordenadas", "Lat/Lng requeridos (puede ser 'lat/lng' o 'latitud/longitud').");
                    return;
                }
                if (lat < -90 || lat > 90)
                    ctx.AddFailure("Lat/Lng", "Lat fuera de rango (-90 a 90).");
                if (lng < -180 || lng > 180)
                    ctx.AddFailure("Lat/Lng", "Lng fuera de rango (-180 a 180).");
            });

            RuleFor(x => x.PrimaryType)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("primaryType requerido.")
                .MaximumLength(80)
                .Matches("^[a-z0-9_]+$").WithMessage("primaryType debe usar minúsculas, números y '_' (ej: pizza_restaurant).");

            RuleForEach(x => x.Types)
                .MaximumLength(80)
                .Matches("^[a-z0-9_]+$").WithMessage("Cada type debe usar minúsculas, números y '_' (ej: fast_food_restaurant).");

            When(x => x.Platos != null, () =>
            {
                RuleForEach(x => x.Platos!)
                    .Must(p => Enum.TryParse<PlatoComida>(p, true, out _))
                    .WithMessage("Plato inválido.");
            });

            RuleFor(x => x.ImagenUrl)
                .MaximumLength(2048)
                .Must(u => string.IsNullOrWhiteSpace(u) || Uri.IsWellFormedUriString(u, UriKind.Absolute))
                .WithMessage("ImagenUrl no es una URL válida.");

            RuleFor(x => x.Valoracion)
                .InclusiveBetween(0, 5)
                .When(x => x.Valoracion.HasValue)
                .WithMessage("Valoración debe estar entre 0 y 5.");
        }
    }
}
