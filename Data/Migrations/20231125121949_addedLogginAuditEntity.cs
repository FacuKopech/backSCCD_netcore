using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    public partial class addedLogginAuditEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LoginAudit",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioLogueadoId = table.Column<int>(type: "int", nullable: false),
                    FechaYHoraLogin = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaYHoraLogout = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoginAudit", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoginAudit_Usuarios_UsuarioLogueadoId",
                        column: x => x.UsuarioLogueadoId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LoginAudit_UsuarioLogueadoId",
                table: "LoginAudit",
                column: "UsuarioLogueadoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LoginAudit");
        }
    }
}
