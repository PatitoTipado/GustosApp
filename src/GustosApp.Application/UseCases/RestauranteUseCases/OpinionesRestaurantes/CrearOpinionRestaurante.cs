using GustosApp.Application.Interfaces;
using GustosApp.Domain.Common;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GustosApp.Application.UseCases.RestauranteUseCases.OpinionesRestaurantes
{
    public class CrearOpinionRestauranteUseCase
    {
        private readonly IFileStorageService _fileStorage;
        private readonly IOpinionRestauranteRepository _repoOpinionRest;
        private readonly IUsuarioRepository _repoUsuario;

        public CrearOpinionRestauranteUseCase(IFileStorageService fileStorage,
            IOpinionRestauranteRepository repoOpinionRest, IUsuarioRepository repoUsuario
           )
        {
            _fileStorage = fileStorage;
            _repoOpinionRest = repoOpinionRest;
            _repoUsuario = repoUsuario;
        }

        public async Task HandleAsync(string firebaseUid, Guid restauranteId,
     double valoracionUsuario, string opinion, string titulo,
     List<FileUpload>? Imagenes, string? motivoVisita,
     DateTime fechaVisita, CancellationToken cancellationToken)
        {
            var usuario = await _repoUsuario.GetByFirebaseUidAsync(firebaseUid, cancellationToken)
                ?? throw new UnauthorizedAccessException("Usuario no encontrado");

            if (valoracionUsuario < 1 || valoracionUsuario > 5)
                throw new ArgumentException("La valoracion debe estar entre 1 y 5");

            var existe = await _repoOpinionRest.ExisteValoracionAsync(usuario.Id, restauranteId, cancellationToken);
            if (existe)
                throw new ArgumentException("El usuario ya valoro a este restaurante");

       
            if (Imagenes == null || !Imagenes.Any())
                throw new ArgumentException("Debes subir al menos una imagen para crear una opinión.");

            var opinionRestaurante = new OpinionRestaurante
            {
                Id = Guid.NewGuid(),
                RestauranteId = restauranteId,
                UsuarioId = usuario.Id,
                Valoracion = valoracionUsuario,
                Opinion = opinion,
                Titulo = titulo,
                FechaVisita = fechaVisita,
                MotivoVisita = motivoVisita,
                FechaCreacion = fechaVisita,
                EsImportada = false
            };

       
            foreach (var file in Imagenes)
            {
                var url = await _fileStorage.UploadFileAsync(
                    file.Content,
                    file.FileName,
                    "opiniones"
                );

                opinionRestaurante.Fotos.Add(new OpinionFoto { Url = url });
            }


            await _repoOpinionRest.CrearAsync(opinionRestaurante, cancellationToken);


            //lugar visitado por usuario

            await _repoUsuario.MarcarRestauranteComoVisitadoAsync(usuario.Id, restauranteId, cancellationToken);
        }

    }
    }


