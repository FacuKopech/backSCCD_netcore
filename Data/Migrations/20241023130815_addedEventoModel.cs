using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    public partial class addedEventoModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Eventos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Localidad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Motivo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Eventos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EventoPersona",
                columns: table => new
                {
                    AsistiranId = table.Column<int>(type: "int", nullable: false),
                    EventosAsistireId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventoPersona", x => new { x.AsistiranId, x.EventosAsistireId });
                    table.ForeignKey(
                        name: "FK_EventoPersona_Eventos_EventosAsistireId",
                        column: x => x.EventosAsistireId,
                        principalTable: "Eventos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventoPersona_Personas_AsistiranId",
                        column: x => x.AsistiranId,
                        principalTable: "Personas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventoPersona1",
                columns: table => new
                {
                    EventosNoAsistireId = table.Column<int>(type: "int", nullable: false),
                    NoAsistiranId = table.Column<int>(type: "int", nullable: false)
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
                    EventosTalVezAsistaId = table.Column<int>(type: "int", nullable: false),
                    TalVezAsistanId = table.Column<int>(type: "int", nullable: false)
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
                name: "IX_EventoPersona_EventosAsistireId",
                table: "EventoPersona",
                column: "EventosAsistireId");

            migrationBuilder.CreateIndex(
                name: "IX_EventoPersona1_NoAsistiranId",
                table: "EventoPersona1",
                column: "NoAsistiranId");

            migrationBuilder.CreateIndex(
                name: "IX_EventoPersona2_TalVezAsistanId",
                table: "EventoPersona2",
                column: "TalVezAsistanId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventoPersona");

            migrationBuilder.DropTable(
                name: "EventoPersona1");

            migrationBuilder.DropTable(
                name: "EventoPersona2");

            migrationBuilder.DropTable(
                name: "Eventos");
        }
    }
}
