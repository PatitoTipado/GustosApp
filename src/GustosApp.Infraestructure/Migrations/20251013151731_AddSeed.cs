using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GustosApp.Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "CondicionMedicaTags",
                keyColumns: new[] { "CondicionesMedicasId", "TagsId" },
                keyValues: new object[] { new Guid("0a1b5b30-3940-46f6-8b06-d094f5da9a18"), new Guid("93696e0e-6226-47df-9af1-4dee58470fda") });

            migrationBuilder.DeleteData(
                table: "CondicionMedicaTags",
                keyColumns: new[] { "CondicionesMedicasId", "TagsId" },
                keyValues: new object[] { new Guid("0fba5975-a2d3-40e7-aad5-f68f4697812a"), new Guid("b1ccc4a5-d618-4032-8855-9fdcd4444d94") });

            migrationBuilder.DeleteData(
                table: "CondicionMedicaTags",
                keyColumns: new[] { "CondicionesMedicasId", "TagsId" },
                keyValues: new object[] { new Guid("1f192bb2-99ac-4725-9f90-0a83fecf0802"), new Guid("d25dee01-f656-4e95-901c-adcf5afb9854") });

            migrationBuilder.DeleteData(
                table: "CondicionMedicaTags",
                keyColumns: new[] { "CondicionesMedicasId", "TagsId" },
                keyValues: new object[] { new Guid("7cb101c0-3467-4671-9673-ca4f485cd5cf"), new Guid("b78455c1-23de-4429-b7e3-f0e902fc735c") });

            migrationBuilder.DeleteData(
                table: "CondicionMedicaTags",
                keyColumns: new[] { "CondicionesMedicasId", "TagsId" },
                keyValues: new object[] { new Guid("800c6326-7351-4132-8807-d6313cb586c4"), new Guid("e4de5bca-1945-42ea-8f4f-737665b37adf") });

            migrationBuilder.DeleteData(
                table: "CondicionMedicaTags",
                keyColumns: new[] { "CondicionesMedicasId", "TagsId" },
                keyValues: new object[] { new Guid("94c58ba4-fedb-42bc-a121-9b6d65a1af59"), new Guid("c45e4daa-4a52-439c-aff7-8075f993cef4") });

            migrationBuilder.DeleteData(
                table: "CondicionMedicaTags",
                keyColumns: new[] { "CondicionesMedicasId", "TagsId" },
                keyValues: new object[] { new Guid("a90feec1-4e14-4730-ab1a-c8b97e0515f4"), new Guid("fb386e3b-675c-4086-9fd4-d431c754da96") });

            migrationBuilder.DeleteData(
                table: "CondicionMedicaTags",
                keyColumns: new[] { "CondicionesMedicasId", "TagsId" },
                keyValues: new object[] { new Guid("b9ce1123-4673-420d-bbcc-e979e9b5a3bf"), new Guid("11637566-47f1-4d18-9bd7-573fdda77e13") });

            migrationBuilder.DeleteData(
                table: "CondicionMedicaTags",
                keyColumns: new[] { "CondicionesMedicasId", "TagsId" },
                keyValues: new object[] { new Guid("c1fbf544-c48b-4f2f-b4a8-476e6193240d"), new Guid("fcebd133-af80-4b53-9af3-1e1013a187df") });

            migrationBuilder.DeleteData(
                table: "CondicionMedicaTags",
                keyColumns: new[] { "CondicionesMedicasId", "TagsId" },
                keyValues: new object[] { new Guid("d9a326ab-f139-4548-bc33-e7a2dfce5fe8"), new Guid("4b610fb2-f412-4e94-b145-635e2d942b65") });

            migrationBuilder.DeleteData(
                table: "CondicionMedicaTags",
                keyColumns: new[] { "CondicionesMedicasId", "TagsId" },
                keyValues: new object[] { new Guid("df0fd0d9-ccbd-4ee0-85ef-cef99221394e"), new Guid("57fc04c5-81f4-44c8-8f55-5a833b1e2a3b") });

            migrationBuilder.DeleteData(
                table: "CondicionMedicaTags",
                keyColumns: new[] { "CondicionesMedicasId", "TagsId" },
                keyValues: new object[] { new Guid("ea28f81d-8dfe-4558-93b6-19537a0d47af"), new Guid("35f41fd6-267d-4f75-8268-e9f21424796b") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("03427ae8-a042-4868-9373-8ca80f97c9a4"), new Guid("11637566-47f1-4d18-9bd7-573fdda77e13") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("03427ae8-a042-4868-9373-8ca80f97c9a4"), new Guid("fb386e3b-675c-4086-9fd4-d431c754da96") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("0571ba30-490d-4ef4-ae9a-f935da97b3b0"), new Guid("b6027ef8-788c-47e1-85e1-85cebd901747") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("0832bd35-f5db-43c0-8b65-17f372cb2378"), new Guid("35f41fd6-267d-4f75-8268-e9f21424796b") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("0832bd35-f5db-43c0-8b65-17f372cb2378"), new Guid("b78455c1-23de-4429-b7e3-f0e902fc735c") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("0dee7103-d13b-47cf-886c-b3f0d4cb2e60"), new Guid("bbe9d4dd-2890-4725-860e-2affab9eef19") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("1c1c46d0-0d31-4107-b309-b4129f6c60d4"), new Guid("11637566-47f1-4d18-9bd7-573fdda77e13") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("1c1c46d0-0d31-4107-b309-b4129f6c60d4"), new Guid("b1ccc4a5-d618-4032-8855-9fdcd4444d94") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("1c1c46d0-0d31-4107-b309-b4129f6c60d4"), new Guid("fb386e3b-675c-4086-9fd4-d431c754da96") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("1d6363af-9a2b-4110-a58b-cfa444479ae3"), new Guid("31e31211-5d09-4782-9b5e-c523e7207bfb") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("1d6363af-9a2b-4110-a58b-cfa444479ae3"), new Guid("fb386e3b-675c-4086-9fd4-d431c754da96") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("1f31e2d9-0d7f-4a0e-818e-9f7951d67f79"), new Guid("57fc04c5-81f4-44c8-8f55-5a833b1e2a3b") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("1f31e2d9-0d7f-4a0e-818e-9f7951d67f79"), new Guid("b78455c1-23de-4429-b7e3-f0e902fc735c") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("254fe076-1791-43bb-940d-b72fc61f0aac"), new Guid("93696e0e-6226-47df-9af1-4dee58470fda") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("254fe076-1791-43bb-940d-b72fc61f0aac"), new Guid("b78455c1-23de-4429-b7e3-f0e902fc735c") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("254fe076-1791-43bb-940d-b72fc61f0aac"), new Guid("fb386e3b-675c-4086-9fd4-d431c754da96") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("3bd6bc23-b7ad-4f5e-bbbb-4dbcf28f4d97"), new Guid("3eb25592-620d-4a4c-842b-7b3067bf2275") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("3bd6bc23-b7ad-4f5e-bbbb-4dbcf28f4d97"), new Guid("fb386e3b-675c-4086-9fd4-d431c754da96") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("3f081178-d8e0-4399-8598-b02ff9a9c489"), new Guid("31e31211-5d09-4782-9b5e-c523e7207bfb") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("505fa8cf-d1bd-4799-804d-2735d691f0db"), new Guid("57fc04c5-81f4-44c8-8f55-5a833b1e2a3b") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("505fa8cf-d1bd-4799-804d-2735d691f0db"), new Guid("c8f41e3d-a411-4b6c-ad16-a93f9c6847a7") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("505fa8cf-d1bd-4799-804d-2735d691f0db"), new Guid("d25dee01-f656-4e95-901c-adcf5afb9854") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("56b6d595-a1b7-48fc-9ae0-4cf291e1eb21"), new Guid("11637566-47f1-4d18-9bd7-573fdda77e13") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("56b6d595-a1b7-48fc-9ae0-4cf291e1eb21"), new Guid("93696e0e-6226-47df-9af1-4dee58470fda") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("57494608-d0e8-48a5-ac5e-7c07afbb1831"), new Guid("c45e4daa-4a52-439c-aff7-8075f993cef4") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("57494608-d0e8-48a5-ac5e-7c07afbb1831"), new Guid("fb386e3b-675c-4086-9fd4-d431c754da96") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("58806a47-bd1b-4925-beec-658cff1a2b05"), new Guid("57fc04c5-81f4-44c8-8f55-5a833b1e2a3b") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("58806a47-bd1b-4925-beec-658cff1a2b05"), new Guid("bbe9d4dd-2890-4725-860e-2affab9eef19") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("58806a47-bd1b-4925-beec-658cff1a2b05"), new Guid("c8f41e3d-a411-4b6c-ad16-a93f9c6847a7") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("5fdc549f-8311-4f3c-a4c6-9e047e5698ca"), new Guid("4b610fb2-f412-4e94-b145-635e2d942b65") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("5fdc549f-8311-4f3c-a4c6-9e047e5698ca"), new Guid("fb386e3b-675c-4086-9fd4-d431c754da96") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("679c82e9-f149-441a-8f6d-e790fb171323"), new Guid("93696e0e-6226-47df-9af1-4dee58470fda") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("679c82e9-f149-441a-8f6d-e790fb171323"), new Guid("b78455c1-23de-4429-b7e3-f0e902fc735c") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("679c82e9-f149-441a-8f6d-e790fb171323"), new Guid("c1f31540-1462-422c-b4d1-15a8ac24b6d7") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("7102923e-ef0d-4a24-af3c-a18cc0b394eb"), new Guid("d25dee01-f656-4e95-901c-adcf5afb9854") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("758c61bf-cce5-402d-95b3-8c24926cdf7e"), new Guid("31e31211-5d09-4782-9b5e-c523e7207bfb") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("758c61bf-cce5-402d-95b3-8c24926cdf7e"), new Guid("b78455c1-23de-4429-b7e3-f0e902fc735c") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("758c61bf-cce5-402d-95b3-8c24926cdf7e"), new Guid("fb386e3b-675c-4086-9fd4-d431c754da96") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("7ec6923b-71db-42da-87f0-9916e491ef44"), new Guid("11637566-47f1-4d18-9bd7-573fdda77e13") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("7ec6923b-71db-42da-87f0-9916e491ef44"), new Guid("4b610fb2-f412-4e94-b145-635e2d942b65") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("7ec6923b-71db-42da-87f0-9916e491ef44"), new Guid("93696e0e-6226-47df-9af1-4dee58470fda") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("80426c4e-c519-4c8f-baf8-3ba207032d78"), new Guid("11637566-47f1-4d18-9bd7-573fdda77e13") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("80426c4e-c519-4c8f-baf8-3ba207032d78"), new Guid("4b610fb2-f412-4e94-b145-635e2d942b65") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("80426c4e-c519-4c8f-baf8-3ba207032d78"), new Guid("c8f41e3d-a411-4b6c-ad16-a93f9c6847a7") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("9bc3b600-23a5-46e5-8b97-271960b5d18d"), new Guid("57fc04c5-81f4-44c8-8f55-5a833b1e2a3b") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("9bc3b600-23a5-46e5-8b97-271960b5d18d"), new Guid("bbe9d4dd-2890-4725-860e-2affab9eef19") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("a7044fcb-2311-4cc8-a273-4fbdb112c626"), new Guid("11637566-47f1-4d18-9bd7-573fdda77e13") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("a7044fcb-2311-4cc8-a273-4fbdb112c626"), new Guid("e4de5bca-1945-42ea-8f4f-737665b37adf") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("aae43d0c-5915-40d0-b407-690ee84a25e6"), new Guid("11637566-47f1-4d18-9bd7-573fdda77e13") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("aae43d0c-5915-40d0-b407-690ee84a25e6"), new Guid("4b610fb2-f412-4e94-b145-635e2d942b65") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("aae43d0c-5915-40d0-b407-690ee84a25e6"), new Guid("93696e0e-6226-47df-9af1-4dee58470fda") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("b5b74a56-4cd8-4bab-ac12-a1f15b62f8be"), new Guid("93696e0e-6226-47df-9af1-4dee58470fda") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("b5b74a56-4cd8-4bab-ac12-a1f15b62f8be"), new Guid("e4de5bca-1945-42ea-8f4f-737665b37adf") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("b5b74a56-4cd8-4bab-ac12-a1f15b62f8be"), new Guid("fb386e3b-675c-4086-9fd4-d431c754da96") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("b96d06d9-7c3a-454c-a167-dab3f1e3ac13"), new Guid("bbe9d4dd-2890-4725-860e-2affab9eef19") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("c5e5e944-810f-44ab-a665-5c5ea434a544"), new Guid("11637566-47f1-4d18-9bd7-573fdda77e13") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("c5e5e944-810f-44ab-a665-5c5ea434a544"), new Guid("b1ccc4a5-d618-4032-8855-9fdcd4444d94") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("c5e5e944-810f-44ab-a665-5c5ea434a544"), new Guid("e4de5bca-1945-42ea-8f4f-737665b37adf") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("d24d4abd-7090-46e2-8b2f-2e343c829113"), new Guid("57fc04c5-81f4-44c8-8f55-5a833b1e2a3b") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("d24d4abd-7090-46e2-8b2f-2e343c829113"), new Guid("b1ccc4a5-d618-4032-8855-9fdcd4444d94") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("d74ff1ba-437f-458d-94ba-2b28482ad6fd"), new Guid("b6027ef8-788c-47e1-85e1-85cebd901747") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("e2c9ae68-0846-4211-8a19-7456dea0033f"), new Guid("31e31211-5d09-4782-9b5e-c523e7207bfb") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("e2c9ae68-0846-4211-8a19-7456dea0033f"), new Guid("b9e5fd0f-e917-456e-b7fd-4ee80db5998f") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("f485cf12-4fd7-4cee-a58b-78d5e3fdf5bf"), new Guid("31e31211-5d09-4782-9b5e-c523e7207bfb") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("f485cf12-4fd7-4cee-a58b-78d5e3fdf5bf"), new Guid("3eb25592-620d-4a4c-842b-7b3067bf2275") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("f6b2c395-c742-46a6-a586-821bbe22a0d8"), new Guid("31e31211-5d09-4782-9b5e-c523e7207bfb") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("f6b2c395-c742-46a6-a586-821bbe22a0d8"), new Guid("3eb25592-620d-4a4c-842b-7b3067bf2275") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("f6b2c395-c742-46a6-a586-821bbe22a0d8"), new Guid("c45e4daa-4a52-439c-aff7-8075f993cef4") });

            migrationBuilder.DeleteData(
                table: "RestriccionTags",
                keyColumns: new[] { "RestriccionesId", "TagsId" },
                keyValues: new object[] { new Guid("2ab8ac74-4cc3-476f-9e90-d7c63e012e08"), new Guid("11637566-47f1-4d18-9bd7-573fdda77e13") });

            migrationBuilder.DeleteData(
                table: "RestriccionTags",
                keyColumns: new[] { "RestriccionesId", "TagsId" },
                keyValues: new object[] { new Guid("5723b940-a18d-4f0f-97f6-480ea7d05b32"), new Guid("3d21eb50-be8b-4d1b-a88b-65bb2c564499") });

            migrationBuilder.DeleteData(
                table: "RestriccionTags",
                keyColumns: new[] { "RestriccionesId", "TagsId" },
                keyValues: new object[] { new Guid("63a5147a-84a7-4193-9bcb-40ad10025b6c"), new Guid("fb386e3b-675c-4086-9fd4-d431c754da96") });

            migrationBuilder.DeleteData(
                table: "RestriccionTags",
                keyColumns: new[] { "RestriccionesId", "TagsId" },
                keyValues: new object[] { new Guid("731b89a2-8898-4557-94dc-b7ee5f1bcbec"), new Guid("b6027ef8-788c-47e1-85e1-85cebd901747") });

            migrationBuilder.DeleteData(
                table: "RestriccionTags",
                keyColumns: new[] { "RestriccionesId", "TagsId" },
                keyValues: new object[] { new Guid("88ad6a6d-2fce-4713-8e22-b56041c57be1"), new Guid("93696e0e-6226-47df-9af1-4dee58470fda") });

            migrationBuilder.DeleteData(
                table: "RestriccionTags",
                keyColumns: new[] { "RestriccionesId", "TagsId" },
                keyValues: new object[] { new Guid("903d1554-2bbc-4e1c-8ba3-dc88f2d60b82"), new Guid("57fc04c5-81f4-44c8-8f55-5a833b1e2a3b") });

            migrationBuilder.DeleteData(
                table: "RestriccionTags",
                keyColumns: new[] { "RestriccionesId", "TagsId" },
                keyValues: new object[] { new Guid("9dc4cee3-db39-4af1-980d-dc6c2811b989"), new Guid("c45e4daa-4a52-439c-aff7-8075f993cef4") });

            migrationBuilder.DeleteData(
                table: "RestriccionTags",
                keyColumns: new[] { "RestriccionesId", "TagsId" },
                keyValues: new object[] { new Guid("abed4fde-7bb6-420c-935f-bc018a81f6a4"), new Guid("4b610fb2-f412-4e94-b145-635e2d942b65") });

            migrationBuilder.DeleteData(
                table: "RestriccionTags",
                keyColumns: new[] { "RestriccionesId", "TagsId" },
                keyValues: new object[] { new Guid("cb0209bc-2675-4515-ad0b-5889a660de17"), new Guid("fcebd133-af80-4b53-9af3-1e1013a187df") });

            migrationBuilder.DeleteData(
                table: "RestriccionTags",
                keyColumns: new[] { "RestriccionesId", "TagsId" },
                keyValues: new object[] { new Guid("d2507a73-8d64-45bc-bed5-3e1981613b70"), new Guid("b78455c1-23de-4429-b7e3-f0e902fc735c") });

            migrationBuilder.DeleteData(
                table: "RestriccionTags",
                keyColumns: new[] { "RestriccionesId", "TagsId" },
                keyValues: new object[] { new Guid("e1cd3b11-8559-4364-ab35-513c48a9196e"), new Guid("e4de5bca-1945-42ea-8f4f-737665b37adf") });

            migrationBuilder.DeleteData(
                table: "RestriccionTags",
                keyColumns: new[] { "RestriccionesId", "TagsId" },
                keyValues: new object[] { new Guid("f24a155c-408f-470f-8064-a8043de937ed"), new Guid("35f41fd6-267d-4f75-8268-e9f21424796b") });

            migrationBuilder.DeleteData(
                table: "CondicionesMedicas",
                keyColumn: "Id",
                keyValue: new Guid("0a1b5b30-3940-46f6-8b06-d094f5da9a18"));

            migrationBuilder.DeleteData(
                table: "CondicionesMedicas",
                keyColumn: "Id",
                keyValue: new Guid("0fba5975-a2d3-40e7-aad5-f68f4697812a"));

            migrationBuilder.DeleteData(
                table: "CondicionesMedicas",
                keyColumn: "Id",
                keyValue: new Guid("1f192bb2-99ac-4725-9f90-0a83fecf0802"));

            migrationBuilder.DeleteData(
                table: "CondicionesMedicas",
                keyColumn: "Id",
                keyValue: new Guid("7cb101c0-3467-4671-9673-ca4f485cd5cf"));

            migrationBuilder.DeleteData(
                table: "CondicionesMedicas",
                keyColumn: "Id",
                keyValue: new Guid("800c6326-7351-4132-8807-d6313cb586c4"));

            migrationBuilder.DeleteData(
                table: "CondicionesMedicas",
                keyColumn: "Id",
                keyValue: new Guid("94c58ba4-fedb-42bc-a121-9b6d65a1af59"));

            migrationBuilder.DeleteData(
                table: "CondicionesMedicas",
                keyColumn: "Id",
                keyValue: new Guid("a90feec1-4e14-4730-ab1a-c8b97e0515f4"));

            migrationBuilder.DeleteData(
                table: "CondicionesMedicas",
                keyColumn: "Id",
                keyValue: new Guid("b9ce1123-4673-420d-bbcc-e979e9b5a3bf"));

            migrationBuilder.DeleteData(
                table: "CondicionesMedicas",
                keyColumn: "Id",
                keyValue: new Guid("c1fbf544-c48b-4f2f-b4a8-476e6193240d"));

            migrationBuilder.DeleteData(
                table: "CondicionesMedicas",
                keyColumn: "Id",
                keyValue: new Guid("d9a326ab-f139-4548-bc33-e7a2dfce5fe8"));

            migrationBuilder.DeleteData(
                table: "CondicionesMedicas",
                keyColumn: "Id",
                keyValue: new Guid("df0fd0d9-ccbd-4ee0-85ef-cef99221394e"));

            migrationBuilder.DeleteData(
                table: "CondicionesMedicas",
                keyColumn: "Id",
                keyValue: new Guid("ea28f81d-8dfe-4558-93b6-19537a0d47af"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("03427ae8-a042-4868-9373-8ca80f97c9a4"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("0571ba30-490d-4ef4-ae9a-f935da97b3b0"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("0832bd35-f5db-43c0-8b65-17f372cb2378"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("0dee7103-d13b-47cf-886c-b3f0d4cb2e60"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("1c1c46d0-0d31-4107-b309-b4129f6c60d4"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("1d6363af-9a2b-4110-a58b-cfa444479ae3"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("1f31e2d9-0d7f-4a0e-818e-9f7951d67f79"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("254fe076-1791-43bb-940d-b72fc61f0aac"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("3bd6bc23-b7ad-4f5e-bbbb-4dbcf28f4d97"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("3f081178-d8e0-4399-8598-b02ff9a9c489"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("505fa8cf-d1bd-4799-804d-2735d691f0db"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("56b6d595-a1b7-48fc-9ae0-4cf291e1eb21"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("57494608-d0e8-48a5-ac5e-7c07afbb1831"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("58806a47-bd1b-4925-beec-658cff1a2b05"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("5fdc549f-8311-4f3c-a4c6-9e047e5698ca"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("679c82e9-f149-441a-8f6d-e790fb171323"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("7102923e-ef0d-4a24-af3c-a18cc0b394eb"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("758c61bf-cce5-402d-95b3-8c24926cdf7e"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("7ec6923b-71db-42da-87f0-9916e491ef44"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("80426c4e-c519-4c8f-baf8-3ba207032d78"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("9bc3b600-23a5-46e5-8b97-271960b5d18d"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("a7044fcb-2311-4cc8-a273-4fbdb112c626"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("aae43d0c-5915-40d0-b407-690ee84a25e6"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("b5b74a56-4cd8-4bab-ac12-a1f15b62f8be"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("b96d06d9-7c3a-454c-a167-dab3f1e3ac13"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("c5e5e944-810f-44ab-a665-5c5ea434a544"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("d24d4abd-7090-46e2-8b2f-2e343c829113"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("d74ff1ba-437f-458d-94ba-2b28482ad6fd"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("e2c9ae68-0846-4211-8a19-7456dea0033f"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("f485cf12-4fd7-4cee-a58b-78d5e3fdf5bf"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("f6b2c395-c742-46a6-a586-821bbe22a0d8"));

            migrationBuilder.DeleteData(
                table: "Restricciones",
                keyColumn: "Id",
                keyValue: new Guid("2ab8ac74-4cc3-476f-9e90-d7c63e012e08"));

            migrationBuilder.DeleteData(
                table: "Restricciones",
                keyColumn: "Id",
                keyValue: new Guid("5723b940-a18d-4f0f-97f6-480ea7d05b32"));

            migrationBuilder.DeleteData(
                table: "Restricciones",
                keyColumn: "Id",
                keyValue: new Guid("63a5147a-84a7-4193-9bcb-40ad10025b6c"));

            migrationBuilder.DeleteData(
                table: "Restricciones",
                keyColumn: "Id",
                keyValue: new Guid("731b89a2-8898-4557-94dc-b7ee5f1bcbec"));

            migrationBuilder.DeleteData(
                table: "Restricciones",
                keyColumn: "Id",
                keyValue: new Guid("88ad6a6d-2fce-4713-8e22-b56041c57be1"));

            migrationBuilder.DeleteData(
                table: "Restricciones",
                keyColumn: "Id",
                keyValue: new Guid("903d1554-2bbc-4e1c-8ba3-dc88f2d60b82"));

            migrationBuilder.DeleteData(
                table: "Restricciones",
                keyColumn: "Id",
                keyValue: new Guid("9dc4cee3-db39-4af1-980d-dc6c2811b989"));

            migrationBuilder.DeleteData(
                table: "Restricciones",
                keyColumn: "Id",
                keyValue: new Guid("abed4fde-7bb6-420c-935f-bc018a81f6a4"));

            migrationBuilder.DeleteData(
                table: "Restricciones",
                keyColumn: "Id",
                keyValue: new Guid("cb0209bc-2675-4515-ad0b-5889a660de17"));

            migrationBuilder.DeleteData(
                table: "Restricciones",
                keyColumn: "Id",
                keyValue: new Guid("d2507a73-8d64-45bc-bed5-3e1981613b70"));

            migrationBuilder.DeleteData(
                table: "Restricciones",
                keyColumn: "Id",
                keyValue: new Guid("e1cd3b11-8559-4364-ab35-513c48a9196e"));

            migrationBuilder.DeleteData(
                table: "Restricciones",
                keyColumn: "Id",
                keyValue: new Guid("f24a155c-408f-470f-8064-a8043de937ed"));

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("11637566-47f1-4d18-9bd7-573fdda77e13"));

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("31e31211-5d09-4782-9b5e-c523e7207bfb"));

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("35f41fd6-267d-4f75-8268-e9f21424796b"));

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("3d21eb50-be8b-4d1b-a88b-65bb2c564499"));

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("3eb25592-620d-4a4c-842b-7b3067bf2275"));

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("4b610fb2-f412-4e94-b145-635e2d942b65"));

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("57fc04c5-81f4-44c8-8f55-5a833b1e2a3b"));

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("93696e0e-6226-47df-9af1-4dee58470fda"));

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("b1ccc4a5-d618-4032-8855-9fdcd4444d94"));

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("b6027ef8-788c-47e1-85e1-85cebd901747"));

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("b78455c1-23de-4429-b7e3-f0e902fc735c"));

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("b9e5fd0f-e917-456e-b7fd-4ee80db5998f"));

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("bbe9d4dd-2890-4725-860e-2affab9eef19"));

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("c1f31540-1462-422c-b4d1-15a8ac24b6d7"));

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("c45e4daa-4a52-439c-aff7-8075f993cef4"));

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("c8f41e3d-a411-4b6c-ad16-a93f9c6847a7"));

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("d25dee01-f656-4e95-901c-adcf5afb9854"));

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("e4de5bca-1945-42ea-8f4f-737665b37adf"));

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("fb386e3b-675c-4086-9fd4-d431c754da96"));

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("fcebd133-af80-4b53-9af3-1e1013a187df"));

            migrationBuilder.InsertData(
                table: "CondicionesMedicas",
                columns: new[] { "Id", "Nombre" },
                values: new object[,]
                {
                    { new Guid("44444444-0001-0001-0001-000000000001"), "Diabetes" },
                    { new Guid("44444444-0001-0001-0001-000000000002"), "Hipertensión" },
                    { new Guid("44444444-0001-0001-0001-000000000003"), "Colesterol alto" },
                    { new Guid("44444444-0001-0001-0001-000000000004"), "Gastritis" },
                    { new Guid("44444444-0001-0001-0001-000000000005"), "Enfermedad celíaca" },
                    { new Guid("44444444-0001-0001-0001-000000000006"), "Intolerancia a la lactosa" },
                    { new Guid("44444444-0001-0001-0001-000000000007"), "Alergia a mariscos" },
                    { new Guid("44444444-0001-0001-0001-000000000008"), "Alergia a frutos secos" },
                    { new Guid("44444444-0001-0001-0001-000000000009"), "Alergia al huevo" },
                    { new Guid("44444444-0001-0001-0001-000000000010"), "Síndrome del intestino irritable" },
                    { new Guid("44444444-0001-0001-0001-000000000011"), "Gota" },
                    { new Guid("44444444-0001-0001-0001-000000000012"), "Ansiedad (sensibilidad a cafeína)" }
                });

            migrationBuilder.InsertData(
                table: "Gustos",
                columns: new[] { "Id", "ImagenUrl", "Nombre" },
                values: new object[,]
                {
                    { new Guid("22222222-0001-0001-0001-000000000001"), null, "Pizza" },
                    { new Guid("22222222-0001-0001-0001-000000000002"), null, "Sushi" },
                    { new Guid("22222222-0001-0001-0001-000000000003"), null, "Paella" },
                    { new Guid("22222222-0001-0001-0001-000000000004"), null, "Milanesa con papas" },
                    { new Guid("22222222-0001-0001-0001-000000000005"), null, "Tacos" },
                    { new Guid("22222222-0001-0001-0001-000000000006"), null, "Ensalada César" },
                    { new Guid("22222222-0001-0001-0001-000000000007"), null, "Ramen japonés" },
                    { new Guid("22222222-0001-0001-0001-000000000008"), null, "Empanadas" },
                    { new Guid("22222222-0001-0001-0001-000000000009"), null, "Ceviche" },
                    { new Guid("22222222-0001-0001-0001-000000000010"), null, "Helado" },
                    { new Guid("22222222-0001-0001-0001-000000000011"), null, "Hamburguesa" },
                    { new Guid("22222222-0001-0001-0001-000000000012"), null, "Papas fritas" },
                    { new Guid("22222222-0001-0001-0001-000000000013"), null, "Pollo grillado" },
                    { new Guid("22222222-0001-0001-0001-000000000014"), null, "Kebab" },
                    { new Guid("22222222-0001-0001-0001-000000000015"), null, "Ensalada verde" },
                    { new Guid("22222222-0001-0001-0001-000000000016"), null, "Asado" },
                    { new Guid("22222222-0001-0001-0001-000000000017"), null, "Sopa de verduras" },
                    { new Guid("22222222-0001-0001-0001-000000000018"), null, "Panqueques" },
                    { new Guid("22222222-0001-0001-0001-000000000019"), null, "Café con leche" },
                    { new Guid("22222222-0001-0001-0001-000000000020"), null, "Jugo natural" },
                    { new Guid("22222222-0001-0001-0001-000000000021"), null, "Smoothie de frutas" },
                    { new Guid("22222222-0001-0001-0001-000000000022"), null, "Chocolate" },
                    { new Guid("22222222-0001-0001-0001-000000000023"), null, "Tarta de manzana" },
                    { new Guid("22222222-0001-0001-0001-000000000024"), null, "Pescado al horno" },
                    { new Guid("22222222-0001-0001-0001-000000000025"), null, "Pasta boloñesa" },
                    { new Guid("22222222-0001-0001-0001-000000000026"), null, "Lomo a la pimienta" },
                    { new Guid("22222222-0001-0001-0001-000000000027"), null, "Ensalada de frutas" },
                    { new Guid("22222222-0001-0001-0001-000000000028"), null, "Sándwich de jamón y queso" },
                    { new Guid("22222222-0001-0001-0001-000000000029"), null, "Huevos revueltos" },
                    { new Guid("22222222-0001-0001-0001-000000000030"), null, "Cerveza artesanal" },
                    { new Guid("22222222-0001-0001-0001-000000000031"), null, "Vino tinto" }
                });

            migrationBuilder.InsertData(
                table: "Restricciones",
                columns: new[] { "Id", "Nombre" },
                values: new object[,]
                {
                    { new Guid("33333333-0001-0001-0001-000000000001"), "Sin gluten" },
                    { new Guid("33333333-0001-0001-0001-000000000002"), "Sin lactosa" },
                    { new Guid("33333333-0001-0001-0001-000000000003"), "Sin azúcar" },
                    { new Guid("33333333-0001-0001-0001-000000000004"), "Sin sal" },
                    { new Guid("33333333-0001-0001-0001-000000000005"), "Sin mariscos" },
                    { new Guid("33333333-0001-0001-0001-000000000006"), "Sin carne roja" },
                    { new Guid("33333333-0001-0001-0001-000000000007"), "Sin frito" },
                    { new Guid("33333333-0001-0001-0001-000000000008"), "Sin picante" },
                    { new Guid("33333333-0001-0001-0001-000000000009"), "Sin cafeína" },
                    { new Guid("33333333-0001-0001-0001-000000000010"), "Sin alcohol" },
                    { new Guid("33333333-0001-0001-0001-000000000011"), "Sin soja" },
                    { new Guid("33333333-0001-0001-0001-000000000012"), "Sin frutos secos" }
                });

            migrationBuilder.InsertData(
                table: "Tags",
                columns: new[] { "Id", "Nombre", "Tipo" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "Gluten", "Ingrediente" },
                    { new Guid("11111111-1111-1111-1111-111111111112"), "Lácteo", "Ingrediente" },
                    { new Guid("11111111-1111-1111-1111-111111111113"), "Azúcar", "Nutriente" },
                    { new Guid("11111111-1111-1111-1111-111111111114"), "Sal", "Nutriente" },
                    { new Guid("11111111-1111-1111-1111-111111111115"), "Mariscos", "Ingrediente" },
                    { new Guid("11111111-1111-1111-1111-111111111116"), "Carne roja", "Ingrediente" },
                    { new Guid("11111111-1111-1111-1111-111111111117"), "Carne blanca", "Ingrediente" },
                    { new Guid("11111111-1111-1111-1111-111111111118"), "Vegetal", "Categoria" },
                    { new Guid("11111111-1111-1111-1111-111111111119"), "Fruta", "Categoria" },
                    { new Guid("11111111-1111-1111-1111-111111111120"), "Frito", "Categoria" },
                    { new Guid("11111111-1111-1111-1111-111111111121"), "Picante", "Categoria" },
                    { new Guid("11111111-1111-1111-1111-111111111122"), "Procesado", "Categoria" },
                    { new Guid("11111111-1111-1111-1111-111111111123"), "Pescado", "Ingrediente" },
                    { new Guid("11111111-1111-1111-1111-111111111124"), "Grasa", "Nutriente" },
                    { new Guid("11111111-1111-1111-1111-111111111125"), "Cafeína", "Nutriente" },
                    { new Guid("11111111-1111-1111-1111-111111111126"), "Harina", "Ingrediente" },
                    { new Guid("11111111-1111-1111-1111-111111111127"), "Huevos", "Ingrediente" },
                    { new Guid("11111111-1111-1111-1111-111111111128"), "Frutos secos", "Ingrediente" },
                    { new Guid("11111111-1111-1111-1111-111111111129"), "Soja", "Ingrediente" },
                    { new Guid("11111111-1111-1111-1111-111111111130"), "Alcohol", "Nutriente" }
                });

            migrationBuilder.InsertData(
                table: "CondicionMedicaTags",
                columns: new[] { "CondicionesMedicasId", "TagsId" },
                values: new object[,]
                {
                    { new Guid("44444444-0001-0001-0001-000000000001"), new Guid("11111111-1111-1111-1111-111111111113") },
                    { new Guid("44444444-0001-0001-0001-000000000002"), new Guid("11111111-1111-1111-1111-111111111114") },
                    { new Guid("44444444-0001-0001-0001-000000000003"), new Guid("11111111-1111-1111-1111-111111111124") },
                    { new Guid("44444444-0001-0001-0001-000000000004"), new Guid("11111111-1111-1111-1111-111111111121") },
                    { new Guid("44444444-0001-0001-0001-000000000005"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("44444444-0001-0001-0001-000000000006"), new Guid("11111111-1111-1111-1111-111111111112") },
                    { new Guid("44444444-0001-0001-0001-000000000007"), new Guid("11111111-1111-1111-1111-111111111115") },
                    { new Guid("44444444-0001-0001-0001-000000000008"), new Guid("11111111-1111-1111-1111-111111111128") },
                    { new Guid("44444444-0001-0001-0001-000000000009"), new Guid("11111111-1111-1111-1111-111111111127") },
                    { new Guid("44444444-0001-0001-0001-000000000010"), new Guid("11111111-1111-1111-1111-111111111120") },
                    { new Guid("44444444-0001-0001-0001-000000000011"), new Guid("11111111-1111-1111-1111-111111111116") },
                    { new Guid("44444444-0001-0001-0001-000000000012"), new Guid("11111111-1111-1111-1111-111111111125") }
                });

            migrationBuilder.InsertData(
                table: "GustoTags",
                columns: new[] { "GustosId", "TagsId" },
                values: new object[,]
                {
                    { new Guid("22222222-0001-0001-0001-000000000001"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("22222222-0001-0001-0001-000000000001"), new Guid("11111111-1111-1111-1111-111111111112") },
                    { new Guid("22222222-0001-0001-0001-000000000001"), new Guid("11111111-1111-1111-1111-111111111114") },
                    { new Guid("22222222-0001-0001-0001-000000000002"), new Guid("11111111-1111-1111-1111-111111111114") },
                    { new Guid("22222222-0001-0001-0001-000000000002"), new Guid("11111111-1111-1111-1111-111111111123") },
                    { new Guid("22222222-0001-0001-0001-000000000003"), new Guid("11111111-1111-1111-1111-111111111114") },
                    { new Guid("22222222-0001-0001-0001-000000000003"), new Guid("11111111-1111-1111-1111-111111111115") },
                    { new Guid("22222222-0001-0001-0001-000000000004"), new Guid("11111111-1111-1111-1111-111111111116") },
                    { new Guid("22222222-0001-0001-0001-000000000004"), new Guid("11111111-1111-1111-1111-111111111120") },
                    { new Guid("22222222-0001-0001-0001-000000000004"), new Guid("11111111-1111-1111-1111-111111111126") },
                    { new Guid("22222222-0001-0001-0001-000000000005"), new Guid("11111111-1111-1111-1111-111111111116") },
                    { new Guid("22222222-0001-0001-0001-000000000005"), new Guid("11111111-1111-1111-1111-111111111121") },
                    { new Guid("22222222-0001-0001-0001-000000000006"), new Guid("11111111-1111-1111-1111-111111111112") },
                    { new Guid("22222222-0001-0001-0001-000000000006"), new Guid("11111111-1111-1111-1111-111111111114") },
                    { new Guid("22222222-0001-0001-0001-000000000006"), new Guid("11111111-1111-1111-1111-111111111118") },
                    { new Guid("22222222-0001-0001-0001-000000000007"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("22222222-0001-0001-0001-000000000007"), new Guid("11111111-1111-1111-1111-111111111114") },
                    { new Guid("22222222-0001-0001-0001-000000000007"), new Guid("11111111-1111-1111-1111-111111111121") },
                    { new Guid("22222222-0001-0001-0001-000000000008"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("22222222-0001-0001-0001-000000000008"), new Guid("11111111-1111-1111-1111-111111111116") },
                    { new Guid("22222222-0001-0001-0001-000000000008"), new Guid("11111111-1111-1111-1111-111111111120") },
                    { new Guid("22222222-0001-0001-0001-000000000009"), new Guid("11111111-1111-1111-1111-111111111115") },
                    { new Guid("22222222-0001-0001-0001-000000000009"), new Guid("11111111-1111-1111-1111-111111111118") },
                    { new Guid("22222222-0001-0001-0001-000000000009"), new Guid("11111111-1111-1111-1111-111111111123") },
                    { new Guid("22222222-0001-0001-0001-000000000010"), new Guid("11111111-1111-1111-1111-111111111112") },
                    { new Guid("22222222-0001-0001-0001-000000000010"), new Guid("11111111-1111-1111-1111-111111111113") },
                    { new Guid("22222222-0001-0001-0001-000000000011"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("22222222-0001-0001-0001-000000000011"), new Guid("11111111-1111-1111-1111-111111111116") },
                    { new Guid("22222222-0001-0001-0001-000000000011"), new Guid("11111111-1111-1111-1111-111111111120") },
                    { new Guid("22222222-0001-0001-0001-000000000012"), new Guid("11111111-1111-1111-1111-111111111114") },
                    { new Guid("22222222-0001-0001-0001-000000000012"), new Guid("11111111-1111-1111-1111-111111111120") },
                    { new Guid("22222222-0001-0001-0001-000000000013"), new Guid("11111111-1111-1111-1111-111111111117") },
                    { new Guid("22222222-0001-0001-0001-000000000013"), new Guid("11111111-1111-1111-1111-111111111118") },
                    { new Guid("22222222-0001-0001-0001-000000000014"), new Guid("11111111-1111-1111-1111-111111111116") },
                    { new Guid("22222222-0001-0001-0001-000000000014"), new Guid("11111111-1111-1111-1111-111111111121") },
                    { new Guid("22222222-0001-0001-0001-000000000014"), new Guid("11111111-1111-1111-1111-111111111124") },
                    { new Guid("22222222-0001-0001-0001-000000000015"), new Guid("11111111-1111-1111-1111-111111111118") },
                    { new Guid("22222222-0001-0001-0001-000000000016"), new Guid("11111111-1111-1111-1111-111111111114") },
                    { new Guid("22222222-0001-0001-0001-000000000016"), new Guid("11111111-1111-1111-1111-111111111116") },
                    { new Guid("22222222-0001-0001-0001-000000000016"), new Guid("11111111-1111-1111-1111-111111111124") },
                    { new Guid("22222222-0001-0001-0001-000000000017"), new Guid("11111111-1111-1111-1111-111111111114") },
                    { new Guid("22222222-0001-0001-0001-000000000017"), new Guid("11111111-1111-1111-1111-111111111118") },
                    { new Guid("22222222-0001-0001-0001-000000000018"), new Guid("11111111-1111-1111-1111-111111111113") },
                    { new Guid("22222222-0001-0001-0001-000000000018"), new Guid("11111111-1111-1111-1111-111111111126") },
                    { new Guid("22222222-0001-0001-0001-000000000018"), new Guid("11111111-1111-1111-1111-111111111127") },
                    { new Guid("22222222-0001-0001-0001-000000000019"), new Guid("11111111-1111-1111-1111-111111111112") },
                    { new Guid("22222222-0001-0001-0001-000000000019"), new Guid("11111111-1111-1111-1111-111111111125") },
                    { new Guid("22222222-0001-0001-0001-000000000020"), new Guid("11111111-1111-1111-1111-111111111119") },
                    { new Guid("22222222-0001-0001-0001-000000000021"), new Guid("11111111-1111-1111-1111-111111111113") },
                    { new Guid("22222222-0001-0001-0001-000000000021"), new Guid("11111111-1111-1111-1111-111111111119") },
                    { new Guid("22222222-0001-0001-0001-000000000022"), new Guid("11111111-1111-1111-1111-111111111113") },
                    { new Guid("22222222-0001-0001-0001-000000000022"), new Guid("11111111-1111-1111-1111-111111111124") },
                    { new Guid("22222222-0001-0001-0001-000000000023"), new Guid("11111111-1111-1111-1111-111111111113") },
                    { new Guid("22222222-0001-0001-0001-000000000023"), new Guid("11111111-1111-1111-1111-111111111119") },
                    { new Guid("22222222-0001-0001-0001-000000000023"), new Guid("11111111-1111-1111-1111-111111111126") },
                    { new Guid("22222222-0001-0001-0001-000000000024"), new Guid("11111111-1111-1111-1111-111111111118") },
                    { new Guid("22222222-0001-0001-0001-000000000024"), new Guid("11111111-1111-1111-1111-111111111123") },
                    { new Guid("22222222-0001-0001-0001-000000000025"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("22222222-0001-0001-0001-000000000025"), new Guid("11111111-1111-1111-1111-111111111116") },
                    { new Guid("22222222-0001-0001-0001-000000000026"), new Guid("11111111-1111-1111-1111-111111111114") },
                    { new Guid("22222222-0001-0001-0001-000000000026"), new Guid("11111111-1111-1111-1111-111111111116") },
                    { new Guid("22222222-0001-0001-0001-000000000027"), new Guid("11111111-1111-1111-1111-111111111119") },
                    { new Guid("22222222-0001-0001-0001-000000000028"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("22222222-0001-0001-0001-000000000028"), new Guid("11111111-1111-1111-1111-111111111112") },
                    { new Guid("22222222-0001-0001-0001-000000000028"), new Guid("11111111-1111-1111-1111-111111111122") },
                    { new Guid("22222222-0001-0001-0001-000000000029"), new Guid("11111111-1111-1111-1111-111111111127") },
                    { new Guid("22222222-0001-0001-0001-000000000030"), new Guid("11111111-1111-1111-1111-111111111130") },
                    { new Guid("22222222-0001-0001-0001-000000000031"), new Guid("11111111-1111-1111-1111-111111111130") }
                });

            migrationBuilder.InsertData(
                table: "RestriccionTags",
                columns: new[] { "RestriccionesId", "TagsId" },
                values: new object[,]
                {
                    { new Guid("33333333-0001-0001-0001-000000000001"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("33333333-0001-0001-0001-000000000002"), new Guid("11111111-1111-1111-1111-111111111112") },
                    { new Guid("33333333-0001-0001-0001-000000000003"), new Guid("11111111-1111-1111-1111-111111111113") },
                    { new Guid("33333333-0001-0001-0001-000000000004"), new Guid("11111111-1111-1111-1111-111111111114") },
                    { new Guid("33333333-0001-0001-0001-000000000005"), new Guid("11111111-1111-1111-1111-111111111115") },
                    { new Guid("33333333-0001-0001-0001-000000000006"), new Guid("11111111-1111-1111-1111-111111111116") },
                    { new Guid("33333333-0001-0001-0001-000000000007"), new Guid("11111111-1111-1111-1111-111111111120") },
                    { new Guid("33333333-0001-0001-0001-000000000008"), new Guid("11111111-1111-1111-1111-111111111121") },
                    { new Guid("33333333-0001-0001-0001-000000000009"), new Guid("11111111-1111-1111-1111-111111111125") },
                    { new Guid("33333333-0001-0001-0001-000000000010"), new Guid("11111111-1111-1111-1111-111111111130") },
                    { new Guid("33333333-0001-0001-0001-000000000011"), new Guid("11111111-1111-1111-1111-111111111129") },
                    { new Guid("33333333-0001-0001-0001-000000000012"), new Guid("11111111-1111-1111-1111-111111111128") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "CondicionMedicaTags",
                keyColumns: new[] { "CondicionesMedicasId", "TagsId" },
                keyValues: new object[] { new Guid("44444444-0001-0001-0001-000000000001"), new Guid("11111111-1111-1111-1111-111111111113") });

            migrationBuilder.DeleteData(
                table: "CondicionMedicaTags",
                keyColumns: new[] { "CondicionesMedicasId", "TagsId" },
                keyValues: new object[] { new Guid("44444444-0001-0001-0001-000000000002"), new Guid("11111111-1111-1111-1111-111111111114") });

            migrationBuilder.DeleteData(
                table: "CondicionMedicaTags",
                keyColumns: new[] { "CondicionesMedicasId", "TagsId" },
                keyValues: new object[] { new Guid("44444444-0001-0001-0001-000000000003"), new Guid("11111111-1111-1111-1111-111111111124") });

            migrationBuilder.DeleteData(
                table: "CondicionMedicaTags",
                keyColumns: new[] { "CondicionesMedicasId", "TagsId" },
                keyValues: new object[] { new Guid("44444444-0001-0001-0001-000000000004"), new Guid("11111111-1111-1111-1111-111111111121") });

            migrationBuilder.DeleteData(
                table: "CondicionMedicaTags",
                keyColumns: new[] { "CondicionesMedicasId", "TagsId" },
                keyValues: new object[] { new Guid("44444444-0001-0001-0001-000000000005"), new Guid("11111111-1111-1111-1111-111111111111") });

            migrationBuilder.DeleteData(
                table: "CondicionMedicaTags",
                keyColumns: new[] { "CondicionesMedicasId", "TagsId" },
                keyValues: new object[] { new Guid("44444444-0001-0001-0001-000000000006"), new Guid("11111111-1111-1111-1111-111111111112") });

            migrationBuilder.DeleteData(
                table: "CondicionMedicaTags",
                keyColumns: new[] { "CondicionesMedicasId", "TagsId" },
                keyValues: new object[] { new Guid("44444444-0001-0001-0001-000000000007"), new Guid("11111111-1111-1111-1111-111111111115") });

            migrationBuilder.DeleteData(
                table: "CondicionMedicaTags",
                keyColumns: new[] { "CondicionesMedicasId", "TagsId" },
                keyValues: new object[] { new Guid("44444444-0001-0001-0001-000000000008"), new Guid("11111111-1111-1111-1111-111111111128") });

            migrationBuilder.DeleteData(
                table: "CondicionMedicaTags",
                keyColumns: new[] { "CondicionesMedicasId", "TagsId" },
                keyValues: new object[] { new Guid("44444444-0001-0001-0001-000000000009"), new Guid("11111111-1111-1111-1111-111111111127") });

            migrationBuilder.DeleteData(
                table: "CondicionMedicaTags",
                keyColumns: new[] { "CondicionesMedicasId", "TagsId" },
                keyValues: new object[] { new Guid("44444444-0001-0001-0001-000000000010"), new Guid("11111111-1111-1111-1111-111111111120") });

            migrationBuilder.DeleteData(
                table: "CondicionMedicaTags",
                keyColumns: new[] { "CondicionesMedicasId", "TagsId" },
                keyValues: new object[] { new Guid("44444444-0001-0001-0001-000000000011"), new Guid("11111111-1111-1111-1111-111111111116") });

            migrationBuilder.DeleteData(
                table: "CondicionMedicaTags",
                keyColumns: new[] { "CondicionesMedicasId", "TagsId" },
                keyValues: new object[] { new Guid("44444444-0001-0001-0001-000000000012"), new Guid("11111111-1111-1111-1111-111111111125") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000001"), new Guid("11111111-1111-1111-1111-111111111111") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000001"), new Guid("11111111-1111-1111-1111-111111111112") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000001"), new Guid("11111111-1111-1111-1111-111111111114") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000002"), new Guid("11111111-1111-1111-1111-111111111114") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000002"), new Guid("11111111-1111-1111-1111-111111111123") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000003"), new Guid("11111111-1111-1111-1111-111111111114") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000003"), new Guid("11111111-1111-1111-1111-111111111115") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000004"), new Guid("11111111-1111-1111-1111-111111111116") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000004"), new Guid("11111111-1111-1111-1111-111111111120") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000004"), new Guid("11111111-1111-1111-1111-111111111126") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000005"), new Guid("11111111-1111-1111-1111-111111111116") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000005"), new Guid("11111111-1111-1111-1111-111111111121") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000006"), new Guid("11111111-1111-1111-1111-111111111112") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000006"), new Guid("11111111-1111-1111-1111-111111111114") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000006"), new Guid("11111111-1111-1111-1111-111111111118") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000007"), new Guid("11111111-1111-1111-1111-111111111111") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000007"), new Guid("11111111-1111-1111-1111-111111111114") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000007"), new Guid("11111111-1111-1111-1111-111111111121") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000008"), new Guid("11111111-1111-1111-1111-111111111111") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000008"), new Guid("11111111-1111-1111-1111-111111111116") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000008"), new Guid("11111111-1111-1111-1111-111111111120") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000009"), new Guid("11111111-1111-1111-1111-111111111115") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000009"), new Guid("11111111-1111-1111-1111-111111111118") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000009"), new Guid("11111111-1111-1111-1111-111111111123") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000010"), new Guid("11111111-1111-1111-1111-111111111112") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000010"), new Guid("11111111-1111-1111-1111-111111111113") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000011"), new Guid("11111111-1111-1111-1111-111111111111") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000011"), new Guid("11111111-1111-1111-1111-111111111116") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000011"), new Guid("11111111-1111-1111-1111-111111111120") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000012"), new Guid("11111111-1111-1111-1111-111111111114") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000012"), new Guid("11111111-1111-1111-1111-111111111120") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000013"), new Guid("11111111-1111-1111-1111-111111111117") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000013"), new Guid("11111111-1111-1111-1111-111111111118") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000014"), new Guid("11111111-1111-1111-1111-111111111116") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000014"), new Guid("11111111-1111-1111-1111-111111111121") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000014"), new Guid("11111111-1111-1111-1111-111111111124") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000015"), new Guid("11111111-1111-1111-1111-111111111118") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000016"), new Guid("11111111-1111-1111-1111-111111111114") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000016"), new Guid("11111111-1111-1111-1111-111111111116") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000016"), new Guid("11111111-1111-1111-1111-111111111124") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000017"), new Guid("11111111-1111-1111-1111-111111111114") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000017"), new Guid("11111111-1111-1111-1111-111111111118") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000018"), new Guid("11111111-1111-1111-1111-111111111113") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000018"), new Guid("11111111-1111-1111-1111-111111111126") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000018"), new Guid("11111111-1111-1111-1111-111111111127") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000019"), new Guid("11111111-1111-1111-1111-111111111112") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000019"), new Guid("11111111-1111-1111-1111-111111111125") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000020"), new Guid("11111111-1111-1111-1111-111111111119") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000021"), new Guid("11111111-1111-1111-1111-111111111113") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000021"), new Guid("11111111-1111-1111-1111-111111111119") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000022"), new Guid("11111111-1111-1111-1111-111111111113") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000022"), new Guid("11111111-1111-1111-1111-111111111124") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000023"), new Guid("11111111-1111-1111-1111-111111111113") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000023"), new Guid("11111111-1111-1111-1111-111111111119") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000023"), new Guid("11111111-1111-1111-1111-111111111126") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000024"), new Guid("11111111-1111-1111-1111-111111111118") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000024"), new Guid("11111111-1111-1111-1111-111111111123") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000025"), new Guid("11111111-1111-1111-1111-111111111111") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000025"), new Guid("11111111-1111-1111-1111-111111111116") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000026"), new Guid("11111111-1111-1111-1111-111111111114") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000026"), new Guid("11111111-1111-1111-1111-111111111116") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000027"), new Guid("11111111-1111-1111-1111-111111111119") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000028"), new Guid("11111111-1111-1111-1111-111111111111") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000028"), new Guid("11111111-1111-1111-1111-111111111112") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000028"), new Guid("11111111-1111-1111-1111-111111111122") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000029"), new Guid("11111111-1111-1111-1111-111111111127") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000030"), new Guid("11111111-1111-1111-1111-111111111130") });

            migrationBuilder.DeleteData(
                table: "GustoTags",
                keyColumns: new[] { "GustosId", "TagsId" },
                keyValues: new object[] { new Guid("22222222-0001-0001-0001-000000000031"), new Guid("11111111-1111-1111-1111-111111111130") });

            migrationBuilder.DeleteData(
                table: "RestriccionTags",
                keyColumns: new[] { "RestriccionesId", "TagsId" },
                keyValues: new object[] { new Guid("33333333-0001-0001-0001-000000000001"), new Guid("11111111-1111-1111-1111-111111111111") });

            migrationBuilder.DeleteData(
                table: "RestriccionTags",
                keyColumns: new[] { "RestriccionesId", "TagsId" },
                keyValues: new object[] { new Guid("33333333-0001-0001-0001-000000000002"), new Guid("11111111-1111-1111-1111-111111111112") });

            migrationBuilder.DeleteData(
                table: "RestriccionTags",
                keyColumns: new[] { "RestriccionesId", "TagsId" },
                keyValues: new object[] { new Guid("33333333-0001-0001-0001-000000000003"), new Guid("11111111-1111-1111-1111-111111111113") });

            migrationBuilder.DeleteData(
                table: "RestriccionTags",
                keyColumns: new[] { "RestriccionesId", "TagsId" },
                keyValues: new object[] { new Guid("33333333-0001-0001-0001-000000000004"), new Guid("11111111-1111-1111-1111-111111111114") });

            migrationBuilder.DeleteData(
                table: "RestriccionTags",
                keyColumns: new[] { "RestriccionesId", "TagsId" },
                keyValues: new object[] { new Guid("33333333-0001-0001-0001-000000000005"), new Guid("11111111-1111-1111-1111-111111111115") });

            migrationBuilder.DeleteData(
                table: "RestriccionTags",
                keyColumns: new[] { "RestriccionesId", "TagsId" },
                keyValues: new object[] { new Guid("33333333-0001-0001-0001-000000000006"), new Guid("11111111-1111-1111-1111-111111111116") });

            migrationBuilder.DeleteData(
                table: "RestriccionTags",
                keyColumns: new[] { "RestriccionesId", "TagsId" },
                keyValues: new object[] { new Guid("33333333-0001-0001-0001-000000000007"), new Guid("11111111-1111-1111-1111-111111111120") });

            migrationBuilder.DeleteData(
                table: "RestriccionTags",
                keyColumns: new[] { "RestriccionesId", "TagsId" },
                keyValues: new object[] { new Guid("33333333-0001-0001-0001-000000000008"), new Guid("11111111-1111-1111-1111-111111111121") });

            migrationBuilder.DeleteData(
                table: "RestriccionTags",
                keyColumns: new[] { "RestriccionesId", "TagsId" },
                keyValues: new object[] { new Guid("33333333-0001-0001-0001-000000000009"), new Guid("11111111-1111-1111-1111-111111111125") });

            migrationBuilder.DeleteData(
                table: "RestriccionTags",
                keyColumns: new[] { "RestriccionesId", "TagsId" },
                keyValues: new object[] { new Guid("33333333-0001-0001-0001-000000000010"), new Guid("11111111-1111-1111-1111-111111111130") });

            migrationBuilder.DeleteData(
                table: "RestriccionTags",
                keyColumns: new[] { "RestriccionesId", "TagsId" },
                keyValues: new object[] { new Guid("33333333-0001-0001-0001-000000000011"), new Guid("11111111-1111-1111-1111-111111111129") });

            migrationBuilder.DeleteData(
                table: "RestriccionTags",
                keyColumns: new[] { "RestriccionesId", "TagsId" },
                keyValues: new object[] { new Guid("33333333-0001-0001-0001-000000000012"), new Guid("11111111-1111-1111-1111-111111111128") });

            migrationBuilder.DeleteData(
                table: "CondicionesMedicas",
                keyColumn: "Id",
                keyValue: new Guid("44444444-0001-0001-0001-000000000001"));

            migrationBuilder.DeleteData(
                table: "CondicionesMedicas",
                keyColumn: "Id",
                keyValue: new Guid("44444444-0001-0001-0001-000000000002"));

            migrationBuilder.DeleteData(
                table: "CondicionesMedicas",
                keyColumn: "Id",
                keyValue: new Guid("44444444-0001-0001-0001-000000000003"));

            migrationBuilder.DeleteData(
                table: "CondicionesMedicas",
                keyColumn: "Id",
                keyValue: new Guid("44444444-0001-0001-0001-000000000004"));

            migrationBuilder.DeleteData(
                table: "CondicionesMedicas",
                keyColumn: "Id",
                keyValue: new Guid("44444444-0001-0001-0001-000000000005"));

            migrationBuilder.DeleteData(
                table: "CondicionesMedicas",
                keyColumn: "Id",
                keyValue: new Guid("44444444-0001-0001-0001-000000000006"));

            migrationBuilder.DeleteData(
                table: "CondicionesMedicas",
                keyColumn: "Id",
                keyValue: new Guid("44444444-0001-0001-0001-000000000007"));

            migrationBuilder.DeleteData(
                table: "CondicionesMedicas",
                keyColumn: "Id",
                keyValue: new Guid("44444444-0001-0001-0001-000000000008"));

            migrationBuilder.DeleteData(
                table: "CondicionesMedicas",
                keyColumn: "Id",
                keyValue: new Guid("44444444-0001-0001-0001-000000000009"));

            migrationBuilder.DeleteData(
                table: "CondicionesMedicas",
                keyColumn: "Id",
                keyValue: new Guid("44444444-0001-0001-0001-000000000010"));

            migrationBuilder.DeleteData(
                table: "CondicionesMedicas",
                keyColumn: "Id",
                keyValue: new Guid("44444444-0001-0001-0001-000000000011"));

            migrationBuilder.DeleteData(
                table: "CondicionesMedicas",
                keyColumn: "Id",
                keyValue: new Guid("44444444-0001-0001-0001-000000000012"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000001"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000002"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000003"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000004"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000005"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000006"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000007"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000008"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000009"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000010"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000011"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000012"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000013"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000014"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000015"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000016"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000017"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000018"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000019"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000020"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000021"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000022"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000023"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000024"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000025"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000026"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000027"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000028"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000029"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000030"));

            migrationBuilder.DeleteData(
                table: "Gustos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0001-0001-0001-000000000031"));

            migrationBuilder.DeleteData(
                table: "Restricciones",
                keyColumn: "Id",
                keyValue: new Guid("33333333-0001-0001-0001-000000000001"));

            migrationBuilder.DeleteData(
                table: "Restricciones",
                keyColumn: "Id",
                keyValue: new Guid("33333333-0001-0001-0001-000000000002"));

            migrationBuilder.DeleteData(
                table: "Restricciones",
                keyColumn: "Id",
                keyValue: new Guid("33333333-0001-0001-0001-000000000003"));

            migrationBuilder.DeleteData(
                table: "Restricciones",
                keyColumn: "Id",
                keyValue: new Guid("33333333-0001-0001-0001-000000000004"));

            migrationBuilder.DeleteData(
                table: "Restricciones",
                keyColumn: "Id",
                keyValue: new Guid("33333333-0001-0001-0001-000000000005"));

            migrationBuilder.DeleteData(
                table: "Restricciones",
                keyColumn: "Id",
                keyValue: new Guid("33333333-0001-0001-0001-000000000006"));

            migrationBuilder.DeleteData(
                table: "Restricciones",
                keyColumn: "Id",
                keyValue: new Guid("33333333-0001-0001-0001-000000000007"));

            migrationBuilder.DeleteData(
                table: "Restricciones",
                keyColumn: "Id",
                keyValue: new Guid("33333333-0001-0001-0001-000000000008"));

            migrationBuilder.DeleteData(
                table: "Restricciones",
                keyColumn: "Id",
                keyValue: new Guid("33333333-0001-0001-0001-000000000009"));

            migrationBuilder.DeleteData(
                table: "Restricciones",
                keyColumn: "Id",
                keyValue: new Guid("33333333-0001-0001-0001-000000000010"));

            migrationBuilder.DeleteData(
                table: "Restricciones",
                keyColumn: "Id",
                keyValue: new Guid("33333333-0001-0001-0001-000000000011"));

            migrationBuilder.DeleteData(
                table: "Restricciones",
                keyColumn: "Id",
                keyValue: new Guid("33333333-0001-0001-0001-000000000012"));

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111112"));

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111113"));

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111114"));

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111115"));

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111116"));

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111117"));

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111118"));

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111119"));

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111120"));

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111121"));

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111122"));

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111123"));

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111124"));

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111125"));

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111126"));

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111127"));

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111128"));

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111129"));

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111130"));

            migrationBuilder.InsertData(
                table: "CondicionesMedicas",
                columns: new[] { "Id", "Nombre" },
                values: new object[,]
                {
                    { new Guid("0a1b5b30-3940-46f6-8b06-d094f5da9a18"), "Enfermedad celíaca" },
                    { new Guid("0fba5975-a2d3-40e7-aad5-f68f4697812a"), "Colesterol alto" },
                    { new Guid("1f192bb2-99ac-4725-9f90-0a83fecf0802"), "Alergia al huevo" },
                    { new Guid("7cb101c0-3467-4671-9673-ca4f485cd5cf"), "Intolerancia a la lactosa" },
                    { new Guid("800c6326-7351-4132-8807-d6313cb586c4"), "Gastritis" },
                    { new Guid("94c58ba4-fedb-42bc-a121-9b6d65a1af59"), "Alergia a mariscos" },
                    { new Guid("a90feec1-4e14-4730-ab1a-c8b97e0515f4"), "Hipertensión" },
                    { new Guid("b9ce1123-4673-420d-bbcc-e979e9b5a3bf"), "Gota" },
                    { new Guid("c1fbf544-c48b-4f2f-b4a8-476e6193240d"), "Alergia a frutos secos" },
                    { new Guid("d9a326ab-f139-4548-bc33-e7a2dfce5fe8"), "Síndrome del intestino irritable" },
                    { new Guid("df0fd0d9-ccbd-4ee0-85ef-cef99221394e"), "Diabetes" },
                    { new Guid("ea28f81d-8dfe-4558-93b6-19537a0d47af"), "Ansiedad (sensibilidad a cafeína)" }
                });

            migrationBuilder.InsertData(
                table: "Gustos",
                columns: new[] { "Id", "ImagenUrl", "Nombre" },
                values: new object[,]
                {
                    { new Guid("03427ae8-a042-4868-9373-8ca80f97c9a4"), null, "Lomo a la pimienta" },
                    { new Guid("0571ba30-490d-4ef4-ae9a-f935da97b3b0"), null, "Vino tinto" },
                    { new Guid("0832bd35-f5db-43c0-8b65-17f372cb2378"), null, "Café con leche" },
                    { new Guid("0dee7103-d13b-47cf-886c-b3f0d4cb2e60"), null, "Ensalada de frutas" },
                    { new Guid("1c1c46d0-0d31-4107-b309-b4129f6c60d4"), null, "Asado" },
                    { new Guid("1d6363af-9a2b-4110-a58b-cfa444479ae3"), null, "Sopa de verduras" },
                    { new Guid("1f31e2d9-0d7f-4a0e-818e-9f7951d67f79"), null, "Helado" },
                    { new Guid("254fe076-1791-43bb-940d-b72fc61f0aac"), null, "Pizza" },
                    { new Guid("3bd6bc23-b7ad-4f5e-bbbb-4dbcf28f4d97"), null, "Sushi" },
                    { new Guid("3f081178-d8e0-4399-8598-b02ff9a9c489"), null, "Ensalada verde" },
                    { new Guid("505fa8cf-d1bd-4799-804d-2735d691f0db"), null, "Panqueques" },
                    { new Guid("56b6d595-a1b7-48fc-9ae0-4cf291e1eb21"), null, "Pasta boloñesa" },
                    { new Guid("57494608-d0e8-48a5-ac5e-7c07afbb1831"), null, "Paella" },
                    { new Guid("58806a47-bd1b-4925-beec-658cff1a2b05"), null, "Tarta de manzana" },
                    { new Guid("5fdc549f-8311-4f3c-a4c6-9e047e5698ca"), null, "Papas fritas" },
                    { new Guid("679c82e9-f149-441a-8f6d-e790fb171323"), null, "Sándwich de jamón y queso" },
                    { new Guid("7102923e-ef0d-4a24-af3c-a18cc0b394eb"), null, "Huevos revueltos" },
                    { new Guid("758c61bf-cce5-402d-95b3-8c24926cdf7e"), null, "Ensalada César" },
                    { new Guid("7ec6923b-71db-42da-87f0-9916e491ef44"), null, "Empanadas" },
                    { new Guid("80426c4e-c519-4c8f-baf8-3ba207032d78"), null, "Milanesa con papas" },
                    { new Guid("9bc3b600-23a5-46e5-8b97-271960b5d18d"), null, "Smoothie de frutas" },
                    { new Guid("a7044fcb-2311-4cc8-a273-4fbdb112c626"), null, "Tacos" },
                    { new Guid("aae43d0c-5915-40d0-b407-690ee84a25e6"), null, "Hamburguesa" },
                    { new Guid("b5b74a56-4cd8-4bab-ac12-a1f15b62f8be"), null, "Ramen japonés" },
                    { new Guid("b96d06d9-7c3a-454c-a167-dab3f1e3ac13"), null, "Jugo natural" },
                    { new Guid("c5e5e944-810f-44ab-a665-5c5ea434a544"), null, "Kebab" },
                    { new Guid("d24d4abd-7090-46e2-8b2f-2e343c829113"), null, "Chocolate" },
                    { new Guid("d74ff1ba-437f-458d-94ba-2b28482ad6fd"), null, "Cerveza artesanal" },
                    { new Guid("e2c9ae68-0846-4211-8a19-7456dea0033f"), null, "Pollo grillado" },
                    { new Guid("f485cf12-4fd7-4cee-a58b-78d5e3fdf5bf"), null, "Pescado al horno" },
                    { new Guid("f6b2c395-c742-46a6-a586-821bbe22a0d8"), null, "Ceviche" }
                });

            migrationBuilder.InsertData(
                table: "Restricciones",
                columns: new[] { "Id", "Nombre" },
                values: new object[,]
                {
                    { new Guid("2ab8ac74-4cc3-476f-9e90-d7c63e012e08"), "Sin carne roja" },
                    { new Guid("5723b940-a18d-4f0f-97f6-480ea7d05b32"), "Sin soja" },
                    { new Guid("63a5147a-84a7-4193-9bcb-40ad10025b6c"), "Sin sal" },
                    { new Guid("731b89a2-8898-4557-94dc-b7ee5f1bcbec"), "Sin alcohol" },
                    { new Guid("88ad6a6d-2fce-4713-8e22-b56041c57be1"), "Sin gluten" },
                    { new Guid("903d1554-2bbc-4e1c-8ba3-dc88f2d60b82"), "Sin azúcar" },
                    { new Guid("9dc4cee3-db39-4af1-980d-dc6c2811b989"), "Sin mariscos" },
                    { new Guid("abed4fde-7bb6-420c-935f-bc018a81f6a4"), "Sin frito" },
                    { new Guid("cb0209bc-2675-4515-ad0b-5889a660de17"), "Sin frutos secos" },
                    { new Guid("d2507a73-8d64-45bc-bed5-3e1981613b70"), "Sin lactosa" },
                    { new Guid("e1cd3b11-8559-4364-ab35-513c48a9196e"), "Sin picante" },
                    { new Guid("f24a155c-408f-470f-8064-a8043de937ed"), "Sin cafeína" }
                });

            migrationBuilder.InsertData(
                table: "Tags",
                columns: new[] { "Id", "Nombre", "Tipo" },
                values: new object[,]
                {
                    { new Guid("11637566-47f1-4d18-9bd7-573fdda77e13"), "Carne roja", "Ingrediente" },
                    { new Guid("31e31211-5d09-4782-9b5e-c523e7207bfb"), "Vegetal", "Categoria" },
                    { new Guid("35f41fd6-267d-4f75-8268-e9f21424796b"), "Cafeína", "Nutriente" },
                    { new Guid("3d21eb50-be8b-4d1b-a88b-65bb2c564499"), "Soja", "Ingrediente" },
                    { new Guid("3eb25592-620d-4a4c-842b-7b3067bf2275"), "Pescado", "Ingrediente" },
                    { new Guid("4b610fb2-f412-4e94-b145-635e2d942b65"), "Frito", "Categoria" },
                    { new Guid("57fc04c5-81f4-44c8-8f55-5a833b1e2a3b"), "Azúcar", "Nutriente" },
                    { new Guid("93696e0e-6226-47df-9af1-4dee58470fda"), "Gluten", "Ingrediente" },
                    { new Guid("b1ccc4a5-d618-4032-8855-9fdcd4444d94"), "Grasa", "Nutriente" },
                    { new Guid("b6027ef8-788c-47e1-85e1-85cebd901747"), "Alcohol", "Nutriente" },
                    { new Guid("b78455c1-23de-4429-b7e3-f0e902fc735c"), "Lácteo", "Ingrediente" },
                    { new Guid("b9e5fd0f-e917-456e-b7fd-4ee80db5998f"), "Carne blanca", "Ingrediente" },
                    { new Guid("bbe9d4dd-2890-4725-860e-2affab9eef19"), "Fruta", "Categoria" },
                    { new Guid("c1f31540-1462-422c-b4d1-15a8ac24b6d7"), "Procesado", "Categoria" },
                    { new Guid("c45e4daa-4a52-439c-aff7-8075f993cef4"), "Mariscos", "Ingrediente" },
                    { new Guid("c8f41e3d-a411-4b6c-ad16-a93f9c6847a7"), "Harina", "Ingrediente" },
                    { new Guid("d25dee01-f656-4e95-901c-adcf5afb9854"), "Huevos", "Ingrediente" },
                    { new Guid("e4de5bca-1945-42ea-8f4f-737665b37adf"), "Picante", "Categoria" },
                    { new Guid("fb386e3b-675c-4086-9fd4-d431c754da96"), "Sal", "Nutriente" },
                    { new Guid("fcebd133-af80-4b53-9af3-1e1013a187df"), "Frutos secos", "Ingrediente" }
                });

            migrationBuilder.InsertData(
                table: "CondicionMedicaTags",
                columns: new[] { "CondicionesMedicasId", "TagsId" },
                values: new object[,]
                {
                    { new Guid("0a1b5b30-3940-46f6-8b06-d094f5da9a18"), new Guid("93696e0e-6226-47df-9af1-4dee58470fda") },
                    { new Guid("0fba5975-a2d3-40e7-aad5-f68f4697812a"), new Guid("b1ccc4a5-d618-4032-8855-9fdcd4444d94") },
                    { new Guid("1f192bb2-99ac-4725-9f90-0a83fecf0802"), new Guid("d25dee01-f656-4e95-901c-adcf5afb9854") },
                    { new Guid("7cb101c0-3467-4671-9673-ca4f485cd5cf"), new Guid("b78455c1-23de-4429-b7e3-f0e902fc735c") },
                    { new Guid("800c6326-7351-4132-8807-d6313cb586c4"), new Guid("e4de5bca-1945-42ea-8f4f-737665b37adf") },
                    { new Guid("94c58ba4-fedb-42bc-a121-9b6d65a1af59"), new Guid("c45e4daa-4a52-439c-aff7-8075f993cef4") },
                    { new Guid("a90feec1-4e14-4730-ab1a-c8b97e0515f4"), new Guid("fb386e3b-675c-4086-9fd4-d431c754da96") },
                    { new Guid("b9ce1123-4673-420d-bbcc-e979e9b5a3bf"), new Guid("11637566-47f1-4d18-9bd7-573fdda77e13") },
                    { new Guid("c1fbf544-c48b-4f2f-b4a8-476e6193240d"), new Guid("fcebd133-af80-4b53-9af3-1e1013a187df") },
                    { new Guid("d9a326ab-f139-4548-bc33-e7a2dfce5fe8"), new Guid("4b610fb2-f412-4e94-b145-635e2d942b65") },
                    { new Guid("df0fd0d9-ccbd-4ee0-85ef-cef99221394e"), new Guid("57fc04c5-81f4-44c8-8f55-5a833b1e2a3b") },
                    { new Guid("ea28f81d-8dfe-4558-93b6-19537a0d47af"), new Guid("35f41fd6-267d-4f75-8268-e9f21424796b") }
                });

            migrationBuilder.InsertData(
                table: "GustoTags",
                columns: new[] { "GustosId", "TagsId" },
                values: new object[,]
                {
                    { new Guid("03427ae8-a042-4868-9373-8ca80f97c9a4"), new Guid("11637566-47f1-4d18-9bd7-573fdda77e13") },
                    { new Guid("03427ae8-a042-4868-9373-8ca80f97c9a4"), new Guid("fb386e3b-675c-4086-9fd4-d431c754da96") },
                    { new Guid("0571ba30-490d-4ef4-ae9a-f935da97b3b0"), new Guid("b6027ef8-788c-47e1-85e1-85cebd901747") },
                    { new Guid("0832bd35-f5db-43c0-8b65-17f372cb2378"), new Guid("35f41fd6-267d-4f75-8268-e9f21424796b") },
                    { new Guid("0832bd35-f5db-43c0-8b65-17f372cb2378"), new Guid("b78455c1-23de-4429-b7e3-f0e902fc735c") },
                    { new Guid("0dee7103-d13b-47cf-886c-b3f0d4cb2e60"), new Guid("bbe9d4dd-2890-4725-860e-2affab9eef19") },
                    { new Guid("1c1c46d0-0d31-4107-b309-b4129f6c60d4"), new Guid("11637566-47f1-4d18-9bd7-573fdda77e13") },
                    { new Guid("1c1c46d0-0d31-4107-b309-b4129f6c60d4"), new Guid("b1ccc4a5-d618-4032-8855-9fdcd4444d94") },
                    { new Guid("1c1c46d0-0d31-4107-b309-b4129f6c60d4"), new Guid("fb386e3b-675c-4086-9fd4-d431c754da96") },
                    { new Guid("1d6363af-9a2b-4110-a58b-cfa444479ae3"), new Guid("31e31211-5d09-4782-9b5e-c523e7207bfb") },
                    { new Guid("1d6363af-9a2b-4110-a58b-cfa444479ae3"), new Guid("fb386e3b-675c-4086-9fd4-d431c754da96") },
                    { new Guid("1f31e2d9-0d7f-4a0e-818e-9f7951d67f79"), new Guid("57fc04c5-81f4-44c8-8f55-5a833b1e2a3b") },
                    { new Guid("1f31e2d9-0d7f-4a0e-818e-9f7951d67f79"), new Guid("b78455c1-23de-4429-b7e3-f0e902fc735c") },
                    { new Guid("254fe076-1791-43bb-940d-b72fc61f0aac"), new Guid("93696e0e-6226-47df-9af1-4dee58470fda") },
                    { new Guid("254fe076-1791-43bb-940d-b72fc61f0aac"), new Guid("b78455c1-23de-4429-b7e3-f0e902fc735c") },
                    { new Guid("254fe076-1791-43bb-940d-b72fc61f0aac"), new Guid("fb386e3b-675c-4086-9fd4-d431c754da96") },
                    { new Guid("3bd6bc23-b7ad-4f5e-bbbb-4dbcf28f4d97"), new Guid("3eb25592-620d-4a4c-842b-7b3067bf2275") },
                    { new Guid("3bd6bc23-b7ad-4f5e-bbbb-4dbcf28f4d97"), new Guid("fb386e3b-675c-4086-9fd4-d431c754da96") },
                    { new Guid("3f081178-d8e0-4399-8598-b02ff9a9c489"), new Guid("31e31211-5d09-4782-9b5e-c523e7207bfb") },
                    { new Guid("505fa8cf-d1bd-4799-804d-2735d691f0db"), new Guid("57fc04c5-81f4-44c8-8f55-5a833b1e2a3b") },
                    { new Guid("505fa8cf-d1bd-4799-804d-2735d691f0db"), new Guid("c8f41e3d-a411-4b6c-ad16-a93f9c6847a7") },
                    { new Guid("505fa8cf-d1bd-4799-804d-2735d691f0db"), new Guid("d25dee01-f656-4e95-901c-adcf5afb9854") },
                    { new Guid("56b6d595-a1b7-48fc-9ae0-4cf291e1eb21"), new Guid("11637566-47f1-4d18-9bd7-573fdda77e13") },
                    { new Guid("56b6d595-a1b7-48fc-9ae0-4cf291e1eb21"), new Guid("93696e0e-6226-47df-9af1-4dee58470fda") },
                    { new Guid("57494608-d0e8-48a5-ac5e-7c07afbb1831"), new Guid("c45e4daa-4a52-439c-aff7-8075f993cef4") },
                    { new Guid("57494608-d0e8-48a5-ac5e-7c07afbb1831"), new Guid("fb386e3b-675c-4086-9fd4-d431c754da96") },
                    { new Guid("58806a47-bd1b-4925-beec-658cff1a2b05"), new Guid("57fc04c5-81f4-44c8-8f55-5a833b1e2a3b") },
                    { new Guid("58806a47-bd1b-4925-beec-658cff1a2b05"), new Guid("bbe9d4dd-2890-4725-860e-2affab9eef19") },
                    { new Guid("58806a47-bd1b-4925-beec-658cff1a2b05"), new Guid("c8f41e3d-a411-4b6c-ad16-a93f9c6847a7") },
                    { new Guid("5fdc549f-8311-4f3c-a4c6-9e047e5698ca"), new Guid("4b610fb2-f412-4e94-b145-635e2d942b65") },
                    { new Guid("5fdc549f-8311-4f3c-a4c6-9e047e5698ca"), new Guid("fb386e3b-675c-4086-9fd4-d431c754da96") },
                    { new Guid("679c82e9-f149-441a-8f6d-e790fb171323"), new Guid("93696e0e-6226-47df-9af1-4dee58470fda") },
                    { new Guid("679c82e9-f149-441a-8f6d-e790fb171323"), new Guid("b78455c1-23de-4429-b7e3-f0e902fc735c") },
                    { new Guid("679c82e9-f149-441a-8f6d-e790fb171323"), new Guid("c1f31540-1462-422c-b4d1-15a8ac24b6d7") },
                    { new Guid("7102923e-ef0d-4a24-af3c-a18cc0b394eb"), new Guid("d25dee01-f656-4e95-901c-adcf5afb9854") },
                    { new Guid("758c61bf-cce5-402d-95b3-8c24926cdf7e"), new Guid("31e31211-5d09-4782-9b5e-c523e7207bfb") },
                    { new Guid("758c61bf-cce5-402d-95b3-8c24926cdf7e"), new Guid("b78455c1-23de-4429-b7e3-f0e902fc735c") },
                    { new Guid("758c61bf-cce5-402d-95b3-8c24926cdf7e"), new Guid("fb386e3b-675c-4086-9fd4-d431c754da96") },
                    { new Guid("7ec6923b-71db-42da-87f0-9916e491ef44"), new Guid("11637566-47f1-4d18-9bd7-573fdda77e13") },
                    { new Guid("7ec6923b-71db-42da-87f0-9916e491ef44"), new Guid("4b610fb2-f412-4e94-b145-635e2d942b65") },
                    { new Guid("7ec6923b-71db-42da-87f0-9916e491ef44"), new Guid("93696e0e-6226-47df-9af1-4dee58470fda") },
                    { new Guid("80426c4e-c519-4c8f-baf8-3ba207032d78"), new Guid("11637566-47f1-4d18-9bd7-573fdda77e13") },
                    { new Guid("80426c4e-c519-4c8f-baf8-3ba207032d78"), new Guid("4b610fb2-f412-4e94-b145-635e2d942b65") },
                    { new Guid("80426c4e-c519-4c8f-baf8-3ba207032d78"), new Guid("c8f41e3d-a411-4b6c-ad16-a93f9c6847a7") },
                    { new Guid("9bc3b600-23a5-46e5-8b97-271960b5d18d"), new Guid("57fc04c5-81f4-44c8-8f55-5a833b1e2a3b") },
                    { new Guid("9bc3b600-23a5-46e5-8b97-271960b5d18d"), new Guid("bbe9d4dd-2890-4725-860e-2affab9eef19") },
                    { new Guid("a7044fcb-2311-4cc8-a273-4fbdb112c626"), new Guid("11637566-47f1-4d18-9bd7-573fdda77e13") },
                    { new Guid("a7044fcb-2311-4cc8-a273-4fbdb112c626"), new Guid("e4de5bca-1945-42ea-8f4f-737665b37adf") },
                    { new Guid("aae43d0c-5915-40d0-b407-690ee84a25e6"), new Guid("11637566-47f1-4d18-9bd7-573fdda77e13") },
                    { new Guid("aae43d0c-5915-40d0-b407-690ee84a25e6"), new Guid("4b610fb2-f412-4e94-b145-635e2d942b65") },
                    { new Guid("aae43d0c-5915-40d0-b407-690ee84a25e6"), new Guid("93696e0e-6226-47df-9af1-4dee58470fda") },
                    { new Guid("b5b74a56-4cd8-4bab-ac12-a1f15b62f8be"), new Guid("93696e0e-6226-47df-9af1-4dee58470fda") },
                    { new Guid("b5b74a56-4cd8-4bab-ac12-a1f15b62f8be"), new Guid("e4de5bca-1945-42ea-8f4f-737665b37adf") },
                    { new Guid("b5b74a56-4cd8-4bab-ac12-a1f15b62f8be"), new Guid("fb386e3b-675c-4086-9fd4-d431c754da96") },
                    { new Guid("b96d06d9-7c3a-454c-a167-dab3f1e3ac13"), new Guid("bbe9d4dd-2890-4725-860e-2affab9eef19") },
                    { new Guid("c5e5e944-810f-44ab-a665-5c5ea434a544"), new Guid("11637566-47f1-4d18-9bd7-573fdda77e13") },
                    { new Guid("c5e5e944-810f-44ab-a665-5c5ea434a544"), new Guid("b1ccc4a5-d618-4032-8855-9fdcd4444d94") },
                    { new Guid("c5e5e944-810f-44ab-a665-5c5ea434a544"), new Guid("e4de5bca-1945-42ea-8f4f-737665b37adf") },
                    { new Guid("d24d4abd-7090-46e2-8b2f-2e343c829113"), new Guid("57fc04c5-81f4-44c8-8f55-5a833b1e2a3b") },
                    { new Guid("d24d4abd-7090-46e2-8b2f-2e343c829113"), new Guid("b1ccc4a5-d618-4032-8855-9fdcd4444d94") },
                    { new Guid("d74ff1ba-437f-458d-94ba-2b28482ad6fd"), new Guid("b6027ef8-788c-47e1-85e1-85cebd901747") },
                    { new Guid("e2c9ae68-0846-4211-8a19-7456dea0033f"), new Guid("31e31211-5d09-4782-9b5e-c523e7207bfb") },
                    { new Guid("e2c9ae68-0846-4211-8a19-7456dea0033f"), new Guid("b9e5fd0f-e917-456e-b7fd-4ee80db5998f") },
                    { new Guid("f485cf12-4fd7-4cee-a58b-78d5e3fdf5bf"), new Guid("31e31211-5d09-4782-9b5e-c523e7207bfb") },
                    { new Guid("f485cf12-4fd7-4cee-a58b-78d5e3fdf5bf"), new Guid("3eb25592-620d-4a4c-842b-7b3067bf2275") },
                    { new Guid("f6b2c395-c742-46a6-a586-821bbe22a0d8"), new Guid("31e31211-5d09-4782-9b5e-c523e7207bfb") },
                    { new Guid("f6b2c395-c742-46a6-a586-821bbe22a0d8"), new Guid("3eb25592-620d-4a4c-842b-7b3067bf2275") },
                    { new Guid("f6b2c395-c742-46a6-a586-821bbe22a0d8"), new Guid("c45e4daa-4a52-439c-aff7-8075f993cef4") }
                });

            migrationBuilder.InsertData(
                table: "RestriccionTags",
                columns: new[] { "RestriccionesId", "TagsId" },
                values: new object[,]
                {
                    { new Guid("2ab8ac74-4cc3-476f-9e90-d7c63e012e08"), new Guid("11637566-47f1-4d18-9bd7-573fdda77e13") },
                    { new Guid("5723b940-a18d-4f0f-97f6-480ea7d05b32"), new Guid("3d21eb50-be8b-4d1b-a88b-65bb2c564499") },
                    { new Guid("63a5147a-84a7-4193-9bcb-40ad10025b6c"), new Guid("fb386e3b-675c-4086-9fd4-d431c754da96") },
                    { new Guid("731b89a2-8898-4557-94dc-b7ee5f1bcbec"), new Guid("b6027ef8-788c-47e1-85e1-85cebd901747") },
                    { new Guid("88ad6a6d-2fce-4713-8e22-b56041c57be1"), new Guid("93696e0e-6226-47df-9af1-4dee58470fda") },
                    { new Guid("903d1554-2bbc-4e1c-8ba3-dc88f2d60b82"), new Guid("57fc04c5-81f4-44c8-8f55-5a833b1e2a3b") },
                    { new Guid("9dc4cee3-db39-4af1-980d-dc6c2811b989"), new Guid("c45e4daa-4a52-439c-aff7-8075f993cef4") },
                    { new Guid("abed4fde-7bb6-420c-935f-bc018a81f6a4"), new Guid("4b610fb2-f412-4e94-b145-635e2d942b65") },
                    { new Guid("cb0209bc-2675-4515-ad0b-5889a660de17"), new Guid("fcebd133-af80-4b53-9af3-1e1013a187df") },
                    { new Guid("d2507a73-8d64-45bc-bed5-3e1981613b70"), new Guid("b78455c1-23de-4429-b7e3-f0e902fc735c") },
                    { new Guid("e1cd3b11-8559-4364-ab35-513c48a9196e"), new Guid("e4de5bca-1945-42ea-8f4f-737665b37adf") },
                    { new Guid("f24a155c-408f-470f-8064-a8043de937ed"), new Guid("35f41fd6-267d-4f75-8268-e9f21424796b") }
                });
        }
    }
}
