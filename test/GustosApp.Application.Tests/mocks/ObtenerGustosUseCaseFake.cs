using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GustosApp.Application.UseCases.UsuarioUseCases.GustoUseCases;
using GustosApp.Domain.Model;

namespace GustosApp.Application.Tests.mocks
{
    public class ObtenerGustosUseCaseFake : ObtenerGustosUseCase
    {
        public ObtenerGustosUseCaseFake() : base(null!) { }

        public override Task<UsuarioPreferencias> HandleAsync(string firebaseUid, CancellationToken ct = default, List<string>? gustos = null)
        {
            return Task.FromResult(new UsuarioPreferencias
            {
                Gustos = new List<string> { "Pizza" },
                Restricciones = new List<string>(),
                CondicionesMedicas = new List<string>()
            });
        }
    }

}
