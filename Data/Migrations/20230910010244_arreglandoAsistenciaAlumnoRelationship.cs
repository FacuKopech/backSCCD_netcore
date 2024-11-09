using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    public partial class arreglandoAsistenciaAlumnoRelationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlumnoAsistenciaAusentes");

            migrationBuilder.DropTable(
                name: "AlumnoAsistenciaPresentes");

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

            migrationBuilder.CreateTable(
                name: "AlumnoAsistenciaAusentes",
                columns: table => new
                {
                    AsistenciasId = table.Column<int>(type: "int", nullable: false),
                    AusentesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlumnoAsistenciaAusentes", x => new { x.AsistenciasId, x.AusentesId });
                    table.ForeignKey(
                        name: "FK_AlumnoAsistenciaAusentes_AsistenciasTomadas_AsistenciasId",
                        column: x => x.AsistenciasId,
                        principalTable: "AsistenciasTomadas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AlumnoAsistenciaAusentes_Personas_AusentesId",
                        column: x => x.AusentesId,
                        principalTable: "Personas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AlumnoAsistenciaPresentes",
                columns: table => new
                {
                    AsistenciasId = table.Column<int>(type: "int", nullable: false),
                    PresentesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlumnoAsistenciaPresentes", x => new { x.AsistenciasId, x.PresentesId });
                    table.ForeignKey(
                        name: "FK_AlumnoAsistenciaPresentes_AsistenciasTomadas_AsistenciasId",
                        column: x => x.AsistenciasId,
                        principalTable: "AsistenciasTomadas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AlumnoAsistenciaPresentes_Personas_PresentesId",
                        column: x => x.PresentesId,
                        principalTable: "Personas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AlumnoAsistenciaAusentes_AusentesId",
                table: "AlumnoAsistenciaAusentes",
                column: "AusentesId");

            migrationBuilder.CreateIndex(
                name: "IX_AlumnoAsistenciaPresentes_PresentesId",
                table: "AlumnoAsistenciaPresentes",
                column: "PresentesId");
        }
    }
}
