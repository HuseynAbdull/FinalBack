using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinalProjectCode.Migrations
{
    public partial class NewArrivalColumnAddedToProducts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsNewArrival",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsNewArrival",
                table: "Products");
        }
    }
}
