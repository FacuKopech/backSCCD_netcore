using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    public partial class modifiedEventoModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreadorId",
                table: "Eventos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Eventos_CreadorId",
                table: "Eventos",
                column: "CreadorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Eventos_Personas_CreadorId",
                table: "Eventos",
                column: "CreadorId",
                principalTable: "Personas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Eventos_Personas_CreadorId",
                table: "Eventos");

            migrationBuilder.DropIndex(
                name: "IX_Eventos_CreadorId",
                table: "Eventos");

            migrationBuilder.DropColumn(
                name: "CreadorId",
                table: "Eventos");
        }
    }
}
