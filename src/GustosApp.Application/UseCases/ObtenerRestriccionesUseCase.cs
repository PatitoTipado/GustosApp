using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GustosApp.Application.DTO;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;

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

        public async Task<List<Restriccion>> HandleAsync(string firebaseUid, CancellationToken ct)
        {

            var usuario = await _usuarios.GetByFirebaseUidAsync(firebaseUid, ct)
                          ?? throw new InvalidOperationException("Usuario no encontrado.");


            var todas = await _restricciones.GetAllAsync(ct);

            return todas;
        }
    }

    }
