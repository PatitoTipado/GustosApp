﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GustosApp.Application.DTO;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;

namespace GustosApp.Application.UseCases
{
    public class ObtenerGustosFiltradosUseCase
    {
        private readonly IUsuarioRepository _usuarios;
        private readonly IGustoRepository _gustos;
        public ObtenerGustosFiltradosUseCase(IUsuarioRepository usuarios, IGustoRepository gustos)
        {
            _usuarios = usuarios;
            _gustos = gustos;
        }

        public async Task<List<GustoDto>> HandleAsync(string firebaseUid, CancellationToken ct)
        {
            var usuario = await _usuarios.GetByFirebaseUidAsync(firebaseUid, ct)
                ?? throw new Exception("Usuario no encontrado");

            var todosLosGustos = await _gustos.GetAllAsync(ct);

            var tagsProhibidos = (usuario.Restricciones ?? Enumerable.Empty<Restriccion>())
                .SelectMany(r => r.TagsProhibidos ?? Enumerable.Empty<Tag>())
                .Concat((usuario.CondicionesMedicas ?? Enumerable.Empty<CondicionMedica>())
                    .SelectMany(c => c.TagsCriticos ?? Enumerable.Empty<Tag>()))
                .Select(t => t.NombreNormalizado)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToHashSet();

          
            var gustosFiltrados = todosLosGustos
                .Where(g => !(g.Tags ?? new List<Tag>())
                    .Any(t => tagsProhibidos.Contains(t.NombreNormalizado)))
                .ToList();

            return gustosFiltrados
                .Select(g => new GustoDto(g.Id, g.Nombre, g.ImagenUrl))
                .ToList();
        }
    }
}
