using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventarioService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreateUniqueIndexEventoId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_EventoProcesado_EventoId",
                table: "EventoProcesado",
                column: "EventoId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EventoProcesado_EventoId",
                table: "EventoProcesado");
        }
    }
}
