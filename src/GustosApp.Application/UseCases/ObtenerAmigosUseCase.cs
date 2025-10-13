
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

        public async Task<IEnumerable<UsuarioSimpleResponse>> HandleAsync(string firebaseUid, CancellationToken cancellationToken = default)
        {
            var usuario = await _usuarioRepository.GetByFirebaseUidAsync(firebaseUid, cancellationToken);
            if (usuario == null) throw new UnauthorizedAccessException("Usuario no encontrado");

            var amigos = await _solicitudRepository.GetAmigosByUsuarioIdAsync(usuario.Id, cancellationToken);

            return amigos.Select(a => new UsuarioSimpleResponse { Id = a.Id, Nombre = a.Nombre + " " + a.Apellido, Email = a.Email, FotoPerfilUrl = a.FotoPerfilUrl }).ToList();
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
            if (usuario == null) throw new UnauthorizedAccessException("Usuario no encontrado");

            // Buscar cualquier solicitud entre usuario.Id y amigoId que esté Aceptada
            var pendientes = await _solicitudRepository.GetSolicitudesEnviadasByUsuarioIdAsync(usuario.Id, cancellationToken);
            var enviada = pendientes.FirstOrDefault(s => s.DestinatarioId == amigoId && s.Estado == EstadoSolicitud.Aceptada);

            if (enviada != null)
            {
                await _solicitudRepository.DeleteAsync(enviada.Id, cancellationToken);
                return true;
            }

            var recibidas = await _solicitudRepository.GetSolicitudesEnviadasByUsuarioIdAsync(amigoId, cancellationToken);
            var reciproca = recibidas.FirstOrDefault(s => s.DestinatarioId == usuario.Id && s.Estado == EstadoSolicitud.Aceptada);
            if (reciproca != null)
            {
                await _solicitudRepository.DeleteAsync(reciproca.Id, cancellationToken);
                return true;
            }

            // No se encontró amistad aceptada
            throw new ArgumentException("No se encontró una amistad activa con el usuario especificado");
        }
    }
}