
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GustosApp.Application.DTO;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;

namespace GustosApp.Application.UseCases
{
    public class ObtenerAmigosUseCase
    {
        private readonly ISolicitudAmistadRepository _solicitudRepository;
        private readonly IUsuarioRepository _usuarioRepository;

        public ObtenerAmigosUseCase(ISolicitudAmistadRepository solicitudRepository, IUsuarioRepository usuarioRepository)
        {
            _solicitudRepository = solicitudRepository;
            _usuarioRepository = usuarioRepository;
        }

        public async Task<IEnumerable<Usuario>> HandleAsync(string firebaseUid, CancellationToken cancellationToken = default)
        {
            var usuario = await _usuarioRepository.GetByFirebaseUidAsync(firebaseUid, cancellationToken);
            if (usuario == null) throw new UnauthorizedAccessException("Usuario no encontrado");

            return  await _solicitudRepository.GetAmigosByUsuarioIdAsync(usuario.Id, cancellationToken);

            
        }
    }

    // UseCase para eliminar (deshacer) una amistad
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

            // Buscar amistad aceptada en ambos sentidos
            var amistad = await _solicitudRepository.GetAmistadEntreUsuariosAsync(usuario.Id, amigoId, cancellationToken);
            if (amistad == null)
                throw new ArgumentException("No se encontró una amistad activa con el usuario especificado");

            await _solicitudRepository.DeleteAsync(amistad.Id, cancellationToken);
            return true;
        }
    
    }
}