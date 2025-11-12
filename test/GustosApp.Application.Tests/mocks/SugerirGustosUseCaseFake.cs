using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GustosApp.Application.UseCases.RestauranteUseCases;
using GustosApp.Domain.Model;

namespace GustosApp.Application.Tests.mocks
{
    public class SugerirGustosUseCaseFake : SugerirGustosUseCase
    {
        public SugerirGustosUseCaseFake() : base(null!, null!, null!) { }

        public override Task<List<Restaurante>> Handle(UsuarioPreferencias usuario, int maxResults = 10, CancellationToken ct = default)
        {
            return Task.FromResult(new List<Restaurante>
        {
            new Restaurante { Id = Guid.NewGuid(), Nombre = "Pizza Loca" },
            new Restaurante { Id = Guid.NewGuid(), Nombre = "La Napo" }
        });
        }
    }

}
