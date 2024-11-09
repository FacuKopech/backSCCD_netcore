using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    public partial class modifiedAlumnoAndAulaEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AñoActual",
                table: "Personas");

            migrationBuilder.DropColumn(
                name: "Division",
                table: "Personas");

            migrationBuilder.DropColumn(
                name: "Grado",
                table: "Personas");

            migrationBuilder.AddColumn<string>(
                name: "Division",
                table: "Aulas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Grado",
                table: "Aulas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Division",
                table: "Aulas");

            migrationBuilder.DropColumn(
                name: "Grado",
                table: "Aulas");

            migrationBuilder.AddColumn<int>(
                name: "AñoActual",
                table: "Personas",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Division",
                table: "Personas",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Grado",
                table: "Personas",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
