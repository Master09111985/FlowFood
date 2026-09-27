using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlowFood.Migrations
{
    /// <inheritdoc />
    public partial class CreandoMetodoPago : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DireccionEntrega",
                table: "Comandas");

            migrationBuilder.DropColumn(
                name: "Estado",
                table: "Comandas");

            migrationBuilder.DropColumn(
                name: "PlataformaNombre",
                table: "Comandas");

            migrationBuilder.RenameColumn(
                name: "HoraEntrega",
                table: "Comandas",
                newName: "FechaHoraAgendada");

            migrationBuilder.AlterColumn<int>(
                name: "TipoPedido",
                table: "Comandas",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<int>(
                name: "ClienteId",
                table: "Comandas",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Estatus",
                table: "Comandas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MetodoPagoId",
                table: "Comandas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "NombreClienteLlevar",
                table: "Comandas",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PlataformaId",
                table: "Comandas",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MetodosPago",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Estado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetodosPago", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MetodosPago");

            migrationBuilder.DropColumn(
                name: "ClienteId",
                table: "Comandas");

            migrationBuilder.DropColumn(
                name: "Estatus",
                table: "Comandas");

            migrationBuilder.DropColumn(
                name: "MetodoPagoId",
                table: "Comandas");

            migrationBuilder.DropColumn(
                name: "NombreClienteLlevar",
                table: "Comandas");

            migrationBuilder.DropColumn(
                name: "PlataformaId",
                table: "Comandas");

            migrationBuilder.RenameColumn(
                name: "FechaHoraAgendada",
                table: "Comandas",
                newName: "HoraEntrega");

            migrationBuilder.AlterColumn<string>(
                name: "TipoPedido",
                table: "Comandas",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "DireccionEntrega",
                table: "Comandas",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Estado",
                table: "Comandas",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PlataformaNombre",
                table: "Comandas",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }
    }
}
