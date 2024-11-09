using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    public partial class modificoDbContextAsistenciaAlumnoRelationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Personas_AsistenciasTomadas_AsistenciaId",
                table: "Personas");

            migrationBuilder.DropForeignKey(
                name: "FK_Personas_AsistenciasTomadas_AsistenciaId1",
                table: "Personas");

            migrationBuilder.DropIndex(
                name: "IX_Personas_AsistenciaId",
                table: "Personas");

            migrationBuilder.DropIndex(
                name: "IX_Personas_AsistenciaId1",
                table: "Personas");

            migrationBuilder.DropColumn(
                name: "AsistenciaId",
                table: "Personas");

            migrationBuilder.DropColumn(
                name: "AsistenciaId1",
                table: "Personas");

            migrationBuilder.CreateTable(
                name: "AlumnoAsistencia",
                columns: table => new
                {
                    AsistenciasId = table.Column<int>(type: "int", nullable: false),
                    PresentesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlumnoAsistencia", x => new { x.AsistenciasId, x.PresentesId });
                    table.ForeignKey(
                        name: "FK_AlumnoAsistencia_AsistenciasTomadas_AsistenciasId",
                        column: x => x.AsistenciasId,
                        principalTable: "AsistenciasTomadas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AlumnoAsistencia_Personas_PresentesId",
                        column: x => x.PresentesId,
                        principalTable: "Personas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AlumnoAsistencia_PresentesId",
                table: "AlumnoAsistencia",
                column: "PresentesId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlumnoAsistencia");

            migrationBuilder.AddColumn<int>(
                name: "AsistenciaId",
                table: "Personas",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AsistenciaId1",
                table: "Personas",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Personas_AsistenciaId",
                table: "Personas",
                column: "AsistenciaId");

            migrationBuilder.CreateIndex(
                name: "IX_Personas_AsistenciaId1",
                table: "Personas",
                column: "AsistenciaId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Personas_AsistenciasTomadas_AsistenciaId",
                table: "Personas",
                column: "AsistenciaId",
                principalTable: "AsistenciasTomadas",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Personas_AsistenciasTomadas_AsistenciaId1",
                table: "Personas",
                column: "AsistenciaId1",
                principalTable: "AsistenciasTomadas",
                principalColumn: "Id");
        }
    }
}
