using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proyecto_faunasilvestre.Migrations
{
    /// <inheritdoc />
    public partial class eliminarimagen : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagenAnimal",
                table: "ModeloAnimales");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "ImagenAnimal",
                table: "ModeloAnimales",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);
        }
    }
}
