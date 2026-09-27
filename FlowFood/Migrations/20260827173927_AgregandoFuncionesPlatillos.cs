using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlowFood.Migrations
{
    /// <inheritdoc />
    public partial class AgregandoFuncionesPlatillos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Prioridad",
                table: "Comandas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumeroPlato",
                table: "ComandaDetalles",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Prioridad",
                table: "Comandas");

            migrationBuilder.DropColumn(
                name: "NumeroPlato",
                table: "ComandaDetalles");
        }
    }
}
