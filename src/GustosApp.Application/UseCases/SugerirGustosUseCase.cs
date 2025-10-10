using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Claims;
using GustosApp.Application.DTO;
using GustosApp.Application.Interfaces;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;

namespace GustosApp.Application.UseCases
{
    public class SugerirGustosUseCase {

        private readonly IUsuarioRepository _usuarios;
        private readonly IGustoRepository _gustos;
        private readonly IEmbeddingService? _embeddings;

        public SugerirGustosUseCase(IUsuarioRepository usuario, IGustoRepository gusto, IEmbeddingService? embeddings = null)
        {
            _usuarios = usuario;
            _gustos = gusto;
            _embeddings = embeddings;
        }

        public async Task<List<GustoResponse>> HandleAsync(ClaimsPrincipal user, int top = 5, CancellationToken ct = default)
        {
            var firebaseUid = user.FindFirst("user_id")?.Value
                        ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                        ?? user.FindFirst("sub")?.Value;

            if (string.IsNullOrWhiteSpace(firebaseUid))
                throw new UnauthorizedAccessException("No se encontró el UID de Firebase en el token.");

            var usuario = await _usuarios.GetByFirebaseUidWithGustosAsync(firebaseUid, ct);
            if (usuario == null)
                return new List<GustoResponse>();

            var todos = await _usuarios.GetAllWithGustosAsync(ct) ?? new List<Domain.Model.Usuario>();
            var userGustosIds = new HashSet<Guid>(usuario.Gustos.Select(g => g.Id));
            if (_embeddings != null)
            {
                try
                {
                    var allGustos = await _gustos.GetAllAsync(ct);

                    var textoUsuario = string.Join(" ", usuario.Gustos.Select(g => g.Nombre));
                    var userVec = await _embeddings.GetTextEmbeddingAsync(textoUsuario, ct);

                    var candidates = new List<(Guid id, double score)>();

                    foreach (var g in allGustos)
                    {
                        if (userGustosIds.Contains(g.Id)) continue;
                        var vecG = await _embeddings.GetTextEmbeddingAsync(g.Nombre, ct);
                        var sim = CosineSimilarity(userVec, vecG);
                        candidates.Add((g.Id, sim));
                    }

                    var gustoById = (await _gustos.GetAllAsync(ct)).ToDictionary(g => g.Id);

                    var topIds = candidates.OrderByDescending(c => c.score)
                                            .Take(top)
                                            .Select(c => c.id)
                                            .ToList();

                    var result = topIds.Select(id =>
                    {
                        if (gustoById.TryGetValue(id, out var g)) return new GustoResponse(g.Id, g.Nombre, g.ImagenUrl);
                        return new GustoResponse(id, "Gusto desconocido", null);
                    }).ToList();

                    if (result.Any()) return result;
                }
                catch
                {
                }
            }

            var scores = new Dictionary<Guid, int>();
            foreach (var other in todos)
            {
                if (other.Id == usuario.Id) continue;
                var comparte = other.Gustos.Any(g => userGustosIds.Contains(g.Id));
                if (!comparte) continue;
                foreach (var g in other.Gustos)
                {
                    if (userGustosIds.Contains(g.Id)) continue;
                    if (!scores.ContainsKey(g.Id)) scores[g.Id] = 0;
                    scores[g.Id] += 1;
                }
            }

            if (!scores.Any())
            {
                var freq = new Dictionary<Guid, int>();
                foreach (var u in todos)
                {
                    foreach (var g in u.Gustos)
                    {
                        if (userGustosIds.Contains(g.Id)) continue;
                        if (!freq.ContainsKey(g.Id)) freq[g.Id] = 0;
                        freq[g.Id] += 1;
                    }
                }
                foreach (var kv in freq) scores[kv.Key] = kv.Value;
            }

            var allGustos2 = await _gustos.GetAllAsync(ct);
            var gustoById2 = allGustos2.ToDictionary(g => g.Id);

            var ordered = scores
                .OrderByDescending(kv => kv.Value)
                .ThenBy(kv => gustoById2.ContainsKey(kv.Key) ? gustoById2[kv.Key].Nombre : string.Empty)
                .Take(top)
                .Select(kv =>
                {
                    if (gustoById2.TryGetValue(kv.Key, out var gusto))
                        return new GustoResponse(gusto.Id, gusto.Nombre, gusto.ImagenUrl);
                    return new GustoResponse(kv.Key, "Gusto desconocido", null);
                })
                .ToList();

            return ordered;
            }

        private static double CosineSimilarity(float[] a, float[] b)
        {
            if (a == null || b == null) return 0;
            var len = Math.Min(a.Length, b.Length);
            double dot = 0, na = 0, nb = 0;
            for (int i = 0; i < len; i++)
            {
                dot += a[i] * b[i];
                na += a[i] * a[i];
                nb += b[i] * b[i];
            }
            var denom = Math.Sqrt(na) * Math.Sqrt(nb);
            if (denom < 1e-8) return 0;
            return dot / denom;
        }
     }
}
