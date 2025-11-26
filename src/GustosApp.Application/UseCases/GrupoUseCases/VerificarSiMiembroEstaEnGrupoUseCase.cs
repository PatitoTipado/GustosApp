using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GustosApp.Domain.Interfaces;

namespace GustosApp.Application.UseCases.GrupoUseCases
{
    public class VerificarSiMiembroEstaEnGrupoUseCase
    {
        private readonly IGrupoRepository _grupoRepository;
        public VerificarSiMiembroEstaEnGrupoUseCase(IGrupoRepository grupoRepository)
        {
            _grupoRepository = grupoRepository;
        }

        public async Task<bool> HandleAsync(string firebaseUid, Guid grupoID, CancellationToken ct)
        {
            return await _grupoRepository.UsuarioEsMiembroAsync(grupoID, firebaseUid, ct);


        }
    }
}
