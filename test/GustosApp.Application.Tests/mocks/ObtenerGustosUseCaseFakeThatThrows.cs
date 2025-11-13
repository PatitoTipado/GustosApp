using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GustosApp.Application.UseCases.UsuarioUseCases.GustoUseCases;
using GustosApp.Domain.Model;

namespace GustosApp.Application.Tests.mocks
{
    public class ObtenerGustosUseCaseFakeThatThrows : ObtenerGustosUseCase
    {
        private int _counter = 0;

        public ObtenerGustosUseCaseFakeThatThrows() : base(null!) { }

        public override Task<UsuarioPreferencias> HandleAsync(string firebaseUid, CancellationToken ct = default, List<string>? gustos = null)
        {
            _counter++;
            if (_counter == 1)
                throw new Exception("Error controlado para test");

            return Task.FromResult(new UsuarioPreferencias
            {
                Gustos = new List<string> { "Sushi" },
                Restricciones = new List<string>(),
                CondicionesMedicas = new List<string>()
            });
        }
    }

}
