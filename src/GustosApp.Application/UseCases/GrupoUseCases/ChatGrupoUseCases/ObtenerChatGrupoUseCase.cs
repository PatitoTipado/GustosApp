using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;

namespace GustosApp.Application.UseCases.GrupoUseCases.ChatGrupoUseCases
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

        public async Task<IEnumerable<ChatMensaje>> HandleAsync(string firebaseUid, Guid grupoId, CancellationToken cancellationToken = default)
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


}
