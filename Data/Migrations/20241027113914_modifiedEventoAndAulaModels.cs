using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    public partial class modifiedEventoAndAulaModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AulaDestinadaId",
                table: "Eventos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Eventos_AulaDestinadaId",
                table: "Eventos",
                column: "AulaDestinadaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Eventos_Aulas_AulaDestinadaId",
                table: "Eventos",
                column: "AulaDestinadaId",
                principalTable: "Aulas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Eventos_Aulas_AulaDestinadaId",
                table: "Eventos");

            migrationBuilder.DropIndex(
                name: "IX_Eventos_AulaDestinadaId",
                table: "Eventos");

            migrationBuilder.DropColumn(
                name: "AulaDestinadaId",
                table: "Eventos");
        }
    }
}
