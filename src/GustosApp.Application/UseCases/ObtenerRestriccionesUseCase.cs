using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GustosApp.Application.DTO;
using GustosApp.Domain.Interfaces;

namespace GustosApp.Application.UseCases
{
    public class ObtenerRestriccionesUseCase
    {

        private readonly IRestriccionRepository _restricciones;
        private readonly IUsuarioRepository _usuarios;


        public ObtenerRestriccionesUseCase(IRestriccionRepository restricciones, IUsuarioRepository usuarios)
        {
            _restricciones = restricciones;
            _usuarios = usuarios;
        }

        public async Task<List<RestriccionDto>> HandleAsync(string firebaseUid, CancellationToken ct)
        {

            var usuario = await _usuarios.GetByFirebaseUidAsync(firebaseUid, ct)
                          ?? throw new InvalidOperationException("Usuario no encontrado.");


            var todas = await _restricciones.GetAllAsync(ct);


            var seleccionadas = usuario.Restricciones?
                .Select(r => r.Id)
                .ToHashSet() ?? new HashSet<Guid>();


            var resultado = todas.Select(r => new RestriccionDto
            {
                Id = r.Id,
                Nombre = r.Nombre,
                Seleccionado = seleccionadas.Contains(r.Id)
            }).ToList();

            return resultado;
        }
    }

    }
