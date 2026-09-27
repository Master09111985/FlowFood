using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlowFood.Migrations
{
    /// <inheritdoc />
    public partial class CreandoRegistroDeBajas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bajas_Inventarios_inventarioId",
                table: "Bajas");

            migrationBuilder.DropForeignKey(
                name: "FK_Bajas_MotivosBaja_motivoBajaId",
                table: "Bajas");

            migrationBuilder.RenameColumn(
                name: "motivoBajaId",
                table: "Bajas",
                newName: "MotivoBajaId");

            migrationBuilder.RenameColumn(
                name: "inventarioId",
                table: "Bajas",
                newName: "InventarioId");

            migrationBuilder.RenameIndex(
                name: "IX_Bajas_motivoBajaId",
                table: "Bajas",
                newName: "IX_Bajas_MotivoBajaId");

            migrationBuilder.RenameIndex(
                name: "IX_Bajas_inventarioId",
                table: "Bajas",
                newName: "IX_Bajas_InventarioId");

            migrationBuilder.AlterColumn<string>(
                name: "Comentarios",
                table: "Bajas",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddForeignKey(
                name: "FK_Bajas_Inventarios_InventarioId",
                table: "Bajas",
                column: "InventarioId",
                principalTable: "Inventarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Bajas_MotivosBaja_MotivoBajaId",
                table: "Bajas",
                column: "MotivoBajaId",
                principalTable: "MotivosBaja",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bajas_Inventarios_InventarioId",
                table: "Bajas");

            migrationBuilder.DropForeignKey(
                name: "FK_Bajas_MotivosBaja_MotivoBajaId",
                table: "Bajas");

            migrationBuilder.RenameColumn(
                name: "MotivoBajaId",
                table: "Bajas",
                newName: "motivoBajaId");

            migrationBuilder.RenameColumn(
                name: "InventarioId",
                table: "Bajas",
                newName: "inventarioId");

            migrationBuilder.RenameIndex(
                name: "IX_Bajas_MotivoBajaId",
                table: "Bajas",
                newName: "IX_Bajas_motivoBajaId");

            migrationBuilder.RenameIndex(
                name: "IX_Bajas_InventarioId",
                table: "Bajas",
                newName: "IX_Bajas_inventarioId");

            migrationBuilder.AlterColumn<string>(
                name: "Comentarios",
                table: "Bajas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Bajas_Inventarios_inventarioId",
                table: "Bajas",
                column: "inventarioId",
                principalTable: "Inventarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Bajas_MotivosBaja_motivoBajaId",
                table: "Bajas",
                column: "motivoBajaId",
                principalTable: "MotivosBaja",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
