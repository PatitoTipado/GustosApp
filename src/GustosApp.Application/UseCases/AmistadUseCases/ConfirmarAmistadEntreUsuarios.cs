using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;

namespace GustosApp.Application.UseCases.AmistadUseCases
{
    public class ConfirmarAmistadEntreUsuarios
    {

        private readonly ISolicitudAmistadRepository _solicitudAmistadRepository;

        public ConfirmarAmistadEntreUsuarios(ISolicitudAmistadRepository solicitudAmistadRepository)
        {
            _solicitudAmistadRepository = solicitudAmistadRepository;
        }

        public virtual async Task<SolicitudAmistad> HandleAsync(Guid idActual, Guid idOtroUsuario, CancellationToken ct)
        {
            if (idActual == Guid.Empty || idOtroUsuario == Guid.Empty)
                throw new ArgumentException("User IDs cannot be empty.");

            var solicitudAmistad = await _solicitudAmistadRepository.
                GetAmistadEntreUsuariosAsync(idActual, idOtroUsuario, ct);

          

            return solicitudAmistad;
        }
    }
}
