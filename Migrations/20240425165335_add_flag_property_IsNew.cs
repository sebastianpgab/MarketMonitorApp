using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketMonitorApp.Migrations
{
    public partial class add_flag_property_IsNew : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsNew",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsNew",
                table: "Products");
        }
    }
}
