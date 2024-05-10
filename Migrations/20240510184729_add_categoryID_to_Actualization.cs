using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketMonitorApp.Migrations
{
    public partial class add_categoryID_to_Actualization : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Actualizations",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Actualizations");
        }
    }
}
