using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    public partial class agregoFirmayLectura : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NotaPersona1",
                columns: table => new
                {
                    LeidaPorId = table.Column<int>(type: "int", nullable: false),
                    NotasLeidasId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotaPersona1", x => new { x.LeidaPorId, x.NotasLeidasId });
                    table.ForeignKey(
                        name: "FK_NotaPersona1_Notas_NotasLeidasId",
                        column: x => x.NotasLeidasId,
                        principalTable: "Notas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NotaPersona1_Personas_LeidaPorId",
                        column: x => x.LeidaPorId,
                        principalTable: "Personas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NotaPersona2",
                columns: table => new
                {
                    FirmadaPorId = table.Column<int>(type: "int", nullable: false),
                    NotasFirmadasId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotaPersona2", x => new { x.FirmadaPorId, x.NotasFirmadasId });
                    table.ForeignKey(
                        name: "FK_NotaPersona2_Notas_NotasFirmadasId",
                        column: x => x.NotasFirmadasId,
                        principalTable: "Notas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NotaPersona2_Personas_FirmadaPorId",
                        column: x => x.FirmadaPorId,
                        principalTable: "Personas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NotaPersona1_NotasLeidasId",
                table: "NotaPersona1",
                column: "NotasLeidasId");

            migrationBuilder.CreateIndex(
                name: "IX_NotaPersona2_NotasFirmadasId",
                table: "NotaPersona2",
                column: "NotasFirmadasId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NotaPersona1");

            migrationBuilder.DropTable(
                name: "NotaPersona2");
        }
    }
}
