using GustosApp.Application.DTO;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GustosApp.Application.UseCases.GrupoUseCases
{
    public class ActualizarGustosAGrupoUseCase
    {
        private IGrupoRepository _grupoRepository;
        private IGustoRepository _gustoRepository;
        private IGustosGrupoRepository _gustosGrupoRepository;
        private IUsuarioRepository _usuarioRepository;
        private IMiembroGrupoRepository _miembroGrupoRepository;

        public ActualizarGustosAGrupoUseCase(
            IGrupoRepository grupoRepository,
            IGustoRepository gustoRepository,
            IGustosGrupoRepository gustosGrupoRepository,
            IUsuarioRepository usuarioRepository,
            IMiembroGrupoRepository miembroGrupoRepository)
        {
            _grupoRepository = grupoRepository;
            _gustoRepository = gustoRepository;
            _gustosGrupoRepository = gustosGrupoRepository;
            _usuarioRepository = usuarioRepository;
            _miembroGrupoRepository = miembroGrupoRepository;
        }

        public async Task<bool> Handle(List<string>gustosDeUsuario, Guid grupoId,string firebaseUid)
        {
            //in first place we should validate that user is member of the group

            var miembroGrupo = await _usuarioRepository.GetByFirebaseUidAsync(firebaseUid);

            if (miembroGrupo == null)
            {
                throw new UnauthorizedAccessException("El usuario no existe.");
            }

            //in second place francia

            if ( !(await _grupoRepository.ExistsAsync(grupoId)))
            {
                throw new KeyNotFoundException("el grupo no existe.");
            }

            if (!(await _miembroGrupoRepository.UsuarioEsMiembroActivoAsync(grupoId,miembroGrupo.Id)))
            {
                throw new UnauthorizedAccessException("El miembro no es un usuario activo (expulsado)");
            }

            //validamos que los gustos existan

            List<Gusto> gustos = await _gustoRepository.obtenerGustosPorNombre(gustosDeUsuario);

            if (gustos == null || gustos.Count() == 0)
            {
                throw new KeyNotFoundException("No existen los gustos mencionados.");
            }

            return await _gustosGrupoRepository.AgregarGustosAlGrupo(grupoId,gustos, miembroGrupo.Id);
        }
    }
}
