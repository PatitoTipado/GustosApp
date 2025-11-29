using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GustosApp.Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class chatmesaje : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessages_Usuarios_UsuarioId",
                table: "ChatMessages");

            migrationBuilder.AlterColumn<Guid>(
                name: "UsuarioId",
                table: "ChatMessages",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000001"),
                column: "ImagenUrl",
                value: "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%20optimizado%2Fpizza%20(1)%20(1).jpg?alt=media&token=acae5ec9-56ff-4b1e-9f52-c6e3aa1c0465");

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000002"),
                column: "ImagenUrl",
                value: "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%20optimizado%2Fsushi%20(1).jpg?alt=media&token=699bb1f7-9ca8-467c-a07c-c25b85e155dd");

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000003"),
                column: "ImagenUrl",
                value: "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%20optimizado%2Fpaella%20(1)%20(1).jpg?alt=media&token=8d28a0aa-4e53-4552-ad79-4007f27ea6ef");

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000006"),
                column: "ImagenUrl",
                value: "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%20optimizado%2Fensalada-cesar%20(1).jpg?alt=media&token=c478c0b8-7b06-40d3-b3b6-b4d8a1c7af45");

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000007"),
                column: "ImagenUrl",
                value: "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%20optimizado%2Framen%20(1).jpg?alt=media&token=f50e95b0-8f37-499a-803e-2f9ce530d68a");

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000008"),
                column: "ImagenUrl",
                value: "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%20optimizado%2Fempanadas.jpg?alt=media&token=07a8e917-a280-4345-9717-fe0b707dc8e7");

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000010"),
                column: "ImagenUrl",
                value: "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%20optimizado%2Fhelado%20(1).jpg?alt=media&token=eef033a6-344a-41af-904a-7ff1116671c3");

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000013"),
                column: "ImagenUrl",
                value: "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%20optimizado%2Fpollo-grill%20(1)%20(1).jpg?alt=media&token=22febf47-6138-4f88-a0a5-d131ef67f44c");

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000016"),
                column: "ImagenUrl",
                value: "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%20optimizado%2Fasado%20(1).jpg?alt=media&token=86e67902-4e82-4d36-87df-b6f4973e9b61");

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000017"),
                column: "ImagenUrl",
                value: "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%20optimizado%2Fsopa-verduras%20(1).jpg?alt=media&token=a9f74b05-2726-4a12-89fc-51e9ab99f895");

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000022"),
                column: "ImagenUrl",
                value: "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%20optimizado%2Fchocolates%20(1).jpg?alt=media&token=52113b04-41a3-4e40-85bf-c6887227528c");

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000023"),
                column: "ImagenUrl",
                value: "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%20optimizado%2Ftarta-manzana%20(1).jpg?alt=media&token=a16c689c-6cbb-4e77-a9f3-5bf448dc6450");

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000024"),
                column: "ImagenUrl",
                value: "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%20optimizado%2FPescado-al-horn.jpg?alt=media&token=dd7a4dee-6229-4766-83e1-2647ab36b64c");

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000026"),
                column: "ImagenUrl",
                value: "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%20optimizado%2Flomo_a_la_pimienta.jpg?alt=media&token=1b1b8e30-083a-4ad2-854b-7903a4f750c9");

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000029"),
                column: "ImagenUrl",
                value: "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%20optimizado%2Fhuevos-revueltos-desayuno.jpg?alt=media&token=c08b7124-4f73-4c68-b85e-7f8949b65688");

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000032"),
                column: "ImagenUrl",
                value: "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2Fensalada-caprese.jpg?alt=media&token=6bc93fe3-acc5-46b4-be63-c1de7b621854");

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000033"),
                column: "ImagenUrl",
                value: "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2Ftarta-verduras.jpg?alt=media&token=65092fe1-acfa-4c5a-84c4-ebcf00ad6068");

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000034"),
                column: "ImagenUrl",
                value: "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2Fomelette-de-verduras.jpg?alt=media&token=25b514e3-5f87-43a0-99b0-83b0b9af243f");

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000035"),
                column: "ImagenUrl",
                value: "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2Fpizza-margarita.jpg?alt=media&token=0ae93afc-274e-4a4b-879a-8d288cac8d3a");

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000036"),
                column: "ImagenUrl",
                value: "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2FMILANESAS-DE-BERENJENA-CON-PURE-DE-ARVEJAS-05%20(1).jpg?alt=media&token=f21fab62-9138-450d-be46-faf0acf43ef1");

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000037"),
                column: "ImagenUrl",
                value: "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2Fnoquis-caseros-con-salsa-de-tomate-1200x900.jpg?alt=media&token=35667f32-5775-41b6-9d29-aed9acf2a64b");

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000038"),
                column: "ImagenUrl",
                value: "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2FRavioles-ricota-espinaca%20(1).jpg?alt=media&token=89d4e8ba-7db8-40d5-8d1a-09cf16216af8");

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000039"),
                column: "ImagenUrl",
                value: "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2Ffideos_con_pesto.jpg?alt=media&token=47cbf2d7-53f2-4710-88a8-ef8b43fb3dfc");

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000040"),
                column: "ImagenUrl",
                value: "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2FPanquequavenarutas.jpg?alt=media&token=0d4dcf14-a9e3-48c1-a6ff-06118c259555");

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000041"),
                column: "ImagenUrl",
                value: "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2Fempanda_humita.jpg?alt=media&token=862cd874-b8b2-4793-93f5-284268db5f6d");

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000042"),
                column: "ImagenUrl",
                value: "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2FLasa%C3%B1a-vegetariana.jpg?alt=media&token=2ad7e9ef-a341-43d3-b135-9b22b2f9d0ba");

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000043"),
                column: "ImagenUrl",
                value: "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2FArrozprimavera.jpg?alt=media&token=3872ad34-aec3-4610-af74-88a5ed01894f");

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000044"),
                column: "ImagenUrl",
                value: "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2FPolenta-salsa-tomate.jpg?alt=media&token=9869796e-f39d-4536-afe4-b79131bdcaa9");

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000045"),
                column: "ImagenUrl",
                value: "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2Fsopa-de-calabaza_web.jpg?alt=media&token=06b7d888-fea8-4f88-8447-c8c4494018e9");

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000046"),
                column: "ImagenUrl",
                value: "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2Ftortilla-de-papas-con-EPMZH233TBBEBETIIAYLJQ57JU.jpg?alt=media&token=42079a0c-02c9-4de4-8e9e-eff9d9c59d9f");

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000047"),
                column: "ImagenUrl",
                value: "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2FQuesadillas-vegetales.jpg?alt=media&token=2f5fcc35-826f-4134-a50f-a7a81f2e20d2");

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000048"),
                column: "ImagenUrl",
                value: "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2FBruschetta-tomate-albahaca.jpg?alt=media&token=3dc0476c-f168-40f1-afa1-d557e2ab4f90");

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000049"),
                column: "ImagenUrl",
                value: "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2FPastel-papas-vegetariano.jpg?alt=media&token=81f3f01a-f6cc-4489-91ca-d191d30a5567");

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000050"),
                column: "ImagenUrl",
                value: "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2FPizza-cuatro-quesos%20(1).jpg?alt=media&token=d92594ca-b9f7-42ac-b2b1-ceb16f269740");

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000051"),
                column: "ImagenUrl",
                value: "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2FEnsalada-de-quinoa-con-verduras.jpg?alt=media&token=aec93b8a-c0ae-431e-b006-8db27840fdc5");

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000052"),
                column: "ImagenUrl",
                value: "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2Farroz-al-curry-con-2HA57GEWZJFM5CD27MHLFJ6ZMU.jpg?alt=media&token=65998321-d8c5-4dea-ba17-dcb311f2cf77");

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000053"),
                column: "ImagenUrl",
                value: "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2Fhamburguesa-lentejas-7.jpg?alt=media&token=a418381f-009f-47dc-ac6d-1713c94df7bf");

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000054"),
                column: "ImagenUrl",
                value: "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2FSopa-crema-zapallo-leche-vegetal.jpg?alt=media&token=fa58ead1-8dbd-4121-a5e0-5f9428713a57");

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000055"),
                column: "ImagenUrl",
                value: "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2Farroz_frito_con_tofu_62074_orig.jpg?alt=media&token=1e451e0b-8eef-4e0d-a761-c9364e6ec4c9");

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000056"),
                column: "ImagenUrl",
                value: "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2FGuiso-lentejas-vegano.jpg?alt=media&token=3ef78d85-c7e4-4fb6-b1cc-8c00b183dea0");

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000057"),
                column: "ImagenUrl",
                value: "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2Fpan_con_palta_tomate.jpg?alt=media&token=2c63b3e3-4092-46f2-b852-4e382a86f452");

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000058"),
                column: "ImagenUrl",
                value: "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2FPanqueques-banana-sin-huevo.jpg?alt=media&token=ccea698a-3c15-41d0-82dd-5080f3a91210");

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000059"),
                column: "ImagenUrl",
                value: "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2FWrap-falafel-hummus.jpg?alt=media&token=26a4a908-3460-4eb2-b372-077acf14fe1b");

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000060"),
                column: "ImagenUrl",
                value: "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2FBrownievegano(con%20harina%20integra%20aceite%20de%20coco.jpg?alt=media&token=96a9ce83-43bf-4d92-9fbb-d6c9bdd0701c");

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000061"),
                column: "ImagenUrl",
                value: "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2Ftarta-de-calabaza-vegana-pumpkin-pie-saludable.jpg?alt=media&token=21f2a9f5-33d6-4968-9963-21615ac588a6");

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000062"),
                column: "ImagenUrl",
                value: "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2Fempanadillas2-1024x683.jpg?alt=media&token=20a2c707-ca29-4a1e-a194-9be0fa3424dd");

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000063"),
                column: "ImagenUrl",
                value: "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2Ftacos_veganos.jpg?alt=media&token=9437ffc6-8a1f-4c93-89f3-33f0e4d35ebf");

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000064"),
                column: "ImagenUrl",
                value: "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2FFideos-arrozrerdurassalteadas.jpg?alt=media&token=a17821fc-210e-4078-856a-d892c75b5a93");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessages_Usuarios_UsuarioId",
                table: "ChatMessages",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessages_Usuarios_UsuarioId",
                table: "ChatMessages");

            migrationBuilder.AlterColumn<Guid>(
                name: "UsuarioId",
                table: "ChatMessages",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

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
                keyValue: new Guid("22222222-0001-0001-0001-000000000010"),
                column: "ImagenUrl",
                value: "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/helado.jpg?alt=media&token=01be542d-9cc4-47f3-a27f-ae3a1b80d306");

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000013"),
                column: "ImagenUrl",
                value: "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/pollo-grill.jpg?alt=media&token=d60229b3-5e8a-4de6-9ed7-4a9622a2f3e1");

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
                keyValue: new Guid("22222222-0001-0001-0001-000000000026"),
                column: "ImagenUrl",
                value: "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/lomo_a_la_pimienta.png?alt=media&token=4d8495a4-181b-4d50-a9c8-e95010bfb100");

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000029"),
                column: "ImagenUrl",
                value: "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/huevos-revueltos-desayuno.jpeg?alt=media&token=0f21b637-b499-427c-bdb0-f0a841a76a9b");

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000032"),
                column: "ImagenUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000033"),
                column: "ImagenUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000034"),
                column: "ImagenUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000035"),
                column: "ImagenUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000036"),
                column: "ImagenUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000037"),
                column: "ImagenUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000038"),
                column: "ImagenUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000039"),
                column: "ImagenUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000040"),
                column: "ImagenUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000041"),
                column: "ImagenUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000042"),
                column: "ImagenUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000043"),
                column: "ImagenUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000044"),
                column: "ImagenUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000045"),
                column: "ImagenUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000046"),
                column: "ImagenUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000047"),
                column: "ImagenUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000048"),
                column: "ImagenUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000049"),
                column: "ImagenUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000050"),
                column: "ImagenUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000051"),
                column: "ImagenUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000052"),
                column: "ImagenUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000053"),
                column: "ImagenUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000054"),
                column: "ImagenUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000055"),
                column: "ImagenUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000056"),
                column: "ImagenUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000057"),
                column: "ImagenUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000058"),
                column: "ImagenUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000059"),
                column: "ImagenUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000060"),
                column: "ImagenUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000061"),
                column: "ImagenUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000062"),
                column: "ImagenUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000063"),
                column: "ImagenUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000064"),
                column: "ImagenUrl",
                value: null);

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessages_Usuarios_UsuarioId",
                table: "ChatMessages",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
