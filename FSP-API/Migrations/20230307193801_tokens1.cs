using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FSP_API.Migrations
{
    /// <inheritdoc />
    public partial class tokens1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModeloUsuarioId = table.Column<int>(type: "int", nullable: false),
                    fecha = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tokens_ModeloUsuarios_ModeloUsuarioId",
                        column: x => x.ModeloUsuarioId,
                        principalTable: "ModeloUsuarios",
                        principalColumn: "ModeloUsuarioId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tokens_ModeloUsuarioId",
                table: "tokens",
                column: "ModeloUsuarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tokens");
        }
    }
}
