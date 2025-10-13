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
    public class ObtenerChatGrupoUseCase
    {
        private readonly IChatRepository _chatRepository;
        private readonly IGrupoRepository _grupoRepository;

        public ObtenerChatGrupoUseCase(IChatRepository chatRepository, IGrupoRepository grupoRepository)
        {
            _chatRepository = chatRepository;
            _grupoRepository = grupoRepository;
        }

        public async Task<IEnumerable<ChatMessage>> HandleAsync(string firebaseUid, Guid grupoId, CancellationToken cancellationToken = default)
        {
            // TODO: verify user is member of grupoId if desired
            var messages = await _chatRepository.GetMessagesByGrupoIdAsync(grupoId, cancellationToken);
            return messages;
        }
    }

    public class EnviarMensajeGrupoUseCase
    {
        private readonly IChatRepository _chatRepository;
        private readonly IUsuarioRepository _usuarioRepository;

        public EnviarMensajeGrupoUseCase(IChatRepository chatRepository, IUsuarioRepository usuarioRepository)
        {
            _chatRepository = chatRepository;
            _usuarioRepository = usuarioRepository;
        }

        public async Task<ChatMessage> HandleAsync(string firebaseUid, Guid grupoId, string mensaje, CancellationToken cancellationToken = default)
        {
            var usuario = await _usuarioRepository.GetByFirebaseUidAsync(firebaseUid, cancellationToken);
            if (usuario == null) throw new UnauthorizedAccessException("Usuario no encontrado");

            var chat = new ChatMessage
            {
                Id = Guid.NewGuid(),
                GrupoId = grupoId,
                UsuarioId = usuario.Id,
                UsuarioNombre = usuario.Nombre + " " + usuario.Apellido,
                Mensaje = mensaje,
                FechaEnvio = DateTime.UtcNow
            };

            var saved = await _chatRepository.AddMessageAsync(chat, cancellationToken);
            return saved;
        }
    }
}
