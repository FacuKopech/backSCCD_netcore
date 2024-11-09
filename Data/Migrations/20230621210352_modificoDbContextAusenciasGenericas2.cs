using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    public partial class modificoDbContextAusenciasGenericas2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ausencias_Personas_AlumnoId",
                table: "Ausencias");

            migrationBuilder.DropForeignKey(
                name: "FK_Personas_Ausencias_AusenciaId",
                table: "Personas");

            migrationBuilder.DropIndex(
                name: "IX_Personas_AusenciaId",
                table: "Personas");

            migrationBuilder.DropIndex(
                name: "IX_Ausencias_AlumnoId",
                table: "Ausencias");

            migrationBuilder.DropColumn(
                name: "AusenciaId",
                table: "Personas");

            migrationBuilder.DropColumn(
                name: "AlumnoId",
                table: "Ausencias");

            migrationBuilder.CreateTable(
                name: "AlumnoAusencia",
                columns: table => new
                {
                    AusenciasId = table.Column<int>(type: "int", nullable: false),
                    HijosConAusenciaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlumnoAusencia", x => new { x.AusenciasId, x.HijosConAusenciaId });
                    table.ForeignKey(
                        name: "FK_AlumnoAusencia_Ausencias_AusenciasId",
                        column: x => x.AusenciasId,
                        principalTable: "Ausencias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AlumnoAusencia_Personas_HijosConAusenciaId",
                        column: x => x.HijosConAusenciaId,
                        principalTable: "Personas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AlumnoAusencia_HijosConAusenciaId",
                table: "AlumnoAusencia",
                column: "HijosConAusenciaId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlumnoAusencia");

            migrationBuilder.AddColumn<int>(
                name: "AusenciaId",
                table: "Personas",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AlumnoId",
                table: "Ausencias",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Personas_AusenciaId",
                table: "Personas",
                column: "AusenciaId");

            migrationBuilder.CreateIndex(
                name: "IX_Ausencias_AlumnoId",
                table: "Ausencias",
                column: "AlumnoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ausencias_Personas_AlumnoId",
                table: "Ausencias",
                column: "AlumnoId",
                principalTable: "Personas",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Personas_Ausencias_AusenciaId",
                table: "Personas",
                column: "AusenciaId",
                principalTable: "Ausencias",
                principalColumn: "Id");
        }
    }
}
