using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GustosApp.Domain.Model;

namespace GustosApp.Application.Tests.mocks
{
    public class UsuarioFake : Usuario
    {
        public UsuarioFake(string firebaseUid, string email, string nombre, string apellido, string idUsuario)
            : base(firebaseUid, email, nombre, apellido, idUsuario)
        {
            Gustos = new List<Gusto>();
            Restricciones = new List<Restriccion>();
            CondicionesMedicas = new List<CondicionMedica>();
        }

        public override List<string> ValidarCompatibilidad()
        {
            // Retorna valores fijos o controlados
            return new List<string> { "Pizza" };
        }
    }
}
