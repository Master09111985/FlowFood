using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlowFood.Migrations
{
    /// <inheritdoc />
    public partial class Actualizacion_ProductosInventarios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Inventarios_UnidadMedidas_unidadMedidaId",
                table: "Inventarios");

            migrationBuilder.DropIndex(
                name: "IX_Inventarios_unidadMedidaId",
                table: "Inventarios");

            migrationBuilder.DropColumn(
                name: "unidadMedidaId",
                table: "Inventarios");

            migrationBuilder.AddColumn<int>(
                name: "unidadId",
                table: "Productos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Productos_unidadId",
                table: "Productos",
                column: "unidadId");

            migrationBuilder.AddForeignKey(
                name: "FK_Productos_UnidadMedidas_unidadId",
                table: "Productos",
                column: "unidadId",
                principalTable: "UnidadMedidas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Productos_UnidadMedidas_unidadId",
                table: "Productos");

            migrationBuilder.DropIndex(
                name: "IX_Productos_unidadId",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "unidadId",
                table: "Productos");

            migrationBuilder.AddColumn<int>(
                name: "unidadMedidaId",
                table: "Inventarios",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Inventarios_unidadMedidaId",
                table: "Inventarios",
                column: "unidadMedidaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Inventarios_UnidadMedidas_unidadMedidaId",
                table: "Inventarios",
                column: "unidadMedidaId",
                principalTable: "UnidadMedidas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
