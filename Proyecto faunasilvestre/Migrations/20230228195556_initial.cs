using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proyecto_faunasilvestre.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ModeloUsuarios",
                columns: table => new
                {
                    ModeloUsuarioId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreUsuario = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ApellidoPaterno = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ApellidoMaterno = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Localidad = table.Column<int>(type: "int", nullable: false),
                    Otralocalidad = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Sexo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Edad = table.Column<int>(type: "int", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Contraseña = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    TipoUsuario = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModeloUsuarios", x => x.ModeloUsuarioId);
                });

            migrationBuilder.CreateTable(
                name: "ModeloAnimales",
                columns: table => new
                {
                    ModeloAnimalesId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ModeloUsuarioId = table.Column<int>(type: "int", nullable: false),
                    ImagenAnimal = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    CondicionAnimal = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Descripcionanimal = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModeloAnimales", x => x.ModeloAnimalesId);
                    table.ForeignKey(
                        name: "FK_ModeloAnimales_ModeloUsuarios_ModeloUsuarioId",
                        column: x => x.ModeloUsuarioId,
                        principalTable: "ModeloUsuarios",
                        principalColumn: "ModeloUsuarioId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AnimalesCatalogos",
                columns: table => new
                {
                    AnimalesCatalogoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ModeloAnimalesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnimalesCatalogos", x => x.AnimalesCatalogoId);
                    table.ForeignKey(
                        name: "FK_AnimalesCatalogos_ModeloAnimales_ModeloAnimalesId",
                        column: x => x.ModeloAnimalesId,
                        principalTable: "ModeloAnimales",
                        principalColumn: "ModeloAnimalesId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnimalesCatalogos_ModeloAnimalesId",
                table: "AnimalesCatalogos",
                column: "ModeloAnimalesId");

            migrationBuilder.CreateIndex(
                name: "IX_ModeloAnimales_ModeloUsuarioId",
                table: "ModeloAnimales",
                column: "ModeloUsuarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnimalesCatalogos");

            migrationBuilder.DropTable(
                name: "ModeloAnimales");

            migrationBuilder.DropTable(
                name: "ModeloUsuarios");
        }
    }
}
