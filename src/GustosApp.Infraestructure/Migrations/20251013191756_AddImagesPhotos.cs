using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GustosApp.Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class AddImagesPhotos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000001"),
                column: "ImagenUrl",
                value: "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/pizza.jpg?alt=media&token=1e4e7fea-31d3-4e04-ae50-1ebe29fd16f2");

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000002"),
                column: "ImagenUrl",
                value: "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/sushi.jpg?alt=media&token=9dfd9b64-8455-4206-a5ec-090c935e86e7");

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000003"),
                column: "ImagenUrl",
                value: "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/paella.jpg?alt=media&token=5cfd79d4-7e92-452e-a7c4-899b374d3ea8");

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000004"),
                column: "ImagenUrl",
                value: "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/milanesa-con-papas-fritas.jpg?alt=media&token=d2ca59bc-6360-4378-919a-886b0c0e93e0");

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000005"),
                column: "ImagenUrl",
                value: "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/tacos.jpg?alt=media&token=431ae163-15e9-41d0-8fa6-6f79e9862150");

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000006"),
                column: "ImagenUrl",
                value: "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/ensalada-cesar.jpg?alt=media&token=a6b5eaf0-be77-4716-8b11-18f3774f004f");

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000007"),
                column: "ImagenUrl",
                value: "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/ramen.jpg?alt=media&token=886fdc48-3d43-46fd-9911-48b1966da347");

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000008"),
                column: "ImagenUrl",
                value: "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/empanadas.png?alt=media&token=7438d05a-c0be-4da0-aea6-b6ab26f7f621");

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000009"),
                column: "ImagenUrl",
                value: "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/ceviche.jpg?alt=media&token=ad28a0df-4bc0-4aa8-ae02-610526ac1152");

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000010"),
                column: "ImagenUrl",
                value: "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/helado.jpg?alt=media&token=01be542d-9cc4-47f3-a27f-ae3a1b80d306");

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000011"),
                column: "ImagenUrl",
                value: "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/Hamburguesa.jpg?alt=media&token=a0fd669b-ade3-427c-b428-c743338885c8");

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000012"),
                column: "ImagenUrl",
                value: "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/papas-fritas.jpg?alt=media&token=5b18bf54-256e-4b36-adc3-e438fa3d374c");

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000013"),
                column: "ImagenUrl",
                value: "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/pollo-grill.jpg?alt=media&token=d60229b3-5e8a-4de6-9ed7-4a9622a2f3e1");

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000014"),
                column: "ImagenUrl",
                value: "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/kebab.jpg?alt=media&token=0acd13ee-654c-4748-bbe4-695a06053f75");

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000015"),
                column: "ImagenUrl",
                value: "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/ensalada-verde.jpg?alt=media&token=0bb027c8-de8d-4ac4-99db-ee80fc7d0f1c");

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000016"),
                column: "ImagenUrl",
                value: "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/asado.jpg?alt=media&token=254fbe63-39ba-4529-87bb-556381370c9a");

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000017"),
                column: "ImagenUrl",
                value: "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/sopa-verduras.jpg?alt=media&token=9858f540-0cb8-4759-a7d5-a6743144863e");

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000018"),
                column: "ImagenUrl",
                value: "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/panquques.jpg?alt=media&token=2b203013-cbcf-40f3-b266-d629466cd0b2");

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000019"),
                column: "ImagenUrl",
                value: "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/cafe-con-leche.jpg?alt=media&token=1f3cde3c-e3b9-4ed0-a690-d7e4d6875447");

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000020"),
                column: "ImagenUrl",
                value: "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/jugos-naturales.jpg?alt=media&token=b8afeb02-882e-4a3f-8445-14949e5871dd");

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000021"),
                column: "ImagenUrl",
                value: "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/Smoothie-frutas.jpg?alt=media&token=17ddc1f6-d60c-40d4-8368-f1f003ddd62b");

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000022"),
                column: "ImagenUrl",
                value: "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/chocolates.jpg?alt=media&token=5f33a2af-cee7-4768-8b47-dda973dd9c4e");

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000023"),
                column: "ImagenUrl",
                value: "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/tarta-manzana.jpg?alt=media&token=c34fa589-0c37-44cb-891e-2e8aaebdb215");

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000024"),
                column: "ImagenUrl",
                value: "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/Pescado-al-horn.png?alt=media&token=953296de-c3be-47a4-826e-31ad29cbae22");

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000025"),
                column: "ImagenUrl",
                value: "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/pasta-bolognesa.jpg?alt=media&token=7893a48f-f693-4a2f-a574-81336f91e62c");

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000026"),
                column: "ImagenUrl",
                value: "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/lomo_a_la_pimienta.png?alt=media&token=4d8495a4-181b-4d50-a9c8-e95010bfb100");

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000027"),
                column: "ImagenUrl",
                value: "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/ensalada_frutas.jpg?alt=media&token=a13662fa-2ac4-40cd-b1f0-347634a991e9");

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000028"),
                column: "ImagenUrl",
                value: "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/sandwich-de-huevo-con-jamon-y-queso.jpg?alt=media&token=8ac9b05d-c316-4a84-a099-9bca4f2d6a9a");

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000029"),
                column: "ImagenUrl",
                value: "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/huevos-revueltos-desayuno.jpeg?alt=media&token=0f21b637-b499-427c-bdb0-f0a841a76a9b");

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000030"),
                column: "ImagenUrl",
                value: "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/tipos-de-cerveza.jpg?alt=media&token=1cfa9e77-b663-421a-b649-d52a1ba751d2");

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000031"),
                column: "ImagenUrl",
                value: "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/vino_artesanal.jpg?alt=media&token=fd22ec00-7739-4776-b488-63e46c2937c5");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000001"),
                column: "ImagenUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000002"),
                column: "ImagenUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000003"),
                column: "ImagenUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000004"),
                column: "ImagenUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000005"),
                column: "ImagenUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000006"),
                column: "ImagenUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000007"),
                column: "ImagenUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000008"),
                column: "ImagenUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000009"),
                column: "ImagenUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000010"),
                column: "ImagenUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000011"),
                column: "ImagenUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000012"),
                column: "ImagenUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000013"),
                column: "ImagenUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000014"),
                column: "ImagenUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000015"),
                column: "ImagenUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000016"),
                column: "ImagenUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000017"),
                column: "ImagenUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000018"),
                column: "ImagenUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000019"),
                column: "ImagenUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000020"),
                column: "ImagenUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000021"),
                column: "ImagenUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000022"),
                column: "ImagenUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000023"),
                column: "ImagenUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000024"),
                column: "ImagenUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000025"),
                column: "ImagenUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000026"),
                column: "ImagenUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000027"),
                column: "ImagenUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000028"),
                column: "ImagenUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000029"),
                column: "ImagenUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000030"),
                column: "ImagenUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000031"),
                column: "ImagenUrl",
                value: null);
        }
    }
}
