
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GustosApp.Application.DTO;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;

namespace GustosApp.Application.UseCases.AmistadUseCases
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

            return await _solicitudRepository.GetAmigosByUsuarioIdAsync(usuario.Id, cancellationToken);


        }
    }



}