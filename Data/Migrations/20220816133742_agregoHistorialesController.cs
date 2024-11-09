using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    public partial class agregoHistorialesController : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Historial_Personas_AlumnoId",
                table: "Historial");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Historial",
                table: "Historial");

            migrationBuilder.RenameTable(
                name: "Historial",
                newName: "Historiales");

            migrationBuilder.RenameIndex(
                name: "IX_Historial_AlumnoId",
                table: "Historiales",
                newName: "IX_Historiales_AlumnoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Historiales",
                table: "Historiales",
                column: "IdHistorial");

            migrationBuilder.AddForeignKey(
                name: "FK_Historiales_Personas_AlumnoId",
                table: "Historiales",
                column: "AlumnoId",
                principalTable: "Personas",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Historiales_Personas_AlumnoId",
                table: "Historiales");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Historiales",
                table: "Historiales");

            migrationBuilder.RenameTable(
                name: "Historiales",
                newName: "Historial");

            migrationBuilder.RenameIndex(
                name: "IX_Historiales_AlumnoId",
                table: "Historial",
                newName: "IX_Historial_AlumnoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Historial",
                table: "Historial",
                column: "IdHistorial");

            migrationBuilder.AddForeignKey(
                name: "FK_Historial_Personas_AlumnoId",
                table: "Historial",
                column: "AlumnoId",
                principalTable: "Personas",
                principalColumn: "Id");
        }
    }
}
