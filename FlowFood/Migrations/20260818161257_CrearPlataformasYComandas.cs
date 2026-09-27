using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlowFood.Migrations
{
    /// <inheritdoc />
    public partial class CrearPlataformasYComandas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Plataformas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Estado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plataformas", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comandas_ClienteId",
                table: "Comandas",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Comandas_MetodoPagoId",
                table: "Comandas",
                column: "MetodoPagoId");

            migrationBuilder.CreateIndex(
                name: "IX_Comandas_PlataformaId",
                table: "Comandas",
                column: "PlataformaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comandas_Clientes_ClienteId",
                table: "Comandas",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Comandas_MetodosPago_MetodoPagoId",
                table: "Comandas",
                column: "MetodoPagoId",
                principalTable: "MetodosPago",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Comandas_Plataformas_PlataformaId",
                table: "Comandas",
                column: "PlataformaId",
                principalTable: "Plataformas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comandas_Clientes_ClienteId",
                table: "Comandas");

            migrationBuilder.DropForeignKey(
                name: "FK_Comandas_MetodosPago_MetodoPagoId",
                table: "Comandas");

            migrationBuilder.DropForeignKey(
                name: "FK_Comandas_Plataformas_PlataformaId",
                table: "Comandas");

            migrationBuilder.DropTable(
                name: "Plataformas");

            migrationBuilder.DropIndex(
                name: "IX_Comandas_ClienteId",
                table: "Comandas");

            migrationBuilder.DropIndex(
                name: "IX_Comandas_MetodoPagoId",
                table: "Comandas");

            migrationBuilder.DropIndex(
                name: "IX_Comandas_PlataformaId",
                table: "Comandas");
        }
    }
}
