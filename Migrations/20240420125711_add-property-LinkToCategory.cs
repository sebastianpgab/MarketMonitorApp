using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketMonitorApp.Migrations
{
    public partial class addpropertyLinkToCategory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IdProduct",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LinkToCategory",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdProduct",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "LinkToCategory",
                table: "Categories");
        }
    }
}
