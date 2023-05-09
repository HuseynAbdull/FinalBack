using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinalProjectCode.Migrations
{
    public partial class ProductTypeismain : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsMain",
                table: "ProductTypes",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsMain",
                table: "ProductTypes");
        }
    }
}
