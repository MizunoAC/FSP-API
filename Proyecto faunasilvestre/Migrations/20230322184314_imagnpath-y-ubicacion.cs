using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proyecto_faunasilvestre.Migrations
{
    /// <inheritdoc />
    public partial class imagnpathyubicacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagenAnimal",
                table: "ModeloAnimales",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Ubicacion",
                table: "ModeloAnimales",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagenAnimal",
                table: "ModeloAnimales");

            migrationBuilder.DropColumn(
                name: "Ubicacion",
                table: "ModeloAnimales");
        }
    }
}
