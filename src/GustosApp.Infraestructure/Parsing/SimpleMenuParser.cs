using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using GustosApp.Application.Interfaces;

namespace GustosApp.Infraestructure.Parsing
{
    /// <summary>
    /// Parser heurístico simple para texto de menú OCR -> JSON canónico.
    /// Estructura resultante:
    /// {
    ///   "nombreMenu": "Menú OCR",
    ///   "moneda": "ARS",
    ///   "categorias": [
    ///     { "nombre": "Pizzas", "items": [
    ///         { "nombre": "Margarita", "descripcion": "", "precios":[{"tamaño":"Único","monto":6500}], "tags":[] }
    ///     ]}
    ///   ]
    /// }
    /// </summary>
    public sealed class SimpleMenuParser : IMenuParser
    {
        private static readonly Regex PriceRegex = new(
            @"(?<!\S)\$?\s*(\d{1,3}(?:[.\s]\d{3})*|\d+)(?:[.,](\d{2}))?\s*\$?(?!\S)",
            RegexOptions.Compiled);

        private static readonly Regex CategoryRegex = new(
            @"^\s*([A-ZÁÉÍÓÚÜÑ][A-ZÁÉÍÓÚÜÑ\s\-]{2,}|[^:]{2,}:)\s*$",
            RegexOptions.Compiled);

        public Task<string> ParsearAsync(string texto, string monedaPorDefecto = "ARS", CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return Task.FromResult(JsonCanonicoVacio(monedaPorDefecto));

            var doc = new MenuDoc
            {
                nombreMenu = "Menú OCR",
                moneda = string.IsNullOrWhiteSpace(monedaPorDefecto) ? "ARS" : monedaPorDefecto
            };

            var categorias = new List<Categoria>();
            Categoria? actual = null;

            var lines = texto.Replace("\r", "").Split('\n')
                             .Select(l => l.Trim())
                             .Where(l => l.Length > 0)
                             .ToList();

            foreach (var line in lines)
            {
                ct.ThrowIfCancellationRequested();

                if (EsCategoria(line))
                {
                    actual = new Categoria { nombre = LimpiaHeader(line) };
                    categorias.Add(actual);
                    continue;
                }

                var (nombre, descripcion, precios) = ParseItem(line);
                if (string.IsNullOrWhiteSpace(nombre) && precios.Count == 0)
                    continue;

                if (actual == null)
                {
                    actual = new Categoria { nombre = "General" };
                    categorias.Add(actual);
                }

                actual.items.Add(new Item
                {
                    nombre = string.IsNullOrWhiteSpace(nombre) ? line : nombre,
                    descripcion = descripcion,
                    precios = precios,
                    tags = new List<string>()
                });
            }

            doc.categorias = categorias;

            var json = JsonSerializer.Serialize(doc, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                WriteIndented = false
            });

            return Task.FromResult(json);
        }

        private static bool EsCategoria(string line)
        {
            if (line.EndsWith(":")) return true;
            if (CategoryRegex.IsMatch(line)) return true;

            var up = line.ToUpperInvariant();
            var letters = line.Count(char.IsLetter);
            if (letters > 0)
            {
                var upperLetters = line.Count(c => char.IsLetter(c) && char.IsUpper(c));
                if ((double)upperLetters / letters >= 0.7) return true;
            }
            return false;
        }

        private static string LimpiaHeader(string line)
        {
            var s = line.Trim();
            if (s.EndsWith(":")) s = s[..^1].Trim();
            return s;
        }

        private static (string nombre, string descripcion, List<Precio> precios) ParseItem(string line)
        {
            var precios = new List<Precio>();

            var matches = PriceRegex.Matches(line);
            string clean = line;
            if (matches.Count > 0)
            {
                var m = matches[^1];
                var entero = m.Groups[1].Value;
                var dec = m.Groups[2].Success ? m.Groups[2].Value : null;

                var monto = NormalizaMonto(entero, dec);
                if (monto.HasValue)
                    precios.Add(new Precio { tamaño = "Único", monto = monto.Value });

                clean = line.Remove(m.Index, m.Length).Trim();
            }

            var parts = clean.Split(new[] { " - ", " | " }, StringSplitOptions.None);
            var nombre = parts[0].Trim();
            var descripcion = parts.Length > 1 ? string.Join(" ", parts.Skip(1)).Trim() : "";

            return (nombre, descripcion, precios);
        }

        private static decimal? NormalizaMonto(string entero, string? dec)
        {
            var sinMiles = entero.Replace(".", "").Replace(" ", "");
            if (!long.TryParse(sinMiles, out var parteEntera)) return null;

            int decimales = 0;
            if (!string.IsNullOrEmpty(dec) && int.TryParse(dec, out var d))
            {
                decimales = d;
                return parteEntera + (decimales / 100m);
            }
            return parteEntera;
        }

        private sealed class MenuDoc
        {
            public string nombreMenu { get; set; } = "Menú OCR";
            public string moneda { get; set; } = "ARS";
            public List<Categoria> categorias { get; set; } = new();
        }

        private sealed class Categoria
        {
            public string nombre { get; set; } = "";
            public List<Item> items { get; set; } = new();
        }

        private sealed class Item
        {
            public string nombre { get; set; } = "";
            public string? descripcion { get; set; }
            public List<Precio> precios { get; set; } = new();
            public List<string> tags { get; set; } = new();
        }

        private sealed class Precio
        {
            public string tamaño { get; set; } = "Único";
            public decimal monto { get; set; }
        }

        private static string JsonCanonicoVacio(string moneda)
        {
            var doc = new MenuDoc { nombreMenu = "Menú OCR", moneda = string.IsNullOrWhiteSpace(moneda) ? "ARS" : moneda };
            return JsonSerializer.Serialize(doc, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            });
        }
    }
}