using GustosApp.Application.Services;
using GustosApp.Application.UseCases.GrupoUseCases;

namespace GustosApp.Application.Handlers
{
    public class ServicioPreferenciasGrupos: IServicioPreferenciasGrupos
    {
        private readonly ActualizarGustosAGrupoUseCase _actualizarGustosGrupoUseCase;
        private readonly EliminarGustosGrupoUseCase _eliminarGustosGrupoUseCase;
        private readonly DesactivarMiembroDeGrupoUseCase _desacivarMiembroDeGrupoUseCase;
        private readonly ActivarMiembroDeGrupoUseCase _activarMiembroDelGrupoUseCase;

        public ServicioPreferenciasGrupos(
            ActualizarGustosAGrupoUseCase actualizarGustosGrupoUseCase,
            EliminarGustosGrupoUseCase eliminarGustosGrupoUseCase,
            DesactivarMiembroDeGrupoUseCase desacivarMiembroDeGrupoUseCase,
            ActivarMiembroDeGrupoUseCase activarMiembroDeGrupoUseCase)
        {
            _actualizarGustosGrupoUseCase = actualizarGustosGrupoUseCase;
            _eliminarGustosGrupoUseCase = eliminarGustosGrupoUseCase;
            _desacivarMiembroDeGrupoUseCase = desacivarMiembroDeGrupoUseCase;
            _activarMiembroDelGrupoUseCase = activarMiembroDeGrupoUseCase;
        }

        public Task<bool> ActivarMiembro(Guid grupoId, Guid usuarioId, string firebaseUid)
        {
            return _activarMiembroDelGrupoUseCase.Handle(grupoId, usuarioId, firebaseUid);
        }

        public Task<bool> ActualizarGustosDeGrupo(List<string> gustosDeUsuario, Guid grupoId, string firebaseUid)
        {
            return _actualizarGustosGrupoUseCase.Handle(gustosDeUsuario, grupoId, firebaseUid);
        }

        public Task<bool> DesactivarMiembroDeGrupo(Guid grupoId, Guid usuarioId, string firebaseUid)
        {
            return _desacivarMiembroDeGrupoUseCase.Handle(grupoId, usuarioId, firebaseUid);
        }

        public Task<bool> EliminarGustosDeGrupo(List<string> gustosDeUsuario,Guid grupoId, string firebaseuid)
        {
            return _eliminarGustosGrupoUseCase.Handle(gustosDeUsuario,grupoId,firebaseuid);
        }
    }
}
