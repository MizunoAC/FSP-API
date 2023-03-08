using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proyecto_faunasilvestre.Migrations
{
    /// <inheritdoc />
    public partial class Tercera : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnimalesCatalogos_ModeloAnimales_ModeloAnimalesId",
                table: "AnimalesCatalogos");

            migrationBuilder.DropIndex(
                name: "IX_AnimalesCatalogos_ModeloAnimalesId",
                table: "AnimalesCatalogos");

            migrationBuilder.DropColumn(
                name: "ModeloAnimalesId",
                table: "AnimalesCatalogos");

            migrationBuilder.AddColumn<int>(
                name: "AnimalesCatalogoId",
                table: "ModeloAnimales",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "NombreComun",
                table: "ModeloAnimales",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CAlimentacion",
                table: "AnimalesCatalogos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CCategoria",
                table: "AnimalesCatalogos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CDescripcionAnimal",
                table: "AnimalesCatalogos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CDistribucion",
                table: "AnimalesCatalogos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CEspecie",
                table: "AnimalesCatalogos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CHabitat",
                table: "AnimalesCatalogos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CHabitos",
                table: "AnimalesCatalogos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<byte[]>(
                name: "CImagenAnimal",
                table: "AnimalesCatalogos",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<string>(
                name: "CNombreComun",
                table: "AnimalesCatalogos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CReproduccion",
                table: "AnimalesCatalogos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<byte[]>(
                name: "DistribucionMapa",
                table: "AnimalesCatalogos",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.CreateIndex(
                name: "IX_ModeloAnimales_AnimalesCatalogoId",
                table: "ModeloAnimales",
                column: "AnimalesCatalogoId");

            migrationBuilder.AddForeignKey(
                name: "FK_ModeloAnimales_AnimalesCatalogos_AnimalesCatalogoId",
                table: "ModeloAnimales",
                column: "AnimalesCatalogoId",
                principalTable: "AnimalesCatalogos",
                principalColumn: "AnimalesCatalogoId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ModeloAnimales_AnimalesCatalogos_AnimalesCatalogoId",
                table: "ModeloAnimales");

            migrationBuilder.DropIndex(
                name: "IX_ModeloAnimales_AnimalesCatalogoId",
                table: "ModeloAnimales");

            migrationBuilder.DropColumn(
                name: "AnimalesCatalogoId",
                table: "ModeloAnimales");

            migrationBuilder.DropColumn(
                name: "NombreComun",
                table: "ModeloAnimales");

            migrationBuilder.DropColumn(
                name: "CAlimentacion",
                table: "AnimalesCatalogos");

            migrationBuilder.DropColumn(
                name: "CCategoria",
                table: "AnimalesCatalogos");

            migrationBuilder.DropColumn(
                name: "CDescripcionAnimal",
                table: "AnimalesCatalogos");

            migrationBuilder.DropColumn(
                name: "CDistribucion",
                table: "AnimalesCatalogos");

            migrationBuilder.DropColumn(
                name: "CEspecie",
                table: "AnimalesCatalogos");

            migrationBuilder.DropColumn(
                name: "CHabitat",
                table: "AnimalesCatalogos");

            migrationBuilder.DropColumn(
                name: "CHabitos",
                table: "AnimalesCatalogos");

            migrationBuilder.DropColumn(
                name: "CImagenAnimal",
                table: "AnimalesCatalogos");

            migrationBuilder.DropColumn(
                name: "CNombreComun",
                table: "AnimalesCatalogos");

            migrationBuilder.DropColumn(
                name: "CReproduccion",
                table: "AnimalesCatalogos");

            migrationBuilder.DropColumn(
                name: "DistribucionMapa",
                table: "AnimalesCatalogos");

            migrationBuilder.AddColumn<int>(
                name: "ModeloAnimalesId",
                table: "AnimalesCatalogos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_AnimalesCatalogos_ModeloAnimalesId",
                table: "AnimalesCatalogos",
                column: "ModeloAnimalesId");

            migrationBuilder.AddForeignKey(
                name: "FK_AnimalesCatalogos_ModeloAnimales_ModeloAnimalesId",
                table: "AnimalesCatalogos",
                column: "ModeloAnimalesId",
                principalTable: "ModeloAnimales",
                principalColumn: "ModeloAnimalesId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
