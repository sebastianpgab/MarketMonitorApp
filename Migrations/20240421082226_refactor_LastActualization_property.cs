using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketMonitorApp.Migrations
{
    public partial class refactor_LastActualization_property : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LasActualisation",
                table: "Actualizations",
                newName: "LastActualization");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastActualization",
                table: "Actualizations",
                newName: "LasActualisation");
        }
    }
}
