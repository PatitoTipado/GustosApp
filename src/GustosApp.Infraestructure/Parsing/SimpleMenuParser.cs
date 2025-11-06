using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using GustosApp.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace GustosApp.Infraestructure.Parsing
{
    public sealed class SimpleMenuParser : IMenuParser
    {
        private static readonly Regex PriceRegex = new(
            // Captura 1..n precios por línea: $ 8.000, $8000, 8.000, 8000, 8.000,50, 8000.50, etc
            @"(?<!\S)\$?\s*(\d{1,3}(?:[.\s]\d{3})*|\d+)(?:[.,](\d{2}))?\s*\$?(?!\S)",
            RegexOptions.Compiled);

        private static readonly Regex LooksLikeOnlyPrice = new(
            @"^\s*\$?\s*(\d{1,3}(?:[.\s]\d{3})*|\d+)(?:[.,](\d{2}))?\s*\$?\s*$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex SizeNearRegex = new(
            @"\b(chico|chica|pequeño|pequeno|pequeña|mediano|mediana|grande|xl|xxl)\b",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

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

                // ⚠️ Evitar deconstrucción: más robusto en algunos toolchains
                var parsed = ParseItem(line);
                var nombre = parsed.nombre;
                var descripcion = parsed.descripcion;
                var preciosExtraidos = parsed.precios;

                if (string.IsNullOrWhiteSpace(nombre) && preciosExtraidos.Count == 0)
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
                    precios = preciosExtraidos,
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
            var preciosExtraidos = new List<Precio>();

            // 1) Extraer TODOS los precios y quitar TODAS las ocurrencias del texto
            var matches = PriceRegex.Matches(line);
            string clean = line;
            if (matches.Count > 0)
            {
                // Quitar de derecha a izquierda para no romper offsets
                foreach (Match m in matches.Cast<Match>().OrderByDescending(m => m.Index))
                {
                    var entero = m.Groups[1].Value;
                    var dec = m.Groups[2].Success ? m.Groups[2].Value : null;
                    var monto = NormalizaMonto(entero, dec);
                    if (monto.HasValue)
                    {
                        // 2) Inferir tamaño cerca del precio
                        var tam = InferirTamanioCercano(clean, m.Index, m.Length) ?? "Único";
                        preciosExtraidos.Add(new Precio { tamaño = tam, monto = monto.Value });
                    }
                    clean = clean.Remove(m.Index, m.Length);
                }
                clean = clean.Trim();
            }

            var parts = clean.Split(new[] { " - ", " | " }, StringSplitOptions.None);
            var nombre = parts[0].Trim();
            var descripcion = parts.Length > 1 ? string.Join(" ", parts.Skip(1)).Trim() : "";

            // 3) Si quedó vacío o solo era precio, placeholder y/o pasar texto a descripción
            if (string.IsNullOrWhiteSpace(nombre) || LooksLikeOnlyPrice.IsMatch(nombre))
            {
                if (!string.IsNullOrWhiteSpace(clean) && !LooksLikeOnlyPrice.IsMatch(clean))
                {
                    descripcion = string.IsNullOrWhiteSpace(descripcion) ? clean : $"{nombre} {descripcion}".Trim();
                }
                nombre = "Item";
            }

            return (nombre, descripcion, preciosExtraidos);
        }

        private static decimal? NormalizaMonto(string entero, string? dec)
        {
            var sinMiles = entero.Replace(".", "").Replace(" ", "");
            if (!long.TryParse(sinMiles, out var parteEntera)) return null;

            if (!string.IsNullOrEmpty(dec) && int.TryParse(dec, out var d))
                return parteEntera + (d / 100m);

            return parteEntera;
        }

        private static string? InferirTamanioCercano(string full, int priceIndex, int priceLen)
        {
            // Mirar ventana de ±20 chars
            int start = Math.Max(0, priceIndex - 20);
            int end = Math.Min(full.Length, priceIndex + priceLen + 20);
            var window = full.Substring(start, end - start);

            var m = SizeNearRegex.Match(window);
            if (!m.Success) return null;

            var raw = m.Value.ToLowerInvariant();
            return raw switch
            {
                "chico" or "chica" or "pequeño" or "pequeno" or "pequeña" => "Chico",
                "mediano" or "mediana" => "Mediano",
                "grande" => "Grande",
                "xl" => "XL",
                "xxl" => "XXL",
                _ => "Único"
            };
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