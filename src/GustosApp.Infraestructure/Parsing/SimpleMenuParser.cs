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
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using System.Text.RegularExpressions;

    public sealed class SimpleMenuParser : IMenuParser
    {
        private static readonly Regex PriceRegex = new(
            @"(?<!\S)\$?\s*(\d{1,3}(?:[.\s]\d{3})*|\d+)(?:[.,](\d{2}))?\s*\$?(?!\S)",
            RegexOptions.Compiled);

        // -----------------------------------------------------------
        //          1) UNIFICADOR DE LÍNEAS (Vision OCR Fix)
        // -----------------------------------------------------------
        private static List<string> UnificarLineas(List<string> lines)
        {
            var result = new List<string>();
            string buffer = "";

            foreach (var line in lines)
            {
                var s = line.Trim();
                if (string.IsNullOrWhiteSpace(s)) continue;

                // Si es categoría → cortar item previo
                if (EsCategoria(s))
                {
                    if (buffer.Length > 0)
                    {
                        result.Add(buffer.Trim());
                        buffer = "";
                    }

                    result.Add(s);
                    continue;
                }

                // Acumulamos líneas normales
                if (buffer.Length == 0)
                    buffer = s;
                else
                    buffer += " " + s;
            }

            if (buffer.Length > 0)
                result.Add(buffer.Trim());

            return result;
        }

        // -----------------------------------------------------------
        //          2) DETECCIÓN ROBUSTA DE CATEGORÍAS
        // -----------------------------------------------------------
        private static bool EsCategoria(string line)
        {
            var s = line.Trim();

            if (s.Length < 4)
                return false;

            var letters = s.Count(char.IsLetter);
            if (letters < 4)
                return false;

            // Termina con ":" → categoría
            if (s.EndsWith(":"))
                return true;

            // Es mayormente mayúsculas → categoría
            var upper = s.Count(c => char.IsLetter(c) && char.IsUpper(c));
            if (upper >= 4 && (double)upper / letters >= 0.65)
                return true;

            // Palabra única larga en mayúsculas
            if (!s.Contains(" ") && s.Length >= 6 && s.All(char.IsLetter) && s.All(char.IsUpper))
                return true;

            return false;
        }

        // -----------------------------------------------------------
        //          3) EXTRAER NOMBRE + PRECIO SI ESTÁ EN LA MISMA LÍNEA
        // -----------------------------------------------------------
        private static (string nombre, decimal? precio) ExtraerItem(string line)
        {
            var match = PriceRegex.Match(line);

            if (match.Success)
            {
                var monto = NormalizaMonto(match.Groups[1].Value, match.Groups[2].Value);
                var limpio = line.Replace(match.Value, "").Trim();
                return (limpio, monto);
            }

            return (line.Trim(), null);
        }

        private static decimal? NormalizaMonto(string entero, string dec)
        {
            var sinMiles = entero.Replace(".", "").Replace(" ", "");
            if (!long.TryParse(sinMiles, out var baseVal))
                return null;

            if (!string.IsNullOrEmpty(dec) && int.TryParse(dec, out var d))
                return baseVal + (d / 100m);

            return baseVal;
        }

        // -----------------------------------------------------------
        //          4) PARSER PRINCIPAL (REFAC­TOR FINAL)
        // -----------------------------------------------------------
        public Task<string> ParsearAsync(string texto, string monedaPorDefecto = "ARS", CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return Task.FromResult(JsonCanonicoVacio(monedaPorDefecto));

            var doc = new MenuDoc
            {
                nombreMenu = "Menú OCR",
                moneda = monedaPorDefecto
            };

            var categorias = new List<Categoria>();
            Categoria? actual = null;

            var lines = texto.Replace("\r", "").Split('\n')
                .Select(x => x.Trim())
                .Where(x => x.Length > 0)
                .ToList();

            // Vision OCR fix: unir líneas partidas
            lines = UnificarLineas(lines);

            foreach (var line in lines)
            {
                ct.ThrowIfCancellationRequested();

                if (EsCategoria(line))
                {
                    actual = new Categoria { nombre = line };
                    categorias.Add(actual);
                    continue;
                }

                if (actual == null)
                {
                    actual = new Categoria { nombre = "General" };
                    categorias.Add(actual);
                }

                var (nombre, precio) = ExtraerItem(line);

                var item = new Item
                {
                    nombre = nombre,
                    descripcion = "",
                    precios = new List<Precio>(),
                    tags = new List<string>()
                };

                if (precio.HasValue)
                {
                    item.precios.Add(new Precio
                    {
                        tamaño = "Único",
                        monto = precio.Value
                    });
                }

                actual.items.Add(item);
            }

            // Eliminar categorías sin contenido
            categorias = categorias
                .Where(x => x.items.Count > 0)
                .ToList();

            // Eliminar "General" basura
            if (categorias.Count > 1)
                categorias = categorias.Where(c => c.nombre != "General").ToList();

            doc.categorias = categorias;

            var json = JsonSerializer.Serialize(doc, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            });

            return Task.FromResult(json);
        }

        // -----------------------------------------------------------
        //          MODELOS DEL MENU
        // -----------------------------------------------------------
        private sealed class MenuDoc
        {
            public string nombreMenu { get; set; } = "";
            public string moneda { get; set; } = "";
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
            var doc = new MenuDoc { nombreMenu = "Menú OCR", moneda = moneda };
            return JsonSerializer.Serialize(doc,
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                });
        }
    }

}