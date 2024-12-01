using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    public partial class modifiedEventoAndPersonaModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventoPersona_Eventos_EventosAsistireId",
                table: "EventoPersona");

            migrationBuilder.DropForeignKey(
                name: "FK_EventoPersona_Personas_AsistiranId",
                table: "EventoPersona");

            migrationBuilder.DropTable(
                name: "EventoPersona1");

            migrationBuilder.DropTable(
                name: "EventoPersona2");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EventoPersona",
                table: "EventoPersona");

            migrationBuilder.RenameColumn(
                name: "EventosAsistireId",
                table: "EventoPersona",
                newName: "PersonaId");

            migrationBuilder.RenameColumn(
                name: "AsistiranId",
                table: "EventoPersona",
                newName: "EventoId");

            migrationBuilder.RenameIndex(
                name: "IX_EventoPersona_EventosAsistireId",
                table: "EventoPersona",
                newName: "IX_EventoPersona_PersonaId");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "EventoPersona",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "NEWID()");

            migrationBuilder.AddColumn<bool>(
                name: "Asistira",
                table: "EventoPersona",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaConfirmacion",
                table: "EventoPersona",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "NoAsistira",
                table: "EventoPersona",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "TalVezAsista",
                table: "EventoPersona",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventoPersona",
                table: "EventoPersona",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_EventoPersona_EventoId",
                table: "EventoPersona",
                column: "EventoId");

            migrationBuilder.AddForeignKey(
                name: "FK_EventoPersona_Eventos_EventoId",
                table: "EventoPersona",
                column: "EventoId",
                principalTable: "Eventos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EventoPersona_Personas_PersonaId",
                table: "EventoPersona",
                column: "PersonaId",
                principalTable: "Personas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventoPersona_Eventos_EventoId",
                table: "EventoPersona");

            migrationBuilder.DropForeignKey(
                name: "FK_EventoPersona_Personas_PersonaId",
                table: "EventoPersona");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EventoPersona",
                table: "EventoPersona");

            migrationBuilder.DropIndex(
                name: "IX_EventoPersona_EventoId",
                table: "EventoPersona");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "EventoPersona");

            migrationBuilder.DropColumn(
                name: "Asistira",
                table: "EventoPersona");

            migrationBuilder.DropColumn(
                name: "FechaConfirmacion",
                table: "EventoPersona");

            migrationBuilder.DropColumn(
                name: "NoAsistira",
                table: "EventoPersona");

            migrationBuilder.DropColumn(
                name: "TalVezAsista",
                table: "EventoPersona");

            migrationBuilder.RenameColumn(
                name: "PersonaId",
                table: "EventoPersona",
                newName: "EventosAsistireId");

            migrationBuilder.RenameColumn(
                name: "EventoId",
                table: "EventoPersona",
                newName: "AsistiranId");

            migrationBuilder.RenameIndex(
                name: "IX_EventoPersona_PersonaId",
                table: "EventoPersona",
                newName: "IX_EventoPersona_EventosAsistireId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventoPersona",
                table: "EventoPersona",
                columns: new[] { "AsistiranId", "EventosAsistireId" });

            migrationBuilder.CreateTable(
                name: "EventoPersona1",
                columns: table => new
                {
                    EventosNoAsistireId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NoAsistiranId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventoPersona1", x => new { x.EventosNoAsistireId, x.NoAsistiranId });
                    table.ForeignKey(
                        name: "FK_EventoPersona1_Eventos_EventosNoAsistireId",
                        column: x => x.EventosNoAsistireId,
                        principalTable: "Eventos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventoPersona1_Personas_NoAsistiranId",
                        column: x => x.NoAsistiranId,
                        principalTable: "Personas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventoPersona2",
                columns: table => new
                {
                    EventosTalVezAsistaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TalVezAsistanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventoPersona2", x => new { x.EventosTalVezAsistaId, x.TalVezAsistanId });
                    table.ForeignKey(
                        name: "FK_EventoPersona2_Eventos_EventosTalVezAsistaId",
                        column: x => x.EventosTalVezAsistaId,
                        principalTable: "Eventos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventoPersona2_Personas_TalVezAsistanId",
                        column: x => x.TalVezAsistanId,
                        principalTable: "Personas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventoPersona1_NoAsistiranId",
                table: "EventoPersona1",
                column: "NoAsistiranId");

            migrationBuilder.CreateIndex(
                name: "IX_EventoPersona2_TalVezAsistanId",
                table: "EventoPersona2",
                column: "TalVezAsistanId");

            migrationBuilder.AddForeignKey(
                name: "FK_EventoPersona_Eventos_EventosAsistireId",
                table: "EventoPersona",
                column: "EventosAsistireId",
                principalTable: "Eventos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EventoPersona_Personas_AsistiranId",
                table: "EventoPersona",
                column: "AsistiranId",
                principalTable: "Personas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
