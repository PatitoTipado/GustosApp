using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GustosApp.Application.Interfaces;

namespace GustosApp.Infraestructure.Services
{
    public class UsuariosActivosService : IUsuariosActivosService
    {
        private static readonly ConcurrentDictionary<string, DateTime> _activos = new();

        public void UsuarioConectado(string firebaseUid)
        {
            _activos[firebaseUid] = DateTime.UtcNow;
        }

        public void UsuarioDesconectado(string firebaseUid)
        {
            _activos.TryRemove(firebaseUid, out _);
        }

        public IReadOnlyCollection<string> GetUsuariosActivos()
        {
            return _activos.Keys.ToList();
        }
    }

}
