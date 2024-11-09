using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    public partial class modificoDbContextAusenciasGenericas : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AusenciaId",
                table: "Personas",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Personas_AusenciaId",
                table: "Personas",
                column: "AusenciaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Personas_Ausencias_AusenciaId",
                table: "Personas",
                column: "AusenciaId",
                principalTable: "Ausencias",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Personas_Ausencias_AusenciaId",
                table: "Personas");

            migrationBuilder.DropIndex(
                name: "IX_Personas_AusenciaId",
                table: "Personas");

            migrationBuilder.DropColumn(
                name: "AusenciaId",
                table: "Personas");
        }
    }
}
