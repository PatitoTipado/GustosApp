using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GustosApp.Domain.Interfaces;

namespace GustosApp.Application.UseCases
{
    public class EliminarAmigoUseCase
    {
        private readonly ISolicitudAmistadRepository _solicitudRepository;
        private readonly IUsuarioRepository _usuarioRepository;

        public EliminarAmigoUseCase(ISolicitudAmistadRepository solicitudRepository, IUsuarioRepository usuarioRepository)
        {
            _solicitudRepository = solicitudRepository;
            _usuarioRepository = usuarioRepository;
        }

        public async Task<bool> HandleAsync(string firebaseUid, Guid amigoId, CancellationToken cancellationToken = default)
        {
            var usuario = await _usuarioRepository.GetByFirebaseUidAsync(firebaseUid, cancellationToken);
            if (usuario == null)
                throw new UnauthorizedAccessException("Usuario no encontrado");

            if (usuario.Id == amigoId)
                throw new ArgumentException("No podés eliminarte a vos mismo como amigo.");


            // Buscar amistad aceptada en ambos sentidos
            var amistad = await _solicitudRepository.GetAmistadEntreUsuariosAsync(usuario.Id, amigoId, cancellationToken);
            if (amistad == null)
                throw new ArgumentException("No se encontró una amistad activa con el usuario especificado");

            await _solicitudRepository.DeleteAsync(amistad.Id, cancellationToken);
            return true;
        }

    }
    
}
