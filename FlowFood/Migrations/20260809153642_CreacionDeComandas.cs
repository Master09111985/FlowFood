using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlowFood.Migrations
{
    /// <inheritdoc />
    public partial class CreacionDeComandas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Comandas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TipoPedido = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NumeroMesa = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PlataformaNombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DireccionEntrega = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    HoraEntrega = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Subtotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comandas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ComandaDetalles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ComandaId = table.Column<int>(type: "int", nullable: false),
                    PlatilloId = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    PrecioUnitario = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Subtotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Notas = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComandaDetalles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComandaDetalles_Comandas_ComandaId",
                        column: x => x.ComandaId,
                        principalTable: "Comandas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ComandaDetalles_Platillos_PlatilloId",
                        column: x => x.PlatilloId,
                        principalTable: "Platillos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ComandaDetalles_ComandaId",
                table: "ComandaDetalles",
                column: "ComandaId");

            migrationBuilder.CreateIndex(
                name: "IX_ComandaDetalles_PlatilloId",
                table: "ComandaDetalles",
                column: "PlatilloId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ComandaDetalles");

            migrationBuilder.DropTable(
                name: "Comandas");
        }
    }
}
