using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GustosApp.Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class CambiosEnClases : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF NOT EXISTS (
    SELECT name
    FROM sys.indexes
    WHERE name = N'IX_ChatMessages_GrupoId'
      AND object_id = OBJECT_ID(N'[dbo].[ChatMessages]')
)
BEGIN
    CREATE INDEX [IX_ChatMessages_GrupoId] ON [dbo].[ChatMessages]([GrupoId]);
END
");

            migrationBuilder.Sql(@"
IF NOT EXISTS (
    SELECT name
    FROM sys.indexes
    WHERE name = N'IX_ChatMessages_UsuarioId'
      AND object_id = OBJECT_ID(N'[dbo].[ChatMessages]')
)
BEGIN
    CREATE INDEX [IX_ChatMessages_UsuarioId] ON [dbo].[ChatMessages]([UsuarioId]);
END
");

            migrationBuilder.Sql(@"
IF NOT EXISTS (
    SELECT 1
    FROM sys.foreign_keys
    WHERE name = N'FK_ChatMessages_Grupos_GrupoId'
      AND parent_object_id = OBJECT_ID(N'[dbo].[ChatMessages]')
)
BEGIN
    ALTER TABLE [dbo].[ChatMessages] WITH CHECK
    ADD CONSTRAINT [FK_ChatMessages_Grupos_GrupoId]
        FOREIGN KEY ([GrupoId]) REFERENCES [dbo].[Grupos] ([Id])
        ON DELETE CASCADE;
END
");

            migrationBuilder.Sql(@"
IF NOT EXISTS (
    SELECT 1
    FROM sys.foreign_keys
    WHERE name = N'FK_ChatMessages_Usuarios_UsuarioId'
      AND parent_object_id = OBJECT_ID(N'[dbo].[ChatMessages]')
)
BEGIN
    ALTER TABLE [dbo].[ChatMessages] WITH CHECK
    ADD CONSTRAINT [FK_ChatMessages_Usuarios_UsuarioId]
        FOREIGN KEY ([UsuarioId]) REFERENCES [dbo].[Usuarios] ([Id])
        ON DELETE CASCADE;
END
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF EXISTS (
    SELECT 1
    FROM sys.foreign_keys
    WHERE name = N'FK_ChatMessages_Grupos_GrupoId'
      AND parent_object_id = OBJECT_ID(N'[dbo].[ChatMessages]')
)
BEGIN
    ALTER TABLE [dbo].[ChatMessages]
    DROP CONSTRAINT [FK_ChatMessages_Grupos_GrupoId];
END
");

            migrationBuilder.Sql(@"
IF EXISTS (
    SELECT 1
    FROM sys.foreign_keys
    WHERE name = N'FK_ChatMessages_Usuarios_UsuarioId'
      AND parent_object_id = OBJECT_ID(N'[dbo].[ChatMessages]')
)
BEGIN
    ALTER TABLE [dbo].[ChatMessages]
    DROP CONSTRAINT [FK_ChatMessages_Usuarios_UsuarioId];
END
");

            migrationBuilder.Sql(@"
IF EXISTS (
    SELECT name
    FROM sys.indexes
    WHERE name = N'IX_ChatMessages_GrupoId'
      AND object_id = OBJECT_ID(N'[dbo].[ChatMessages]')
)
BEGIN
    DROP INDEX [IX_ChatMessages_GrupoId] ON [dbo].[ChatMessages];
END
");

            migrationBuilder.Sql(@"
IF EXISTS (
    SELECT name
    FROM sys.indexes
    WHERE name = N'IX_ChatMessages_UsuarioId'
      AND object_id = OBJECT_ID(N'[dbo].[ChatMessages]')
)
BEGIN
    DROP INDEX [IX_ChatMessages_UsuarioId] ON [dbo].[ChatMessages];
END
");
        }
    }
}


