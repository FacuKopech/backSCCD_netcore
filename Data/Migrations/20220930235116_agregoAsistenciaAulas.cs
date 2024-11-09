using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    public partial class agregoAsistenciaAulas : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AsistenciasTomadas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FechaAsistenciaTomada = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CantidadPresentes = table.Column<int>(type: "int", nullable: false),
                    CantidadAusentes = table.Column<int>(type: "int", nullable: false),
                    AulaId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AsistenciasTomadas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AsistenciasTomadas_Aulas_AulaId",
                        column: x => x.AulaId,
                        principalTable: "Aulas",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AsistenciasTomadas_AulaId",
                table: "AsistenciasTomadas",
                column: "AulaId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AsistenciasTomadas");
        }
    }
}
