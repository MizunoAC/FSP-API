using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proyecto_faunasilvestre.Migrations
{
    /// <inheritdoc />
    public partial class AgregarColumnacorreo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Correo",
                table: "codigos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Correo",
                table: "codigos");
        }
    }
}
