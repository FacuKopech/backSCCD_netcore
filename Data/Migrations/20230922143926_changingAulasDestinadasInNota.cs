using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    public partial class changingAulasDestinadasInNota : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notas_Aulas_AulaDestinadaId",
                table: "Notas");

            migrationBuilder.DropIndex(
                name: "IX_Notas_AulaDestinadaId",
                table: "Notas");

            migrationBuilder.DropColumn(
                name: "AulaDestinadaId",
                table: "Notas");

            migrationBuilder.AddColumn<int>(
                name: "NotaId",
                table: "Aulas",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Aulas_NotaId",
                table: "Aulas",
                column: "NotaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Aulas_Notas_NotaId",
                table: "Aulas",
                column: "NotaId",
                principalTable: "Notas",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Aulas_Notas_NotaId",
                table: "Aulas");

            migrationBuilder.DropIndex(
                name: "IX_Aulas_NotaId",
                table: "Aulas");

            migrationBuilder.DropColumn(
                name: "NotaId",
                table: "Aulas");

            migrationBuilder.AddColumn<int>(
                name: "AulaDestinadaId",
                table: "Notas",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notas_AulaDestinadaId",
                table: "Notas",
                column: "AulaDestinadaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notas_Aulas_AulaDestinadaId",
                table: "Notas",
                column: "AulaDestinadaId",
                principalTable: "Aulas",
                principalColumn: "Id");
        }
    }
}
