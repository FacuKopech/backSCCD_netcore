using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    public partial class agregoIntermediateTableAsistenciaAlumno : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlumnoAsistencia");

            migrationBuilder.CreateTable(
                name: "AsistenciaAlumno",
                columns: table => new
                {
                    AsistenciaId = table.Column<int>(type: "int", nullable: false),
                    AlumnoId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AsistenciaAlumno", x => new { x.AsistenciaId, x.AlumnoId });
                    table.ForeignKey(
                        name: "FK_AsistenciaAlumno_AsistenciasTomadas_AsistenciaId",
                        column: x => x.AsistenciaId,
                        principalTable: "AsistenciasTomadas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AsistenciaAlumno_Personas_AlumnoId",
                        column: x => x.AlumnoId,
                        principalTable: "Personas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AsistenciaAlumno_AlumnoId",
                table: "AsistenciaAlumno",
                column: "AlumnoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AsistenciaAlumno");

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
    }
}
