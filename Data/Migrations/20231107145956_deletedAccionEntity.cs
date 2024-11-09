using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    public partial class deletedAccionEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Grupos_Acciones_AccionId",
                table: "Grupos");

            migrationBuilder.DropTable(
                name: "Acciones");

            migrationBuilder.DropIndex(
                name: "IX_Grupos_AccionId",
                table: "Grupos");

            migrationBuilder.DropColumn(
                name: "AccionId",
                table: "Grupos");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AccionId",
                table: "Grupos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Acciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Acciones", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Grupos_AccionId",
                table: "Grupos",
                column: "AccionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Grupos_Acciones_AccionId",
                table: "Grupos",
                column: "AccionId",
                principalTable: "Acciones",
                principalColumn: "Id");
        }
    }
}
