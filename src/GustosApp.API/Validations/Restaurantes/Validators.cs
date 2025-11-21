using FluentValidation;
using GustosApp.API.DTO;
using GustosApp.Domain.Model.@enum;
using System;

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
            // COORDENADAS
            // ===========================
            RuleFor(x => x.Lat)
                .NotNull().WithMessage("Latitud requerida.")
                .InclusiveBetween(-90, 90);

            RuleFor(x => x.Lng)
                .NotNull().WithMessage("Longitud requerida.")
                .InclusiveBetween(-180, 180);

          

     

            // ===========================
            // GUSTOS / RESTRICCIONES
            // ===========================
            RuleFor(x => x.GustosQueSirveIds)
                .NotNull();

            RuleFor(x => x.RestriccionesQueRespetaIds)
                .NotNull();

            // ===========================
            // IMÁGENES
            // ===========================
            RuleFor(x => x.ImagenDestacada)
                .NotNull().WithMessage("Debe subir una imagen destacada.");

            RuleFor(x => x.ImagenesInterior)
                .Must(list => list == null || list.Count <= 3)
                .WithMessage("Máximo 3 imágenes interiores.");

            RuleFor(x => x.ImagenesComidas)
                .Must(list => list == null || list.Count <= 3)
                .WithMessage("Máximo 3 imágenes de comida.");

            RuleFor(x => x.ImagenMenu)
                .NotNull().WithMessage("Debe subir una imagen del menú.");

            RuleFor(x => x.Logo)
                .NotNull().WithMessage("Debe subir un logo del restaurante.");


            RuleFor(x => x.ImagenDestacada)
            .NotNull().WithMessage("Debe subir una imagen destacada.")
          .Must(f => f.Length <= MaxSize).WithMessage("La imagen destacada no puede superar los 2MB.")
          .Must(f => AllowedExtensions.Contains(Path.GetExtension(f.FileName).ToLower()))
          .WithMessage("Formato de imagen destacada inválido (solo jpg, png, webp).");

            RuleForEach(x => x.ImagenesInterior)
                .Must(f => f.Length <= MaxSize).WithMessage("Una imagen interior supera los 2MB.")
                .Must(f => AllowedExtensions.Contains(Path.GetExtension(f.FileName).ToLower()))
                .WithMessage("Formato inválido en imagen interior.");

            RuleForEach(x => x.ImagenesComidas)
                .Must(f => f.Length <= MaxSize).WithMessage("Una imagen de comida supera los 2MB.")
                .Must(f => AllowedExtensions.Contains(Path.GetExtension(f.FileName).ToLower()))
                .WithMessage("Formato inválido en imagen de comida.");

            RuleFor(x => x.ImagenMenu)
                .NotNull().WithMessage("Debe subir una imagen del menú.")
                .Must(f => f.Length <= MaxSize).WithMessage("La imagen del menú supera los 2MB.")
                .Must(f => AllowedExtensions.Contains(Path.GetExtension(f.FileName).ToLower()))
                .WithMessage("Formato inválido en la imagen del menú.");

            RuleFor(x => x.Logo)
                .NotNull().WithMessage("Debe subir un logo.")
                .Must(f => f.Length <= MaxSize).WithMessage("El logo supera los 2MB.")
                .Must(f => AllowedExtensions.Contains(Path.GetExtension(f.FileName).ToLower()))
                .WithMessage("Formato inválido en el logo.");

        }
    }

}
