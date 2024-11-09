using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    public partial class modifiedPersonaEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Personas_Personas_PersonaId",
                table: "Personas");

            migrationBuilder.RenameColumn(
                name: "PersonaId",
                table: "Personas",
                newName: "PadreId");

            migrationBuilder.RenameIndex(
                name: "IX_Personas_PersonaId",
                table: "Personas",
                newName: "IX_Personas_PadreId");

            migrationBuilder.AddColumn<int>(
                name: "DirectivoId",
                table: "Personas",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DocenteId",
                table: "Personas",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Personas_DirectivoId",
                table: "Personas",
                column: "DirectivoId");

            migrationBuilder.CreateIndex(
                name: "IX_Personas_DocenteId",
                table: "Personas",
                column: "DocenteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Personas_Personas_DirectivoId",
                table: "Personas",
                column: "DirectivoId",
                principalTable: "Personas",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Personas_Personas_DocenteId",
                table: "Personas",
                column: "DocenteId",
                principalTable: "Personas",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Personas_Personas_PadreId",
                table: "Personas",
                column: "PadreId",
                principalTable: "Personas",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Personas_Personas_DirectivoId",
                table: "Personas");

            migrationBuilder.DropForeignKey(
                name: "FK_Personas_Personas_DocenteId",
                table: "Personas");

            migrationBuilder.DropForeignKey(
                name: "FK_Personas_Personas_PadreId",
                table: "Personas");

            migrationBuilder.DropIndex(
                name: "IX_Personas_DirectivoId",
                table: "Personas");

            migrationBuilder.DropIndex(
                name: "IX_Personas_DocenteId",
                table: "Personas");

            migrationBuilder.DropColumn(
                name: "DirectivoId",
                table: "Personas");

            migrationBuilder.DropColumn(
                name: "DocenteId",
                table: "Personas");

            migrationBuilder.RenameColumn(
                name: "PadreId",
                table: "Personas",
                newName: "PersonaId");

            migrationBuilder.RenameIndex(
                name: "IX_Personas_PadreId",
                table: "Personas",
                newName: "IX_Personas_PersonaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Personas_Personas_PersonaId",
                table: "Personas",
                column: "PersonaId",
                principalTable: "Personas",
                principalColumn: "Id");
        }
    }
}
