﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GustosApp.Application.DTO;
using GustosApp.Domain.Interfaces;

namespace GustosApp.Application.UseCases
{
    public class ObtenerGustosUseCase
    {
        private readonly IUsuarioRepository _usuarioRepo;

        public ObtenerGustosUseCase(IUsuarioRepository usuarioRepo)
        {
            _usuarioRepo = usuarioRepo;
        }

        public async Task<UsuarioPreferenciasDTO> Handle(string firebaseUid, CancellationToken ct = default)
        {
            var usuario = await _usuarioRepo.GetByFirebaseUidAsync(firebaseUid, ct);

            if (usuario == null)
                throw new UnauthorizedAccessException("Usuario no encontrado o no registrado.");

            var dto = new UsuarioPreferenciasDTO
            {
                Gustos = usuario.Gustos
                                .Select(g => g.Nombre)
                                .ToList(),

                Restricciones = usuario.Restricciones
                                       .Select(r => r.Nombre)
                                       .ToList(),

                CondicionesMedicas = usuario.CondicionesMedicas
                                            .Select(c => c.Nombre)
                                            .ToList()
            };

            return dto;

        }

    }
}
