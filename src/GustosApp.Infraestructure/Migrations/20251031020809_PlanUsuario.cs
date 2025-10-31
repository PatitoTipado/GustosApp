using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GustosApp.Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class PlanUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF COL_LENGTH('dbo.Usuarios', 'Plan') IS NULL
BEGIN
    ALTER TABLE [dbo].[Usuarios] ADD [Plan] int NOT NULL DEFAULT 0;
END
");

            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[Notificaciones]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Notificaciones] (
        [Id] uniqueidentifier NOT NULL,
        [UsuarioDestinoId] uniqueidentifier NOT NULL,
        [Titulo] nvarchar(200) NOT NULL,
        [Mensaje] nvarchar(500) NOT NULL,
        [Tipo] int NOT NULL,
        [Leida] bit NOT NULL,
        [FechaCreacion] datetime2 NOT NULL,
        CONSTRAINT [PK_Notificaciones] PRIMARY KEY ([Id])
    );
END
");

            migrationBuilder.Sql(@"
IF NOT EXISTS (
    SELECT name
    FROM sys.indexes
    WHERE name = N'IX_Notificaciones_UsuarioDestinoId'
      AND object_id = OBJECT_ID(N'[dbo].[Notificaciones]')
)
BEGIN
    CREATE INDEX [IX_Notificaciones_UsuarioDestinoId]
        ON [dbo].[Notificaciones]([UsuarioDestinoId]);
END
");

            migrationBuilder.Sql(@"
IF NOT EXISTS (
    SELECT 1
    FROM sys.foreign_keys
    WHERE name = N'FK_Notificaciones_Usuarios_UsuarioDestinoId'
      AND parent_object_id = OBJECT_ID(N'[dbo].[Notificaciones]')
)
BEGIN
    ALTER TABLE [dbo].[Notificaciones] WITH CHECK
    ADD CONSTRAINT [FK_Notificaciones_Usuarios_UsuarioDestinoId]
        FOREIGN KEY ([UsuarioDestinoId]) REFERENCES [dbo].[Usuarios]([Id])
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
    WHERE name = N'FK_Notificaciones_Usuarios_UsuarioDestinoId'
      AND parent_object_id = OBJECT_ID(N'[dbo].[Notificaciones]')
)
BEGIN
    ALTER TABLE [dbo].[Notificaciones]
    DROP CONSTRAINT [FK_Notificaciones_Usuarios_UsuarioDestinoId];
END
");

            migrationBuilder.Sql(@"
IF EXISTS (
    SELECT name
    FROM sys.indexes
    WHERE name = N'IX_Notificaciones_UsuarioDestinoId'
      AND object_id = OBJECT_ID(N'[dbo].[Notificaciones]')
)
BEGIN
    DROP INDEX [IX_Notificaciones_UsuarioDestinoId] ON [dbo].[Notificaciones];
END
");

            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[Notificaciones]', N'U') IS NOT NULL
BEGIN
    DROP TABLE [dbo].[Notificaciones];
END
");

            migrationBuilder.Sql(@"
IF COL_LENGTH('dbo.Usuarios', 'Plan') IS NOT NULL
BEGIN
    ALTER TABLE [dbo].[Usuarios] DROP COLUMN [Plan];
END
");
        }
    }
}

