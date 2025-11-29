using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;

namespace GustosApp.Application.UseCases.GrupoUseCases.ChatGrupoUseCases
{
    public class EnviarMensajeGrupoUseCase : IEnviarMensajeGrupoUseCase
    {
        private readonly IChatRepository _chatRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IGrupoRepository _grupoRepository;


        public EnviarMensajeGrupoUseCase(IChatRepository chatRepository, IUsuarioRepository usuarioRepository,
            IGrupoRepository grupoRepository)
        {
            _chatRepository = chatRepository;
            _usuarioRepository = usuarioRepository;
            _grupoRepository = grupoRepository;
        }

        public async Task<ChatMensaje> HandleAsync(string firebaseUid, Guid grupoId, string mensaje
            , CancellationToken cancellationToken = default)
        {

            var usuario = await _usuarioRepository.GetByFirebaseUidAsync(firebaseUid, cancellationToken);
            if (usuario == null)
                throw new UnauthorizedAccessException("Usuario no encontrado");


            var grupo = await _grupoRepository.GetByIdAsync(grupoId, cancellationToken);
            if (grupo == null)
                throw new ArgumentException("El grupo no existe.");


            var esMiembro = grupo.Miembros?.Any(m => m.UsuarioId == usuario.Id) ?? false;
            if (!esMiembro)
                throw new UnauthorizedAccessException("No pertenecés a este grupo.");


            if (string.IsNullOrWhiteSpace(mensaje))
                throw new ArgumentException("El mensaje no puede estar vacío.");

            var chat = new ChatMensaje
            {
                Id = Guid.NewGuid(),
                GrupoId = grupoId,
                UsuarioId = usuario.Id,
                UsuarioNombre = $"{usuario.Nombre} {usuario.Apellido}",
                Mensaje = mensaje.Trim(),
                FechaEnvio = DateTime.UtcNow
            };

            var saved = await _chatRepository.AddMessageAsync(chat, cancellationToken);
            return saved;
        }
        public async Task<ChatMensaje> HandleSystemMessageAsync(Guid grupoId, string mensaje, CancellationToken cancellationToken = default)
        {
            var chat = new ChatMensaje
            {
                Id = Guid.NewGuid(),
                GrupoId = grupoId,
                UsuarioId = Guid.Empty,               
                UsuarioNombre = "Sistema",
                Mensaje = mensaje,
                FechaEnvio = DateTime.UtcNow
                
                                    
            };

            return await _chatRepository.AddMessageAsync(chat, cancellationToken);
        }

    }
}
