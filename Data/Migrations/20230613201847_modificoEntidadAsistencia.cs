using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    public partial class modificoEntidadAsistencia : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CantidadAusentes",
                table: "AsistenciasTomadas");

            migrationBuilder.DropColumn(
                name: "CantidadPresentes",
                table: "AsistenciasTomadas");

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

        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<int>(
                name: "CantidadAusentes",
                table: "AsistenciasTomadas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CantidadPresentes",
                table: "AsistenciasTomadas",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
