using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    public partial class changedHijosFromPadreToPersona : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Personas_Personas_PadreId",
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

        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddForeignKey(
                name: "FK_Personas_Personas_PadreId",
                table: "Personas",
                column: "PadreId",
                principalTable: "Personas",
                principalColumn: "Id");
        }
    }
}
