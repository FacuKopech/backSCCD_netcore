using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    public partial class changingManyToManyAulasDestinadasInNota : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.CreateTable(
                name: "AulaNota",
                columns: table => new
                {
                    AulasDestinadasId = table.Column<int>(type: "int", nullable: false),
                    NotasParaAulaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AulaNota", x => new { x.AulasDestinadasId, x.NotasParaAulaId });
                    table.ForeignKey(
                        name: "FK_AulaNota_Aulas_AulasDestinadasId",
                        column: x => x.AulasDestinadasId,
                        principalTable: "Aulas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AulaNota_Notas_NotasParaAulaId",
                        column: x => x.NotasParaAulaId,
                        principalTable: "Notas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AulaNota_NotasParaAulaId",
                table: "AulaNota",
                column: "NotasParaAulaId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AulaNota");

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
    }
}
