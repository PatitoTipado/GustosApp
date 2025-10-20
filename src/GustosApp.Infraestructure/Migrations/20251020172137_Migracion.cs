using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GustosApp.Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class Migracion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "CondicionesMedicas",
                columns: new[] { "Id", "Nombre" },
                values: new object[,]
                {
                    { new Guid("44444444-0001-0001-0001-000000000013"), "Vegetariano" },
                    { new Guid("44444444-0001-0001-0001-000000000014"), "Vegano" }
                });

            migrationBuilder.InsertData(
                table: "Gustos",
                columns: new[] { "Id", "ImagenUrl", "Nombre" },
                values: new object[,]
                {
                    { new Guid("22222222-0001-0001-0001-000000000032"), null, "Ensalada Caprese" },
                    { new Guid("22222222-0001-0001-0001-000000000033"), null, "Tarta de Verduras" },
                    { new Guid("22222222-0001-0001-0001-000000000034"), null, "Omelette de vegetales" },
                    { new Guid("22222222-0001-0001-0001-000000000035"), null, "Pizza Margarita" },
                    { new Guid("22222222-0001-0001-0001-000000000036"), null, "Milanesa de berenjena" },
                    { new Guid("22222222-0001-0001-0001-000000000037"), null, "Ñoquis con salsa de tomate" },
                    { new Guid("22222222-0001-0001-0001-000000000038"), null, "Ravioles de ricota y espinaca" },
                    { new Guid("22222222-0001-0001-0001-000000000039"), null, "Fideos con pesto" },
                    { new Guid("22222222-0001-0001-0001-000000000040"), null, "Panqueques de avena con frutas" },
                    { new Guid("22222222-0001-0001-0001-000000000041"), null, "Empanadas de humita" },
                    { new Guid("22222222-0001-0001-0001-000000000042"), null, "Lasaña vegetariana" },
                    { new Guid("22222222-0001-0001-0001-000000000043"), null, "Arroz primavera" },
                    { new Guid("22222222-0001-0001-0001-000000000044"), null, "Polenta con salsa de tomate" },
                    { new Guid("22222222-0001-0001-0001-000000000045"), null, "Sopa de calabaza" },
                    { new Guid("22222222-0001-0001-0001-000000000046"), null, "Tortilla de papas" },
                    { new Guid("22222222-0001-0001-0001-000000000047"), null, "Quesadillas de vegetales" },
                    { new Guid("22222222-0001-0001-0001-000000000048"), null, "Bruschettas con tomate y albahaca" },
                    { new Guid("22222222-0001-0001-0001-000000000049"), null, "Pastel de papas vegetariano" },
                    { new Guid("22222222-0001-0001-0001-000000000050"), null, "Pizza cuatro quesos" },
                    { new Guid("22222222-0001-0001-0001-000000000051"), null, "Ensalada de quinoa con vegetales" },
                    { new Guid("22222222-0001-0001-0001-000000000052"), null, "Curry de vegetales con arroz" },
                    { new Guid("22222222-0001-0001-0001-000000000053"), null, "Hamburguesa de lentejas" },
                    { new Guid("22222222-0001-0001-0001-000000000054"), null, "Sopa crema de zapallo con leche vegetal" },
                    { new Guid("22222222-0001-0001-0001-000000000055"), null, "Arroz frito con tofu" },
                    { new Guid("22222222-0001-0001-0001-000000000056"), null, "Guiso de lentejas vegano" },
                    { new Guid("22222222-0001-0001-0001-000000000057"), null, "Pan integral con palta y tomate" },
                    { new Guid("22222222-0001-0001-0001-000000000058"), null, "Panqueques de banana sin huevo" },
                    { new Guid("22222222-0001-0001-0001-000000000059"), null, "Wrap de falafel con hummus" },
                    { new Guid("22222222-0001-0001-0001-000000000060"), null, "Brownie vegano(con harina integral y aceite de coco" },
                    { new Guid("22222222-0001-0001-0001-000000000061"), null, "Tarta vegana de calabaza" },
                    { new Guid("22222222-0001-0001-0001-000000000062"), null, "Empanadas veganas" },
                    { new Guid("22222222-0001-0001-0001-000000000063"), null, "Tacos veganos" },
                    { new Guid("22222222-0001-0001-0001-000000000064"), null, "Fideos de arroz con verduras salteadas" }
                });

            migrationBuilder.InsertData(
                table: "CondicionMedicaTags",
                columns: new[] { "CondicionesMedicasId", "TagsId" },
                values: new object[,]
                {
                    { new Guid("44444444-0001-0001-0001-000000000013"), new Guid("11111111-1111-1111-1111-111111111116") },
                    { new Guid("44444444-0001-0001-0001-000000000013"), new Guid("11111111-1111-1111-1111-111111111117") },
                    { new Guid("44444444-0001-0001-0001-000000000013"), new Guid("11111111-1111-1111-1111-111111111123") },
                    { new Guid("44444444-0001-0001-0001-000000000014"), new Guid("11111111-1111-1111-1111-111111111112") },
                    { new Guid("44444444-0001-0001-0001-000000000014"), new Guid("11111111-1111-1111-1111-111111111116") },
                    { new Guid("44444444-0001-0001-0001-000000000014"), new Guid("11111111-1111-1111-1111-111111111117") },
                    { new Guid("44444444-0001-0001-0001-000000000014"), new Guid("11111111-1111-1111-1111-111111111123") },
                    { new Guid("44444444-0001-0001-0001-000000000014"), new Guid("11111111-1111-1111-1111-111111111127") }
                });

            migrationBuilder.InsertData(
                table: "GustoTags",
                columns: new[] { "GustosId", "TagsId" },
                values: new object[,]
                {
                    { new Guid("22222222-0001-0001-0001-000000000032"), new Guid("11111111-1111-1111-1111-111111111112") },
                    { new Guid("22222222-0001-0001-0001-000000000032"), new Guid("11111111-1111-1111-1111-111111111118") },
                    { new Guid("22222222-0001-0001-0001-000000000033"), new Guid("11111111-1111-1111-1111-111111111118") },
                    { new Guid("22222222-0001-0001-0001-000000000033"), new Guid("11111111-1111-1111-1111-111111111126") },
                    { new Guid("22222222-0001-0001-0001-000000000034"), new Guid("11111111-1111-1111-1111-111111111118") },
                    { new Guid("22222222-0001-0001-0001-000000000034"), new Guid("11111111-1111-1111-1111-111111111127") },
                    { new Guid("22222222-0001-0001-0001-000000000035"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("22222222-0001-0001-0001-000000000035"), new Guid("11111111-1111-1111-1111-111111111112") },
                    { new Guid("22222222-0001-0001-0001-000000000035"), new Guid("11111111-1111-1111-1111-111111111126") },
                    { new Guid("22222222-0001-0001-0001-000000000036"), new Guid("11111111-1111-1111-1111-111111111112") },
                    { new Guid("22222222-0001-0001-0001-000000000036"), new Guid("11111111-1111-1111-1111-111111111118") },
                    { new Guid("22222222-0001-0001-0001-000000000036"), new Guid("11111111-1111-1111-1111-111111111126") },
                    { new Guid("22222222-0001-0001-0001-000000000037"), new Guid("11111111-1111-1111-1111-111111111114") },
                    { new Guid("22222222-0001-0001-0001-000000000037"), new Guid("11111111-1111-1111-1111-111111111126") },
                    { new Guid("22222222-0001-0001-0001-000000000038"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("22222222-0001-0001-0001-000000000038"), new Guid("11111111-1111-1111-1111-111111111112") },
                    { new Guid("22222222-0001-0001-0001-000000000038"), new Guid("11111111-1111-1111-1111-111111111126") },
                    { new Guid("22222222-0001-0001-0001-000000000039"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("22222222-0001-0001-0001-000000000039"), new Guid("11111111-1111-1111-1111-111111111124") },
                    { new Guid("22222222-0001-0001-0001-000000000039"), new Guid("11111111-1111-1111-1111-111111111126") },
                    { new Guid("22222222-0001-0001-0001-000000000040"), new Guid("11111111-1111-1111-1111-111111111119") },
                    { new Guid("22222222-0001-0001-0001-000000000040"), new Guid("11111111-1111-1111-1111-111111111126") },
                    { new Guid("22222222-0001-0001-0001-000000000040"), new Guid("11111111-1111-1111-1111-111111111127") },
                    { new Guid("22222222-0001-0001-0001-000000000041"), new Guid("11111111-1111-1111-1111-111111111112") },
                    { new Guid("22222222-0001-0001-0001-000000000041"), new Guid("11111111-1111-1111-1111-111111111118") },
                    { new Guid("22222222-0001-0001-0001-000000000041"), new Guid("11111111-1111-1111-1111-111111111126") },
                    { new Guid("22222222-0001-0001-0001-000000000042"), new Guid("11111111-1111-1111-1111-111111111112") },
                    { new Guid("22222222-0001-0001-0001-000000000042"), new Guid("11111111-1111-1111-1111-111111111118") },
                    { new Guid("22222222-0001-0001-0001-000000000042"), new Guid("11111111-1111-1111-1111-111111111126") },
                    { new Guid("22222222-0001-0001-0001-000000000043"), new Guid("11111111-1111-1111-1111-111111111118") },
                    { new Guid("22222222-0001-0001-0001-000000000044"), new Guid("11111111-1111-1111-1111-111111111114") },
                    { new Guid("22222222-0001-0001-0001-000000000044"), new Guid("11111111-1111-1111-1111-111111111118") },
                    { new Guid("22222222-0001-0001-0001-000000000045"), new Guid("11111111-1111-1111-1111-111111111118") },
                    { new Guid("22222222-0001-0001-0001-000000000046"), new Guid("11111111-1111-1111-1111-111111111118") },
                    { new Guid("22222222-0001-0001-0001-000000000046"), new Guid("11111111-1111-1111-1111-111111111120") },
                    { new Guid("22222222-0001-0001-0001-000000000046"), new Guid("11111111-1111-1111-1111-111111111127") },
                    { new Guid("22222222-0001-0001-0001-000000000047"), new Guid("11111111-1111-1111-1111-111111111112") },
                    { new Guid("22222222-0001-0001-0001-000000000047"), new Guid("11111111-1111-1111-1111-111111111118") },
                    { new Guid("22222222-0001-0001-0001-000000000047"), new Guid("11111111-1111-1111-1111-111111111126") },
                    { new Guid("22222222-0001-0001-0001-000000000048"), new Guid("11111111-1111-1111-1111-111111111118") },
                    { new Guid("22222222-0001-0001-0001-000000000048"), new Guid("11111111-1111-1111-1111-111111111126") },
                    { new Guid("22222222-0001-0001-0001-000000000049"), new Guid("11111111-1111-1111-1111-111111111112") },
                    { new Guid("22222222-0001-0001-0001-000000000049"), new Guid("11111111-1111-1111-1111-111111111118") },
                    { new Guid("22222222-0001-0001-0001-000000000050"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("22222222-0001-0001-0001-000000000050"), new Guid("11111111-1111-1111-1111-111111111112") },
                    { new Guid("22222222-0001-0001-0001-000000000050"), new Guid("11111111-1111-1111-1111-111111111124") },
                    { new Guid("22222222-0001-0001-0001-000000000050"), new Guid("11111111-1111-1111-1111-111111111126") },
                    { new Guid("22222222-0001-0001-0001-000000000051"), new Guid("11111111-1111-1111-1111-111111111118") },
                    { new Guid("22222222-0001-0001-0001-000000000052"), new Guid("11111111-1111-1111-1111-111111111118") },
                    { new Guid("22222222-0001-0001-0001-000000000052"), new Guid("11111111-1111-1111-1111-111111111121") },
                    { new Guid("22222222-0001-0001-0001-000000000053"), new Guid("11111111-1111-1111-1111-111111111118") },
                    { new Guid("22222222-0001-0001-0001-000000000053"), new Guid("11111111-1111-1111-1111-111111111126") },
                    { new Guid("22222222-0001-0001-0001-000000000054"), new Guid("11111111-1111-1111-1111-111111111118") },
                    { new Guid("22222222-0001-0001-0001-000000000055"), new Guid("11111111-1111-1111-1111-111111111118") },
                    { new Guid("22222222-0001-0001-0001-000000000055"), new Guid("11111111-1111-1111-1111-111111111120") },
                    { new Guid("22222222-0001-0001-0001-000000000055"), new Guid("11111111-1111-1111-1111-111111111129") },
                    { new Guid("22222222-0001-0001-0001-000000000056"), new Guid("11111111-1111-1111-1111-111111111118") },
                    { new Guid("22222222-0001-0001-0001-000000000057"), new Guid("11111111-1111-1111-1111-111111111118") },
                    { new Guid("22222222-0001-0001-0001-000000000057"), new Guid("11111111-1111-1111-1111-111111111126") },
                    { new Guid("22222222-0001-0001-0001-000000000058"), new Guid("11111111-1111-1111-1111-111111111119") },
                    { new Guid("22222222-0001-0001-0001-000000000058"), new Guid("11111111-1111-1111-1111-111111111126") },
                    { new Guid("22222222-0001-0001-0001-000000000059"), new Guid("11111111-1111-1111-1111-111111111118") },
                    { new Guid("22222222-0001-0001-0001-000000000059"), new Guid("11111111-1111-1111-1111-111111111126") },
                    { new Guid("22222222-0001-0001-0001-000000000059"), new Guid("11111111-1111-1111-1111-111111111129") },
                    { new Guid("22222222-0001-0001-0001-000000000060"), new Guid("11111111-1111-1111-1111-111111111113") },
                    { new Guid("22222222-0001-0001-0001-000000000060"), new Guid("11111111-1111-1111-1111-111111111124") },
                    { new Guid("22222222-0001-0001-0001-000000000060"), new Guid("11111111-1111-1111-1111-111111111126") },
                    { new Guid("22222222-0001-0001-0001-000000000061"), new Guid("11111111-1111-1111-1111-111111111118") },
                    { new Guid("22222222-0001-0001-0001-000000000061"), new Guid("11111111-1111-1111-1111-111111111126") },
                    { new Guid("22222222-0001-0001-0001-000000000062"), new Guid("11111111-1111-1111-1111-111111111118") },
                    { new Guid("22222222-0001-0001-0001-000000000062"), new Guid("11111111-1111-1111-1111-111111111126") },
                    { new Guid("22222222-0001-0001-0001-000000000063"), new Guid("11111111-1111-1111-1111-111111111118") },
                    { new Guid("22222222-0001-0001-0001-000000000063"), new Guid("11111111-1111-1111-1111-111111111126") },
                    { new Guid("22222222-0001-0001-0001-000000000064"), new Guid("11111111-1111-1111-1111-111111111118") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "CondicionMedicaTags",
                keyColumns: new[] { "CondicionesMedicasId", "TagsId" },
                keyValues: new object[] { new Guid("44444444-0001-0001-0001-000000000013"), new Guid("11111111-1111-1111-1111-111111111116") });

            migrationBuilder.DeleteData(
                table: "CondicionMedicaTags",
                keyColumns: new[] { "CondicionesMedicasId", "TagsId" },
                keyValues: new object[] { new Guid("44444444-0001-0001-0001-000000000013"), new Guid("11111111-1111-1111-1111-111111111117") });

            migrationBuilder.DeleteData(
                table: "CondicionMedicaTags",
                keyColumns: new[] { "CondicionesMedicasId", "TagsId" },
                keyValues: new object[] { new Guid("44444444-0001-0001-0001-000000000013"), new Guid("11111111-1111-1111-1111-111111111123") });

            migrationBuilder.DeleteData(
                table: "CondicionMedicaTags",
                keyColumns: new[] { "CondicionesMedicasId", "TagsId" },
                keyValues: new object[] { new Guid("44444444-0001-0001-0001-000000000014"), new Guid("11111111-1111-1111-1111-111111111112") });

            migrationBuilder.DeleteData(
                table: "CondicionMedicaTags",
                keyColumns: new[] { "CondicionesMedicasId", "TagsId" },
                keyValues: new object[] { new Guid("44444444-0001-0001-0001-000000000014"), new Guid("11111111-1111-1111-1111-111111111116") });

            migrationBuilder.DeleteData(
                table: "CondicionMedicaTags",
                keyColumns: new[] { "CondicionesMedicasId", "TagsId" },
                keyValues: new object[] { new Guid("44444444-0001-0001-0001-000000000014"), new Guid("11111111-1111-1111-1111-111111111117") });

            migrationBuilder.DeleteData(
                table: "CondicionMedicaTags",
                keyColumns: new[] { "CondicionesMedicasId", "TagsId" },
                keyValues: new object[] { new Guid("44444444-0001-0001-0001-000000000014"), new Guid("11111111-1111-1111-1111-111111111123") });

            migrationBuilder.DeleteData(
                table: "CondicionMedicaTags",
                keyColumns: new[] { "CondicionesMedicasId", "TagsId" },
                keyValues: new object[] { new Guid("44444444-0001-0001-0001-000000000014"), new Guid("11111111-1111-1111-1111-111111111127") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000032"), new Guid("11111111-1111-1111-1111-111111111112") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000032"), new Guid("11111111-1111-1111-1111-111111111118") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000033"), new Guid("11111111-1111-1111-1111-111111111118") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000033"), new Guid("11111111-1111-1111-1111-111111111126") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000034"), new Guid("11111111-1111-1111-1111-111111111118") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000034"), new Guid("11111111-1111-1111-1111-111111111127") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000035"), new Guid("11111111-1111-1111-1111-111111111111") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000035"), new Guid("11111111-1111-1111-1111-111111111112") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000035"), new Guid("11111111-1111-1111-1111-111111111126") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000036"), new Guid("11111111-1111-1111-1111-111111111112") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000036"), new Guid("11111111-1111-1111-1111-111111111118") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000036"), new Guid("11111111-1111-1111-1111-111111111126") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000037"), new Guid("11111111-1111-1111-1111-111111111114") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000037"), new Guid("11111111-1111-1111-1111-111111111126") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000038"), new Guid("11111111-1111-1111-1111-111111111111") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000038"), new Guid("11111111-1111-1111-1111-111111111112") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000038"), new Guid("11111111-1111-1111-1111-111111111126") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000039"), new Guid("11111111-1111-1111-1111-111111111111") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000039"), new Guid("11111111-1111-1111-1111-111111111124") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000039"), new Guid("11111111-1111-1111-1111-111111111126") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000040"), new Guid("11111111-1111-1111-1111-111111111119") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000040"), new Guid("11111111-1111-1111-1111-111111111126") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000040"), new Guid("11111111-1111-1111-1111-111111111127") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000041"), new Guid("11111111-1111-1111-1111-111111111112") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000041"), new Guid("11111111-1111-1111-1111-111111111118") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000041"), new Guid("11111111-1111-1111-1111-111111111126") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000042"), new Guid("11111111-1111-1111-1111-111111111112") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000042"), new Guid("11111111-1111-1111-1111-111111111118") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000042"), new Guid("11111111-1111-1111-1111-111111111126") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000043"), new Guid("11111111-1111-1111-1111-111111111118") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000044"), new Guid("11111111-1111-1111-1111-111111111114") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000044"), new Guid("11111111-1111-1111-1111-111111111118") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000045"), new Guid("11111111-1111-1111-1111-111111111118") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000046"), new Guid("11111111-1111-1111-1111-111111111118") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000046"), new Guid("11111111-1111-1111-1111-111111111120") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000046"), new Guid("11111111-1111-1111-1111-111111111127") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000047"), new Guid("11111111-1111-1111-1111-111111111112") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000047"), new Guid("11111111-1111-1111-1111-111111111118") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000047"), new Guid("11111111-1111-1111-1111-111111111126") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000048"), new Guid("11111111-1111-1111-1111-111111111118") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000048"), new Guid("11111111-1111-1111-1111-111111111126") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000049"), new Guid("11111111-1111-1111-1111-111111111112") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000049"), new Guid("11111111-1111-1111-1111-111111111118") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000050"), new Guid("11111111-1111-1111-1111-111111111111") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000050"), new Guid("11111111-1111-1111-1111-111111111112") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000050"), new Guid("11111111-1111-1111-1111-111111111124") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000050"), new Guid("11111111-1111-1111-1111-111111111126") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000051"), new Guid("11111111-1111-1111-1111-111111111118") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000052"), new Guid("11111111-1111-1111-1111-111111111118") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000052"), new Guid("11111111-1111-1111-1111-111111111121") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000053"), new Guid("11111111-1111-1111-1111-111111111118") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000053"), new Guid("11111111-1111-1111-1111-111111111126") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000054"), new Guid("11111111-1111-1111-1111-111111111118") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000055"), new Guid("11111111-1111-1111-1111-111111111118") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000055"), new Guid("11111111-1111-1111-1111-111111111120") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000055"), new Guid("11111111-1111-1111-1111-111111111129") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000056"), new Guid("11111111-1111-1111-1111-111111111118") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000057"), new Guid("11111111-1111-1111-1111-111111111118") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000057"), new Guid("11111111-1111-1111-1111-111111111126") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000058"), new Guid("11111111-1111-1111-1111-111111111119") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000058"), new Guid("11111111-1111-1111-1111-111111111126") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000059"), new Guid("11111111-1111-1111-1111-111111111118") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000059"), new Guid("11111111-1111-1111-1111-111111111126") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000059"), new Guid("11111111-1111-1111-1111-111111111129") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000060"), new Guid("11111111-1111-1111-1111-111111111113") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000060"), new Guid("11111111-1111-1111-1111-111111111124") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000060"), new Guid("11111111-1111-1111-1111-111111111126") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000061"), new Guid("11111111-1111-1111-1111-111111111118") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000061"), new Guid("11111111-1111-1111-1111-111111111126") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000062"), new Guid("11111111-1111-1111-1111-111111111118") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000062"), new Guid("11111111-1111-1111-1111-111111111126") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000063"), new Guid("11111111-1111-1111-1111-111111111118") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000063"), new Guid("11111111-1111-1111-1111-111111111126") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000064"), new Guid("11111111-1111-1111-1111-111111111118") });

            migrationBuilder.DeleteData(
                table: "CondicionesMedicas",
                keyColumn: "Id",
                keyValue: new Guid("44444444-0001-0001-0001-000000000013"));

            migrationBuilder.DeleteData(
                table: "CondicionesMedicas",
                keyColumn: "Id",
                keyValue: new Guid("44444444-0001-0001-0001-000000000014"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000032"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000033"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000034"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000035"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000036"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000037"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000038"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000039"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000040"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000041"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000042"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000043"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000044"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000045"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000046"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000047"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000048"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000049"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000050"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000051"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000052"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000053"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000054"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000055"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000056"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000057"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000058"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000059"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000060"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000061"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000062"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000063"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000064"));
        }
    }
}
