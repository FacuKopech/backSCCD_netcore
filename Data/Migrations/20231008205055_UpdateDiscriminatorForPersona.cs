using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    public partial class UpdateDiscriminatorForPersona : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE Personas SET Discriminator = 'Persona' WHERE Discriminator <> 'Alumno'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
