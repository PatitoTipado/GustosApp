using FluentValidation;
using GustosApp.API.DTO;
using GustosApp.Domain.Model.@enum;
using System;
using System.Globalization;

namespace GustosApp.Application.Validations.Restaurantes
{
    public class CrearSolicitudRestauranteValidator : AbstractValidator<CrearRestauranteDto>
    {
        private const long MaxSize = 2 * 1024 * 1024;
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };

        public CrearSolicitudRestauranteValidator()
        {
            // ===========================
            // NOMBRE
            // ===========================
            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("El nombre es obligatorio.")
                .MinimumLength(3)
                .MaximumLength(160);

            // ===========================
            // DIRECCIÓN
            // ===========================
            RuleFor(x => x.Direccion)
                .NotEmpty()
                .MaximumLength(300);

            // ===========================
            // WEBSITE
            // ===========================
            RuleFor(x => x.WebsiteUrl)
                .NotEmpty().WithMessage("El sitio web es obligatorio.");

            // ===========================
            // COORDENADAS
            // ===========================
            RuleFor(x => x.Lat)
                .NotEmpty().WithMessage("La latitud es obligatoria.")
                .Must(v => double.TryParse(v?.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out _))
                .WithMessage("Latitud inválida.");

            RuleFor(x => x.Lng)
                .NotEmpty().WithMessage("La longitud es obligatoria.")
                .Must(v => double.TryParse(v?.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out _))
                .WithMessage("Longitud inválida.");

            // ===========================
            // GUSTOS (mínimo 1)
            // ===========================
            RuleFor(x => x.GustosQueSirveIds)
                .NotNull().WithMessage("Debe seleccionar al menos un gusto.")
                .Must(list =>list != null && list.Count >= 1)
                .WithMessage("Debe seleccionar al menos un gusto.");

            // ===========================
            // IMAGEN DESTACADA (1 obligatoria)
            // ===========================
            RuleFor(x => x.ImagenDestacada)
                .NotNull().WithMessage("Debe subir una imagen destacada.")
                .Must(f => f == null || f.Length <= MaxSize).WithMessage("La imagen destacada supera los 2MB.")
                .Must(f => f == null || AllowedExtensions.Contains(Path.GetExtension(f.FileName).ToLower()))
                .WithMessage("Formato inválido (solo jpg, png, webp).");

            // ===========================
            // LOGO (1 obligatorio)
            // ===========================
            RuleFor(x => x.Logo)
                .NotNull().WithMessage("Debe subir un logo.")
                .Must(f => f == null || f.Length <= MaxSize).WithMessage("El logo supera los 2MB.")
                .Must(f => f == null || AllowedExtensions.Contains(Path.GetExtension(f.FileName).ToLower()))
                .WithMessage("Formato inválido en el logo.");

            // ===========================
            // MENÚ (1 obligatorio)
            // ===========================
            RuleFor(x => x.ImagenMenu)
                .NotNull().WithMessage("Debe subir una imagen del menú.")
                .Must(f => f == null || f.Length <= MaxSize).WithMessage("La imagen del menú supera los 2MB.")
                .Must(f => f == null || AllowedExtensions.Contains(Path.GetExtension(f.FileName).ToLower()))
                .WithMessage("Formato inválido en la imagen del menú.");

            // ===========================
            // INTERIOR (mínimo 1, máximo 3)
            // ===========================
            RuleFor(x => x.ImagenesInterior)
              .Must(list => list != null && list.Count >= 1 && list.Count <= 3)
              .WithMessage("Debe subir entre 1 y 3 imágenes interiores.");

            RuleForEach(x => x.ImagenesInterior)
                .Must(f => f != null && f.Length <= MaxSize)
                .WithMessage("Una imagen interior supera los 2MB.")
                .Must(f => f != null && AllowedExtensions.Contains(Path.GetExtension(f.FileName).ToLower()))
                .WithMessage("Formato inválido en imagen interior.");


            // ===========================
            // COMIDAS (mínimo 1, máximo 3)
            // ===========================
            RuleFor(x => x.ImagenesComidas)
             .Must(list => list != null && list.Count >= 1 && list.Count <= 3)
             .WithMessage("Debe subir entre 1 y 3 imágenes de comida.");

            RuleForEach(x => x.ImagenesComidas)
                .Must(f => f != null && f.Length <= MaxSize)
                .WithMessage("Una imagen de comida supera los 2MB.")
                .Must(f => f != null && AllowedExtensions.Contains(Path.GetExtension(f.FileName).ToLower()))
                .WithMessage("Formato inválido en imagen de comida.");

        }
    }

    
}