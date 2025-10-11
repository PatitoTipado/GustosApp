
using FluentValidation;
using GustosApp.Application.DTOs.Restaurantes;

namespace GustosApp.Application.Validations.Restaurantes
{
    public class CrearRestauranteValidator : AbstractValidator<CrearRestauranteDto>
    {
        public CrearRestauranteValidator()
        {
            RuleFor(x => x.Nombre).NotEmpty().MinimumLength(3).MaximumLength(160);
            RuleFor(x => x.Direccion).NotEmpty().MaximumLength(300);
            RuleFor(x => x.Latitud).InclusiveBetween(-90, 90);
            RuleFor(x => x.Longitud).InclusiveBetween(-180, 180);
        }
    }

    public class ActualizarRestauranteValidator : AbstractValidator<ActualizarRestauranteDto>
    {
        public ActualizarRestauranteValidator()
        {
            RuleFor(x => x.Nombre).NotEmpty().MinimumLength(3).MaximumLength(160);
            RuleFor(x => x.Direccion).NotEmpty().MaximumLength(300);
            RuleFor(x => x.Latitud).InclusiveBetween(-90, 90);
            RuleFor(x => x.Longitud).InclusiveBetween(-180, 180);
        }
    }
}
