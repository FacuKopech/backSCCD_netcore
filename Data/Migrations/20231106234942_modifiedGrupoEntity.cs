using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    public partial class modifiedGrupoEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccionGrupo");

            migrationBuilder.DropTable(
                name: "GrupoUsuario");

            migrationBuilder.AddColumn<int>(
                name: "AccionId",
                table: "Grupos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioId",
                table: "Grupos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Grupos_AccionId",
                table: "Grupos",
                column: "AccionId");

            migrationBuilder.CreateIndex(
                name: "IX_Grupos_UsuarioId",
                table: "Grupos",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Grupos_Acciones_AccionId",
                table: "Grupos",
                column: "AccionId",
                principalTable: "Acciones",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Grupos_Usuarios_UsuarioId",
                table: "Grupos",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Grupos_Acciones_AccionId",
                table: "Grupos");

            migrationBuilder.DropForeignKey(
                name: "FK_Grupos_Usuarios_UsuarioId",
                table: "Grupos");

            migrationBuilder.DropIndex(
                name: "IX_Grupos_AccionId",
                table: "Grupos");

            migrationBuilder.DropIndex(
                name: "IX_Grupos_UsuarioId",
                table: "Grupos");

            migrationBuilder.DropColumn(
                name: "AccionId",
                table: "Grupos");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "Grupos");

            migrationBuilder.CreateTable(
                name: "AccionGrupo",
                columns: table => new
                {
                    AccionesId = table.Column<int>(type: "int", nullable: false),
                    GruposId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccionGrupo", x => new { x.AccionesId, x.GruposId });
                    table.ForeignKey(
                        name: "FK_AccionGrupo_Acciones_AccionesId",
                        column: x => x.AccionesId,
                        principalTable: "Acciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccionGrupo_Grupos_GruposId",
                        column: x => x.GruposId,
                        principalTable: "Grupos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GrupoUsuario",
                columns: table => new
                {
                    GruposId = table.Column<int>(type: "int", nullable: false),
                    UsuariosId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GrupoUsuario", x => new { x.GruposId, x.UsuariosId });
                    table.ForeignKey(
                        name: "FK_GrupoUsuario_Grupos_GruposId",
                        column: x => x.GruposId,
                        principalTable: "Grupos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GrupoUsuario_Usuarios_UsuariosId",
                        column: x => x.UsuariosId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccionGrupo_GruposId",
                table: "AccionGrupo",
                column: "GruposId");

            migrationBuilder.CreateIndex(
                name: "IX_GrupoUsuario_UsuariosId",
                table: "GrupoUsuario",
                column: "UsuariosId");
        }
    }
}
