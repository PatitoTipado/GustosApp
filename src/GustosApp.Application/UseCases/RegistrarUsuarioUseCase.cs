using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GustosApp.Application.DTO;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;

namespace GustosApp.Application.UseCases
{
    public class RegistrarUsuarioUseCase
    {

        private readonly IUsuarioRepository _repo;

        public RegistrarUsuarioUseCase(IUsuarioRepository repo)
        {
            _repo = repo;
        }

        public async Task<UsuarioResponse> HandleAsync (string firebaseUid,RegistrarUsuarioRequest request,
            CancellationToken ct = default)
        {

            var existente= await _repo.GetByFirebaseUidAsync(firebaseUid,ct);

            if(existente  != null)
            {
                return new UsuarioResponse(existente.Id, existente.FirebaseUid, existente.Email, existente.Nombre,existente.Apellido,existente.IdUsuario, existente.FotoPerfilUrl);
            }

            var usuario= new Usuario(firebaseUid,request.Email,request.Nombre,request.Apellido,request.Usuario,request.FotoPerfilUrl);


            await _repo.AddAsync(usuario,ct);
            await _repo.SaveChangesAsync(ct);


            return new UsuarioResponse(usuario.Id, usuario.FirebaseUid, usuario.Email, usuario.Nombre,usuario.Apellido, usuario.IdUsuario, usuario.FotoPerfilUrl);
        }

    }
}
