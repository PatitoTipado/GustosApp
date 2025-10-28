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
            var grupo = await _grupoRepository.GetByIdAsync(grupoId, cancellationToken);
            if (grupo == null)
                throw new ArgumentException("Grupo no encontrado");

            bool esMiembro = grupo.Miembros.Any(m => m.Usuario.FirebaseUid == firebaseUid);
            if (!esMiembro)
                throw new UnauthorizedAccessException("No pertenece a este grupo");

            return await _chatRepository.GetMessagesByGrupoIdAsync(grupoId, cancellationToken);

        }
    }

       
        
        public class EnviarMensajeGrupoUseCase
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

            public async Task<ChatMessage> HandleAsync( string firebaseUid,Guid grupoId,string mensaje
                ,CancellationToken cancellationToken = default)
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

                var chat = new ChatMessage
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
        }
    }
   
