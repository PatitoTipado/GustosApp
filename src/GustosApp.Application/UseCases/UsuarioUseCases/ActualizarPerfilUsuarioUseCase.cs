using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GustosApp.Application.Interfaces;
using GustosApp.Domain.Common;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;

namespace GustosApp.Application.UseCases.UsuarioUseCases
{
    public class ActualizarPerfilUsuarioUseCase
    {
        private readonly IUsuarioRepository _usuarios;
        private readonly IFileStorageService _files;
        private readonly IFirebaseAuthService _firebase;

        public ActualizarPerfilUsuarioUseCase(
            IUsuarioRepository usuarios,
            IFileStorageService files,
            IFirebaseAuthService firebase)
        {
            _usuarios = usuarios;
            _files = files;
            _firebase = firebase;
        }

        public async Task<Usuario> HandleAsync(
            string firebaseUid, string? email, string? nombre, string? apellido,
            FileUpload? fotoPerfil, bool esPrivado
 ,
            CancellationToken ct = default)
        {
           
            var usuario = await _usuarios.GetByFirebaseUidAsync(firebaseUid, ct);
            if (usuario == null)
                throw new Exception("Usuario no encontrado");

       
            if (!string.IsNullOrWhiteSpace(email) && email != usuario.Email)
            {
                await _firebase.UpdateUserEmailAsync(firebaseUid, email);
                usuario.Email = email;
            }

        
            if (!string.IsNullOrWhiteSpace(nombre))
                usuario.Nombre = nombre;

            if (!string.IsNullOrWhiteSpace(apellido))
                usuario.Apellido = apellido;

            usuario.EsPrivado = esPrivado;

          
            if (fotoPerfil != null)
            {
              
                if (!string.IsNullOrWhiteSpace(usuario.FotoPerfilUrl))
                    await _files.DeleteFileAsync(usuario.FotoPerfilUrl);

                var newUrl = await _files.UploadFileAsync(
                    fotoPerfil.Content,
                   fotoPerfil.FileName,
                    "perfil"
                );

                usuario.FotoPerfilUrl = newUrl;
            }


            await _usuarios.SaveChangesAsync(ct);

            return usuario;
        }
    }
    }
