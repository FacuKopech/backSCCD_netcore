using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    public partial class modifiedNotaAndPersonaModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NotaPersona_Notas_NotasRecibidasId",
                table: "NotaPersona");

            migrationBuilder.DropForeignKey(
                name: "FK_NotaPersona_Personas_DestinatariosId",
                table: "NotaPersona");

            migrationBuilder.DropTable(
                name: "NotaPersona1");

            migrationBuilder.DropTable(
                name: "NotaPersona2");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NotaPersona",
                table: "NotaPersona");

            migrationBuilder.DropColumn(
                name: "Leida",
                table: "Notas");

            migrationBuilder.RenameColumn(
                name: "NotasRecibidasId",
                table: "NotaPersona",
                newName: "PersonaId");

            migrationBuilder.RenameColumn(
                name: "DestinatariosId",
                table: "NotaPersona",
                newName: "NotaId");

            migrationBuilder.RenameIndex(
                name: "IX_NotaPersona_NotasRecibidasId",
                table: "NotaPersona",
                newName: "IX_NotaPersona_PersonaId");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "NotaPersona",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaFirma",
                table: "NotaPersona",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaLectura",
                table: "NotaPersona",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Firmada",
                table: "NotaPersona",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Leida",
                table: "NotaPersona",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_NotaPersona",
                table: "NotaPersona",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_NotaPersona_NotaId",
                table: "NotaPersona",
                column: "NotaId");

            migrationBuilder.AddForeignKey(
                name: "FK_NotaPersona_Notas_NotaId",
                table: "NotaPersona",
                column: "NotaId",
                principalTable: "Notas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NotaPersona_Personas_PersonaId",
                table: "NotaPersona",
                column: "PersonaId",
                principalTable: "Personas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NotaPersona_Notas_NotaId",
                table: "NotaPersona");

            migrationBuilder.DropForeignKey(
                name: "FK_NotaPersona_Personas_PersonaId",
                table: "NotaPersona");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NotaPersona",
                table: "NotaPersona");

            migrationBuilder.DropIndex(
                name: "IX_NotaPersona_NotaId",
                table: "NotaPersona");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "NotaPersona");

            migrationBuilder.DropColumn(
                name: "FechaFirma",
                table: "NotaPersona");

            migrationBuilder.DropColumn(
                name: "FechaLectura",
                table: "NotaPersona");

            migrationBuilder.DropColumn(
                name: "Firmada",
                table: "NotaPersona");

            migrationBuilder.DropColumn(
                name: "Leida",
                table: "NotaPersona");

            migrationBuilder.RenameColumn(
                name: "PersonaId",
                table: "NotaPersona",
                newName: "NotasRecibidasId");

            migrationBuilder.RenameColumn(
                name: "NotaId",
                table: "NotaPersona",
                newName: "DestinatariosId");

            migrationBuilder.RenameIndex(
                name: "IX_NotaPersona_PersonaId",
                table: "NotaPersona",
                newName: "IX_NotaPersona_NotasRecibidasId");

            migrationBuilder.AddColumn<bool>(
                name: "Leida",
                table: "Notas",
                type: "bit",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_NotaPersona",
                table: "NotaPersona",
                columns: new[] { "DestinatariosId", "NotasRecibidasId" });

            migrationBuilder.CreateTable(
                name: "NotaPersona1",
                columns: table => new
                {
                    LeidaPorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NotasLeidasId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotaPersona1", x => new { x.LeidaPorId, x.NotasLeidasId });
                    table.ForeignKey(
                        name: "FK_NotaPersona1_Notas_NotasLeidasId",
                        column: x => x.NotasLeidasId,
                        principalTable: "Notas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NotaPersona1_Personas_LeidaPorId",
                        column: x => x.LeidaPorId,
                        principalTable: "Personas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NotaPersona2",
                columns: table => new
                {
                    FirmadaPorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NotasFirmadasId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotaPersona2", x => new { x.FirmadaPorId, x.NotasFirmadasId });
                    table.ForeignKey(
                        name: "FK_NotaPersona2_Notas_NotasFirmadasId",
                        column: x => x.NotasFirmadasId,
                        principalTable: "Notas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NotaPersona2_Personas_FirmadaPorId",
                        column: x => x.FirmadaPorId,
                        principalTable: "Personas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NotaPersona1_NotasLeidasId",
                table: "NotaPersona1",
                column: "NotasLeidasId");

            migrationBuilder.CreateIndex(
                name: "IX_NotaPersona2_NotasFirmadasId",
                table: "NotaPersona2",
                column: "NotasFirmadasId");

            migrationBuilder.AddForeignKey(
                name: "FK_NotaPersona_Notas_NotasRecibidasId",
                table: "NotaPersona",
                column: "NotasRecibidasId",
                principalTable: "Notas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NotaPersona_Personas_DestinatariosId",
                table: "NotaPersona",
                column: "DestinatariosId",
                principalTable: "Personas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
