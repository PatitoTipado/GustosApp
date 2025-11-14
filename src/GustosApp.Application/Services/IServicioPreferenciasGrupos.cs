using GustosApp.Application.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GustosApp.Application.Services
{
    public interface IServicioPreferenciasGrupos
    {
        Task<bool> ActivarMiembro(Guid grupoId, Guid usuarioId, string firebaseUid);
        public Task<bool> ActualizarGustosDeGrupo(List<string> gustosDeUsuario, Guid grupoId,string firebaseUid);
        Task<bool> DesactivarMiembroDeGrupo(Guid grupoId, Guid usuarioId, string firebaseUid);
        public Task<bool> EliminarGustosDeGrupo(List<string> gustosDeUsuario,Guid grupoId,string firebaseuid);
    }
}
