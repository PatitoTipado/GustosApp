using FluentValidation;
using GustosApp.API.DTO;
namespace GustosApp.API.Validations.OpinionRestaurantes
{


    public class CrearOpinionRestauranteValidator : AbstractValidator<CrearOpinionRestauranteRequest>
    {
        private const long MaxSize = 3 * 1024 * 1024; // 3MB
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };

        public CrearOpinionRestauranteValidator()
        {
            // VALORACIÓN
            RuleFor(x => x.Valoracion)
                .InclusiveBetween(1.0, 5.0)  
                .WithMessage("La valoración debe estar entre 1 y 5.");

            // OPINIÓN (OBLIGATORIA)
            RuleFor(x => x.Opinion)
                .NotEmpty().WithMessage("La opinión es obligatoria.")
                .MinimumLength(5).WithMessage("La opinión debe tener al menos 5 caracteres.")
                .MaximumLength(2000).WithMessage("La opinión no puede superar los 2000 caracteres.");

            // TÍTULO (OBLIGATORIO)
            RuleFor(x => x.Titulo)
                .NotEmpty().WithMessage("El título es obligatorio.")
                .MinimumLength(3).WithMessage("El título debe tener al menos 3 caracteres.")
                .MaximumLength(100).WithMessage("El título no puede superar los 100 caracteres.");

            // FECHA VISITA (OBLIGATORIA)
            RuleFor(x => x.FechaVisita)
                .NotNull().WithMessage("La fecha de visita es obligatoria.")
                .LessThanOrEqualTo(DateTime.UtcNow)
                .WithMessage("La fecha de visita no puede ser futura.");

            // MOTIVO VISITA (OBLIGATORIO)
            RuleFor(x => x.MotivoVisita)
                .NotEmpty().WithMessage("El motivo de la visita es obligatorio.")
                .MinimumLength(3).WithMessage("El motivo debe tener al menos 3 caracteres.");

            RuleFor(x => x.Imagenes)
             .Must(list => list != null && list.Count >= 1 && list.Count <= 3)
             .WithMessage("Debe subir entre 1 y 3 imágenes.");

            RuleForEach(x => x.Imagenes)
                .Must(f => f != null && f.Length <= MaxSize)
                .WithMessage("Una imagen supera los 3MB.")
                .Must(f => f != null && AllowedExtensions.Contains(Path.GetExtension(f.FileName).ToLower()))
                .WithMessage("Formato de imagen inválido (solo jpg, jpeg, png o webp).");

        }
    }

}
